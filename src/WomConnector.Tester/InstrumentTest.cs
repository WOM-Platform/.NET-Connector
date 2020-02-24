using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Org.BouncyCastle.Crypto;
using WomPlatform.Connector;
using WomPlatform.Connector.Models;

namespace WomConnector.Tester {

    public class InstrumentTest {

        public static async Task<(Guid otc, string password)> GenerateVoucherRequestSource1(int count) {
            var instrument = Util.CreateInstrument(1, "keys/source1.pem");

            var (otc, password) = await instrument.RequestVouchers(new VoucherCreatePayload.VoucherInfo[] {
                new VoucherCreatePayload.VoucherInfo {
                    Aim = "E",
                    Count = 1,
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
            var instrument = Util.CreateInstrument(1, "keys/pos1.pem");

            Assert.ThrowsAsync<InvalidCipherTextException>(async () => {
                var (otc, password) = await instrument.RequestVouchers(new VoucherCreatePayload.VoucherInfo[] {
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
            var instrument = Util.CreateInstrument(2, "keys/source1.pem");

            Assert.ThrowsAsync<InvalidCipherTextException>(async () => {
                var (otc, password) = await instrument.RequestVouchers(new VoucherCreatePayload.VoucherInfo[] {
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
