﻿using System;
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

    }

}
