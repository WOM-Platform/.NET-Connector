using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using WomPlatform.Connector.Models;

namespace WomConnector.Tester {

    public class PocketTest {

        [Test]
        public async Task CreateSimpleVoucher() {
            var (otc, password) = await InstrumentTest.GenerateVoucherRequestSource1(1);

            var pocket = Util.CreatePocket();
            await pocket.CollectVouchers(otc, password);

            Assert.AreEqual(1, pocket.VoucherCount);
        }

    }

}
