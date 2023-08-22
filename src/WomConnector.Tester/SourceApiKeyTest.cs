using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace WomConnector.Tester {
    public class SourceApiKeyTest {

        [Test]
        public async Task CreateApiKey() {
            var apiKey1 = await Util.Client.CreateSourceApiKey(Util.Instrument1Identifier, Util.AdminEmail, Util.AdminPassword, "Connector_Test");
            Console.WriteLine("API key: {0}", apiKey1);
            Assert.NotNull(apiKey1);

            var apiKey2 = await Util.Client.CreateSourceApiKey(Util.Instrument1Identifier, Util.AdminEmail, Util.AdminPassword, "Connector_Test");
            Assert.AreEqual(apiKey1, apiKey2);
        }

        [Test]
        public async Task CreateVouchersWithApiKey() {
            var instrument = await Util.Client.CreateInstrument(Util.Instrument1Identifier, Util.AdminEmail, Util.AdminPassword, "Connector_Test");
            Assert.AreEqual(Util.Instrument1Identifier, instrument.Identifier);

            var voucherResponse = await instrument.RequestVouchers(new WomPlatform.Connector.Models.VoucherCreatePayload.VoucherInfo[] {
                new WomPlatform.Connector.Models.VoucherCreatePayload.VoucherInfo {
                    Aim = "H",
                    Count = 10,
                    Latitude = 10,
                    Longitude = 10,
                    Timestamp = DateTime.UtcNow
                }
            });
            Console.WriteLine("Voucher OTC: {0}", voucherResponse.OtcGen);
        }

    }
}
