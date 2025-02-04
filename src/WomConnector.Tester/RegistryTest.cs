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
        public async Task TestAnonymousPosCredentials() {
            var creds = await _client.FetchAnonymousCredentials();

            Assert.That(creds.PosId, Is.EqualTo("5f69a60698e66631aaf79929"));
            Assert.That(creds.PosPrivateKey, Is.Not.Null);
            Assert.That(creds.PosPrivateKey.Length, Is.GreaterThanOrEqualTo(1679));
        }

        [Test]
        public async Task TestExchangeCredentials() {
            var creds = await _client.FetchExchangeCredentials();

            Assert.That(creds.SourceId, Is.EqualTo("64e5f0c93a5339481060a756"));
            Assert.That(creds.SourcePublicKey, Is.Not.Null);
            Assert.That(creds.SourcePublicKey.Length, Is.GreaterThanOrEqualTo(451));
        }

        [Test]
        public async Task TestAutoPublicFetch() {
            AsymmetricCipherKeyPair keys = null;
            using(var fs = new FileStream("keys/source1.pem", FileMode.Open)) {
                keys = KeyUtil.LoadCipherKeyPairFromPem(fs);
            }
            var instrument = _client.CreateInstrument("5e74203f5f21bb265a2d26bd", keys.Private);

            await instrument.RequestVouchers([
                new VoucherCreatePayload.VoucherInfo {
                    Aim = "C",
                    Count = 1,
                    Latitude = 10,
                    Longitude = -10,
                    Timestamp = DateTime.UtcNow
                }
            ]);

            Assert.That(await _client.GetRegistryPublicKey(), Is.Not.Null);
        }

        [Test]
        public async Task TestMerchantLogin() {
            var response = await _client.LoginAsMerchant("dummy@example.org", "NfBTFsXsYpCyJeV1yUcx");

            Assert.That(response.Email, Is.EqualTo("dummy@example.org"));
            Assert.That(response.Name, Is.EqualTo("Dummy"));
            Assert.That(response.Merchants.Count, Is.GreaterThanOrEqualTo(1));
            Assert.That(response.Merchants[0].Pos.Count, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public async Task TestSourceLogin() {
            var response = await _client.LoginAsSource("dummy@example.org", "NfBTFsXsYpCyJeV1yUcx");

            Assert.That(response.Sources.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task TestAimList() {
            var response = await _client.GetAims();

            Console.WriteLine("{0} aims retrieved", response.Aims.Count);
            Assert.That(response.Aims.Count, Is.GreaterThan(0));
        }

    }

}
