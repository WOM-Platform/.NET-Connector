using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Org.BouncyCastle.Crypto;
using WomPlatform.Connector;
using WomPlatform.Connector.Models;

namespace WomConnector.Tester {

    public class PointOfSaleTest {

        [Test]
        public async Task CreateSimplePayment() {
            var pos = Util.CreatePos(1, "keys/pos1.pem");

            var (otc, password) = await pos.RequestPayment(1, "https://example.org");

            Console.WriteLine("Payment {0} pwd {1}", otc, password);
        }

        [Test]
        public async Task ExecuteSimplePaymentExchange() {
            var pocket = Util.CreatePocket();

            var (reqOtc, reqPassword) = await InstrumentTest.GenerateVoucherRequestSource1(1);

            await pocket.CollectVouchers(reqOtc, reqPassword);
            Assert.AreEqual(1, pocket.VoucherCount);

            var pos = Util.CreatePos(1, "keys/pos1.pem");
            var (payOtc, payPassword) = await pos.RequestPayment(1, "https://example.org");

            string ackUrl = await pocket.PayWithRandomVouchers(payOtc, payPassword);
            Assert.AreEqual(0, pocket.VoucherCount);
            Assert.AreEqual("https://example.org", ackUrl);
        }

        [Test]
        public async Task ExecuteFilteredPaymentExchange() {
            var pocket = Util.CreatePocket();

            var instrument = Util.CreateInstrument(1, "keys/source1.pem");
            var (reqOtc, reqPassword) = await instrument.RequestVouchers(new VoucherCreatePayload.VoucherInfo[] {
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

            await pocket.CollectVouchers(reqOtc, reqPassword);
            Assert.AreEqual(3, pocket.VoucherCount);

            var pos = Util.CreatePos(1, "keys/pos1.pem");
            var (payOtc, payPassword) = await pos.RequestPayment(2, "https://example.org",
                filter: new SimpleFilter {
                    Aim = "H"
                }
            );

            string ackUrl = await pocket.PayWithRandomVouchers(payOtc, payPassword);
            Assert.AreEqual(1, pocket.VoucherCount);
            Assert.AreEqual("https://example.org", ackUrl);
            Assert.AreEqual("E", pocket.Vouchers[0].Aim);
        }

        [Test]
        public async Task ExecuteInsufficientExchange() {
            var pocket = Util.CreatePocket();

            var instrument = Util.CreateInstrument(1, "keys/source1.pem");
            var (reqOtc, reqPassword) = await instrument.RequestVouchers(new VoucherCreatePayload.VoucherInfo[] {
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

            await pocket.CollectVouchers(reqOtc, reqPassword);
            Assert.AreEqual(3, pocket.VoucherCount);

            var pos = Util.CreatePos(1, "keys/pos1.pem");
            var (payOtc, payPassword) = await pos.RequestPayment(4, "https://example.org");

            Assert.ThrowsAsync<InvalidOperationException>(async () => {
                await pocket.PayWithRandomVouchers(payOtc, payPassword);
            });
            
            Assert.AreEqual(3, pocket.VoucherCount);
        }

        [Test]
        public async Task ExecuteWrongExchange() {
            var pocket = Util.CreatePocket();

            var instrument = Util.CreateInstrument(1, "keys/source1.pem");
            var (reqOtc, reqPassword) = await instrument.RequestVouchers(new VoucherCreatePayload.VoucherInfo[] {
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

            await pocket.CollectVouchers(reqOtc, reqPassword);
            Assert.AreEqual(4, pocket.VoucherCount);

            var pos = Util.CreatePos(1, "keys/pos1.pem");
            var (payOtc, payPassword) = await pos.RequestPayment(3, "https://example.org",
                filter: new SimpleFilter {
                    Aim = "H",
                    Bounds = new Bounds {
                        LeftTop = new double[] { 43.797161, 12.488031 },
                        RightBottom = new double[] { 43.636500, 12.796507 }
                    }
                }
            );

            Assert.ThrowsAsync<InvalidOperationException>(async () => {
                await pocket.PayWithRandomVouchers(payOtc, payPassword);
            });

            Assert.AreEqual(4, pocket.VoucherCount);
        }

    }

}
