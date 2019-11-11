using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Org.BouncyCastle.Crypto;
using WomPlatform.Connector;
using WomPlatform.Connector.Models;

namespace WomConnector.Tester {

    public class InstrumentTest {

        private Client _client;
        private Instrument _instrument;

        [SetUp]
        public void Setup() {
            _client = Util.CreateClient();

            AsymmetricCipherKeyPair keys = null;
            using(var fs = new FileStream("keys/source1.pem", FileMode.Open)) {
                keys = KeyUtil.LoadCipherKeyPairFromPem(fs);
            }

            _instrument = _client.CreateInstrument(1, keys.Private);
        }

        [Test]
        public async Task CreateSimpleVoucher() {
            var (otc, password) = await _instrument.RequestVouchers(new VoucherCreatePayload.VoucherInfo[] {
                new VoucherCreatePayload.VoucherInfo {
                    Aim = "E",
                    Count = 1,
                    Latitude = 43.72621,
                    Longitude = 12.63633,
                    Timestamp = DateTime.UtcNow
                }
            });

            Console.WriteLine("Voucher {0} pwd {1}", otc, password);
        }

    }

}
