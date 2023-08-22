using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using WomPlatform.Connector;
using WomPlatform.Connector.Models;

namespace WomConnector.Tester {

    public class PocketTest {

        [Test]
        public async Task CreateSimpleVoucher() {
            var pocket = Util.CreatePocket();

            var instrument = Util.GenerateInstrument();
            var response = await instrument.RequestVouchers(new VoucherCreatePayload.VoucherInfo[] {
                new VoucherCreatePayload.VoucherInfo {
                    Aim = "E",
                    Count = 10,
                    Latitude = 43.72621,
                    Longitude = 12.63633,
                    Timestamp = DateTime.UtcNow
                }
            });

            await pocket.CollectVouchers(response.OtcGen, response.Password);

            Assert.AreEqual(10, pocket.VoucherCount);
        }

        [Test]
        public async Task SpendOldLegacyVoucher() {
            var pocket = Util.CreatePocket();
            pocket.AddVouchers(new Voucher[] {
                new Voucher(
                    new Identifier(17),
                    "nKCK1AMm4ruDp/c9EZhOjw==",
                    "C",
                    43.7244397,
                    12.6397422,
                    new DateTime(2019, 07, 18)
                )
            });

            var pos = Util.GeneratePos();
            var payment = await pos.RequestPayment(1, "https://example.org", null, new SimpleFilter {
                Aim = "C"
            });

            var pocketAck = await pocket.PayWithRandomVouchers(payment.OtcPay, payment.Password);

            Assert.AreEqual("https://example.org", pocketAck);
        }

    }

}
