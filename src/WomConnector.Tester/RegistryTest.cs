using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Org.BouncyCastle.Crypto;
using WomPlatform.Connector;
using WomPlatform.Connector.Models;

namespace WomConnector.Tester {

    public class RegistryTest {

        Client _client;

        [SetUp]
        public void Setup() {
            _client = new Client("dev.wom.social", new LoggerFactory());
        }

        [Test]
        public async Task TestRegistryPublicKey() {
            var response = await _client.GetRegistryPublicKey();

            Console.WriteLine("Registry public key: {0}", response);
        }

        [Test]
        public async Task TestAutoPublicFetch() {
            AsymmetricCipherKeyPair keys = null;
            using(var fs = new FileStream("keys/source1.pem", FileMode.Open)) {
                keys = KeyUtil.LoadCipherKeyPairFromPem(fs);
            }
            var instrument = _client.CreateInstrument("5e74203f5f21bb265a2d26bd", keys.Private);

            await instrument.RequestVouchers(new VoucherCreatePayload.VoucherInfo[] {
                new VoucherCreatePayload.VoucherInfo {
                    Aim = "C",
                    Count = 1,
                    Latitude = 10,
                    Longitude = -10,
                    Timestamp = DateTime.UtcNow
                }
            });

            Assert.IsNotNull(await _client.GetRegistryPublicKey());
        }

        [Test]
        public async Task TestMerchantLogin() {
            var response = await _client.LoginAsMerchant("dummy@example.org", "password");

            Assert.AreEqual("dummy@example.org", response.Email);
            Assert.AreEqual("Dummy", response.Name);
            Assert.AreEqual(0, response.Merchants.Count);
        }

        [Test]
        public async Task TestSourceLogin() {
            var response = await _client.LoginAsMerchant("dummy@example.org", "password");

            Assert.AreEqual("dummy@example.org", response.Email);
            Assert.AreEqual("Dummy", response.Name);
            Assert.AreEqual(0, response.Merchants.Count);
        }

        [Test]
        public async Task TestAimList() {
            var response = await _client.GetAims();

            Console.WriteLine("{0} aims retrieved", response.Aims.Count);
            Assert.Greater(response.Aims.Count, 0);
        }

    }

}
