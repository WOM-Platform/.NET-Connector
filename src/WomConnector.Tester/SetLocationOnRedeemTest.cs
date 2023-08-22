using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using WomPlatform.Connector.Models;

namespace WomConnector.Tester {
    public class SetLocationOnRedeemTest {

        [Test]
        public async Task CreateVouchersWithoutLocation() {
            var instrument = Util.GenerateInstrument();
            var response = await instrument.RequestVouchers(new VoucherCreatePayload.VoucherInfo[] {
                new VoucherCreatePayload.VoucherInfo {
                    Aim = "H",
                    Count = 10,
                    Timestamp = DateTime.UtcNow,
                    CreationMode = VoucherCreatePayload.VoucherCreationMode.SetLocationOnRedeem
                }
            });

            Console.WriteLine("Voucher link: {0}", response.Link);
            Console.WriteLine("Password: {0}", response.Password);
        }

            [Test]
        public async Task SimpleSetLocationOnRedeemTest() {
            var pocket = Util.CreatePocket();

            var instrument = Util.GenerateInstrument();
            var response = await instrument.RequestVouchers(new VoucherCreatePayload.VoucherInfo[] {
                new VoucherCreatePayload.VoucherInfo {
                    Aim = "H",
                    Count = 10,
                    Timestamp = DateTime.UtcNow,
                    CreationMode = VoucherCreatePayload.VoucherCreationMode.SetLocationOnRedeem
                }
            });

            await pocket.CollectVouchers(response.OtcGen, response.Password,
                redeemLocation: new WomPlatform.Connector.GeoCoords {
                    Latitude = 11,
                    Longitude = 12
                }
            );

            Assert.AreEqual(10, pocket.VoucherCount);

            var pos = Util.GeneratePos();
            var responsePay = await pos.RequestPayment(
                10,
                "https://example.org",
                filter: new SimpleFilter {
                    MaxAge = 1,
                    Bounds = new Bounds {
                        LeftTop = new double[] { 13, 9.5 },
                        RightBottom = new double[] { 8, 16 }
                    }
                }
            );

            string ackUrl = await pocket.PayWithRandomVouchers(responsePay.OtcPay, responsePay.Password);
            Assert.AreEqual(0, pocket.VoucherCount);
            Assert.AreEqual("https://example.org", ackUrl);
        }

    }
}
