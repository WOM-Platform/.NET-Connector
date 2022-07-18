using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Org.BouncyCastle.Crypto;
using WomPlatform.Connector;
using WomPlatform.Connector.Models;

namespace WomConnector.Tester {

    public class PointOfSaleTest {

        private PointOfSale _pos;

        [SetUp]
        public void Setup() {
            _pos = Util.GeneratePos();
        }

        [Test]
        public async Task CreateSimplePayment() {
            var response = await _pos.RequestPayment(1, "https://example.org");

            Console.WriteLine("Payment {0} pwd {1}", response.OtcPay, response.Password);
        }

        [Test]
        public async Task ExecuteSimplePaymentExchange() {
            var pocket = Util.CreatePocket();

            var instrument = Util.GenerateInstrument();
            var response = await instrument.RequestVouchers(new VoucherCreatePayload.VoucherInfo[] {
                new VoucherCreatePayload.VoucherInfo {
                    Aim = "H",
                    Latitude = 10,
                    Longitude = 10,
                    Count = 1,
                    Timestamp = DateTime.UtcNow
                }
            });

            await pocket.CollectVouchers(response.OtcGen, response.Password);
            Assert.AreEqual(1, pocket.VoucherCount);

            var singleVoucher = pocket.Vouchers[0];

            var respPay1 = await _pos.RequestPayment(1, "https://example.org");

            string ackUrl = await pocket.PayWithRandomVouchers(respPay1.OtcPay, respPay1.Password);
            Assert.AreEqual(0, pocket.VoucherCount);
            Assert.AreEqual("https://example.org", ackUrl);

            // Test double spending
            var respPay2 = await _pos.RequestPayment(1, "https://example.org");

            Assert.ThrowsAsync<InvalidOperationException>(async () => {
                await pocket.Pay(respPay2.OtcPay, respPay2.Password, new Voucher[] { singleVoucher });
            });
            
        }

        [Test]
        public async Task ExecuteFilteredPaymentExchange() {
            var pocket = Util.CreatePocket();

            var instrument = Util.GenerateInstrument();
            var response = await instrument.RequestVouchers(new VoucherCreatePayload.VoucherInfo[] {
                new VoucherCreatePayload.VoucherInfo {
                    Aim = "E",
                    Count = 1,
                    Latitude = 43.72621,
                    Longitude = 12.63633,
                    Timestamp = DateTime.UtcNow.Subtract(TimeSpan.FromDays(30))
                },
                new VoucherCreatePayload.VoucherInfo {
                    Aim = "H",
                    Count = 2,
                    Latitude = 41.72621,
                    Longitude = 15.63633,
                    Timestamp = DateTime.UtcNow.Subtract(TimeSpan.FromDays(30))
                }
            });

            await pocket.CollectVouchers(response.OtcGen, response.Password);
            Assert.AreEqual(3, pocket.VoucherCount);

            var pos = Util.GeneratePos();
            var responsePay = await pos.RequestPayment(2, "https://example.org",
                filter: new SimpleFilter {
                    Aim = "H"
                }
            );

            string ackUrl = await pocket.PayWithRandomVouchers(responsePay.OtcPay, responsePay.Password);
            Assert.AreEqual(1, pocket.VoucherCount);
            Assert.AreEqual("https://example.org", ackUrl);
            Assert.AreEqual("E", pocket.Vouchers[0].Aim);
        }

        [Test]
        public async Task ExecuteInsufficientExchange() {
            var pocket = Util.CreatePocket();

            var instrument = Util.GenerateInstrument();
            var response = await instrument.RequestVouchers(new VoucherCreatePayload.VoucherInfo[] {
                new VoucherCreatePayload.VoucherInfo {
                    Aim = "E",
                    Count = 1,
                    Latitude = 43.72621,
                    Longitude = 12.63633,
                    Timestamp = DateTime.UtcNow.Subtract(TimeSpan.FromDays(30))
                },
                new VoucherCreatePayload.VoucherInfo {
                    Aim = "H",
                    Count = 2,
                    Latitude = 41.72621,
                    Longitude = 15.63633,
                    Timestamp = DateTime.UtcNow.Subtract(TimeSpan.FromDays(30))
                }
            });

            await pocket.CollectVouchers(response.OtcGen, response.Password);
            Assert.AreEqual(3, pocket.VoucherCount);

            var pos = Util.GeneratePos();
            var responsePay = await pos.RequestPayment(4, "https://example.org");

            Assert.ThrowsAsync<InvalidOperationException>(async () => {
                await pocket.PayWithRandomVouchers(responsePay.OtcPay, responsePay.Password);
            });
            
            Assert.AreEqual(3, pocket.VoucherCount);
        }

        [Test]
        public async Task ExecuteWrongExchange() {
            var pocket = Util.CreatePocket();

            var instrument = Util.GenerateInstrument();
            var response = await instrument.RequestVouchers(new VoucherCreatePayload.VoucherInfo[] {
                new VoucherCreatePayload.VoucherInfo {
                    Aim = "H",
                    Count = 2,
                    Latitude = 43.72621,
                    Longitude = 12.63633,
                    Timestamp = DateTime.UtcNow.Subtract(TimeSpan.FromDays(30))
                },
                new VoucherCreatePayload.VoucherInfo {
                    Aim = "H",
                    Count = 2,
                    Latitude = 41.72621,
                    Longitude = 15.63633,
                    Timestamp = DateTime.UtcNow.Subtract(TimeSpan.FromDays(30))
                }
            });

            await pocket.CollectVouchers(response.OtcGen, response.Password);
            Assert.AreEqual(4, pocket.VoucherCount);

            var pos = Util.GeneratePos();
            var responsePay = await pos.RequestPayment(3, "https://example.org",
                filter: new SimpleFilter {
                    Aim = "H",
                    Bounds = new Bounds {
                        LeftTop = new double[] { 43.797161, 12.488031 },
                        RightBottom = new double[] { 43.636500, 12.796507 }
                    }
                }
            );

            Assert.ThrowsAsync<InvalidOperationException>(async () => {
                await pocket.PayWithRandomVouchers(responsePay.OtcPay, responsePay.Password);
            });

            Assert.AreEqual(4, pocket.VoucherCount);
        }

        [Test]
        public async Task CheckPaymentInformation() {
            var pos = Util.GeneratePos();

            var request = await pos.RequestPayment(
                amount: 10,
                pocketAckUrl: "https://example.org",
                filter: new SimpleFilter {
                    Aim = "H"
                }
            );

            var info = await pos.GetPaymentInformation(request.OtcPay);
            Assert.AreEqual(pos.Identifier, info.PosId);
            Assert.AreEqual(0, info.Confirmations.Count);

            var pocket = Util.CreatePocket();

            var instrument = Util.GenerateInstrument();
            var response = await instrument.RequestVouchers(new VoucherCreatePayload.VoucherInfo[] {
                new VoucherCreatePayload.VoucherInfo {
                    Aim = "E",
                    Count = 5,
                    Latitude = 43.72621,
                    Longitude = 12.63633,
                    Timestamp = DateTime.UtcNow.Subtract(TimeSpan.FromDays(30))
                },
                new VoucherCreatePayload.VoucherInfo {
                    Aim = "H",
                    Count = 10,
                    Latitude = 41.72621,
                    Longitude = 15.63633,
                    Timestamp = DateTime.UtcNow.Subtract(TimeSpan.FromDays(10))
                }
            });

            await pocket.CollectVouchers(response.OtcGen, response.Password);

            var paymentAckUrl = await pocket.PayWithRandomVouchers(response.OtcGen, response.Password);
            Assert.AreEqual("https://example.org", paymentAckUrl);

            info = await pos.GetPaymentInformation(request.OtcPay);
            Assert.AreEqual(pos.Identifier, info.PosId);
            Assert.GreaterOrEqual(1, info.Confirmations.Count);
        }

    }

}
