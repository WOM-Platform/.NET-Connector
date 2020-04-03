using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Crypto;
using WomPlatform.Connector.Models;

namespace WomPlatform.Connector {

    public class Instrument {

        private readonly Client _client;
        private readonly Identifier _id;
        private readonly AsymmetricKeyParameter _privateKey;

        internal Instrument(Client c, Identifier id, AsymmetricKeyParameter privateKey) {
            _client = c;
            _id = id;
            _privateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
            if(!_privateKey.IsPrivate) {
                throw new ArgumentException("Key must be private", nameof(privateKey));
            }
        }

        public async Task<VoucherRequest> RequestVouchers(VoucherCreatePayload.VoucherInfo[] vouchers,
            string nonce = null, string password = null) {

            if(vouchers is null || vouchers.Length == 0) {
                throw new ArgumentNullException(nameof(vouchers));
            }

            _client.Logger.LogInformation(LoggingEvents.Instrument,
                "Requesting vouchers with {0} specification templates", vouchers.Length);

            var effectiveNonce = nonce ?? Guid.NewGuid().ToString("N");

            var request = _client.CreateJsonPostRequest("voucher/create", new VoucherCreatePayload {
                SourceId = _id,
                Nonce = effectiveNonce,
                Payload = _client.Crypto.Encrypt(new VoucherCreatePayload.Content {
                    SourceId = _id,
                    Nonce = effectiveNonce,
                    Password = password,
                    Vouchers = vouchers
                }, _client.RegistryPublicKey)
            });

            _client.Logger.LogDebug(LoggingEvents.Instrument,
                "Performing voucher creation request");

            var response = await _client.PerformRequest<VoucherCreateResponse>(request);
            var responseContent = _client.Crypto.Decrypt<VoucherCreateResponse.Content>(response.Payload, _privateKey);

            request = _client.CreateJsonPostRequest("voucher/verify", new VoucherVerifyPayload {
                Payload = _client.Crypto.Encrypt(new VoucherVerifyPayload.Content {
                    Otc = responseContent.Otc
                }, _client.RegistryPublicKey)
            });

            _client.Logger.LogDebug(LoggingEvents.Instrument,
                "Performing voucher verification request");

            await _client.RestClient.PerformRequest(request);

            _client.Logger.LogDebug(LoggingEvents.Instrument,
                "Voucher creation succeeded");

            return new VoucherRequest(_client, responseContent.Otc, responseContent.Password);
        }

    }

}
