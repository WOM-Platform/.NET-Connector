using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Org.BouncyCastle.Crypto;
using WomPlatform.Connector;
using WomPlatform.Connector.Models;

namespace WomConnector.Tester {

    public class InstrumentTest {

        private Instrument _instrument;

        [SetUp]
        public void Setup() {
            _instrument = Util.GenerateInstrument();
        }

        public async Task<(Guid otc, string password)> GenerateVoucherRequestSource1(int count) {
            var (otc, password) = await _instrument.RequestVouchers(new VoucherCreatePayload.VoucherInfo[] {
                new VoucherCreatePayload.VoucherInfo {
                    Aim = "E",
                    Count = count,
                    Latitude = 43.72621,
                    Longitude = 12.63633,
                    Timestamp = DateTime.UtcNow
                }
            });

            return (otc, password);
        }

        [Test]
        public async Task CreateSimpleVoucher() {
            var (otc, password) = await GenerateVoucherRequestSource1(1);

            Console.WriteLine("Voucher {0} pwd {1}", otc, password);
        }

        [Test]
        public void FailSourceKey() {
            var wrongInstrument = Util.CreateInstrument("5e74203f5f21bb265a2d26bd", "keys/pos1.pem");

            Assert.ThrowsAsync<InvalidCipherTextException>(async () => {
                var (otc, password) = await wrongInstrument.RequestVouchers(new VoucherCreatePayload.VoucherInfo[] {
                new VoucherCreatePayload.VoucherInfo {
                    Aim = "E",
                    Count = 1,
                    Latitude = 43.72621,
                    Longitude = 12.63633,
                    Timestamp = DateTime.UtcNow
                }
            });

                Console.WriteLine("Voucher {0} pwd {1}", otc, password);
            });
        }

        [Test]
        public void FailSourceId() {
            var wrongInstrument = Util.CreateInstrument("5e74203f5f21bb265a2d27bd", "keys/source1.pem");

            Assert.ThrowsAsync<InvalidOperationException>(async () => {
                var (otc, password) = await wrongInstrument.RequestVouchers(new VoucherCreatePayload.VoucherInfo[] {
                new VoucherCreatePayload.VoucherInfo {
                    Aim = "E",
                    Count = 1,
                    Latitude = 43.72621,
                    Longitude = 12.63633,
                    Timestamp = DateTime.UtcNow
                }
            });

                Console.WriteLine("Voucher {0} pwd {1}", otc, password);
            });
        }

    }

}
