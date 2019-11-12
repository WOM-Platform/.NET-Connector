using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WomPlatform.Connector.Models;

namespace WomPlatform.Connector {

    public class Pocket {

        private readonly Client _client;
        private readonly List<Voucher> _vouchers;

        internal Pocket(Client c) {
            _client = c;
            _vouchers = new List<Voucher>();
        }

        public int VoucherCount => _vouchers.Count;

        /// <summary>
        /// Collect vouchers from a given voucher generation request.
        /// </summary>
        /// <param name="otc">One-time code of the generation request.</param>
        /// <param name="password">Password of the generation request.</param>
        public async Task CollectVouchers(Guid otc, string password) {
            _client.Logger.LogInformation(LoggingEvents.Pocket,
                "Acquiring vouchers from request {0}", otc);

            var sessionKey = _client.Crypto.GenerateSessionKey();

            var request = _client.RestClient.CreateJsonPostRequest("voucher/redeem", new VoucherRedeemPayload {
                Payload = _client.Crypto.Encrypt(new VoucherRedeemPayload.Content {
                    Otc = otc,
                    Password = password,
                    SessionKey = sessionKey.ToBase64()
                }, _client.RegistryPublicKey)
            });

            _client.Logger.LogDebug(LoggingEvents.Pocket,
                "Performing voucher redeem request");

            var response = await _client.RestClient.PerformRequest<VoucherRedeemResponse>(request);
            var responseContent = _client.Crypto.Decrypt<VoucherRedeemResponse.Content>(response.Payload, sessionKey);

            _client.Logger.LogDebug(LoggingEvents.Pocket,
                "Redeemed {0} vouchers from source #{2} {3}",
                responseContent.Vouchers.Length, responseContent.SourceId, responseContent.SourceName);

            _vouchers.AddRange(from v in responseContent.Vouchers
                               select new Voucher {
                                   Id = v.Id,
                                   Secret = v.Secret,
                                   Aim = v.Aim,
                                   Latitude = v.Latitude,
                                   Longitude = v.Longitude,
                                   Timestamp = v.Timestamp
                               });
        }

    }

}
