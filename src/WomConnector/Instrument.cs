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
        private readonly string _apiKey;

        internal Instrument(Client c, Identifier id, AsymmetricKeyParameter privateKey, string apiKey = null) {
            _client = c;
            _id = id;
            _privateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
            if(!_privateKey.IsPrivate) {
                throw new ArgumentException("Key must be private", nameof(privateKey));
            }
            _apiKey = apiKey;
        }

        /// <summary>
        /// Retrieves the instrument's identifier.
        /// </summary>
        public Identifier Identifier {
            get {
                return _id;
            }
        }

        public Task<VoucherRequest> RequestVouchers(VoucherCreatePayload.VoucherInfo vouchers,
            string nonce = null, string password = null) {

            return RequestVouchers(new VoucherCreatePayload.VoucherInfo[] {
                vouchers
            }, nonce, password);
        }

        public async Task<VoucherRequest> RequestVouchers(VoucherCreatePayload.VoucherInfo[] vouchers,
            string nonce = null, string password = null) {

            if(vouchers is null || vouchers.Length == 0) {
                throw new ArgumentNullException(nameof(vouchers));
            }

            _client.Logger.LogInformation(LoggingEvents.Instrument,
                "Requesting vouchers with {0} specification templates", vouchers.Length);

            var effectiveNonce = nonce ?? Guid.NewGuid().ToString("N");

            _client.Logger.LogDebug(LoggingEvents.Instrument,
                "Performing voucher creation request");

            var response = await _client.PerformOperation<VoucherCreateResponse>("v1/voucher/create", new VoucherCreatePayload {
                SourceId = _id,
                Nonce = effectiveNonce,
                Payload = _client.Crypto.Encrypt(new VoucherCreatePayload.Content {
                    SourceId = _id,
                    Nonce = effectiveNonce,
                    Password = password,
                    Vouchers = vouchers
                }, await _client.GetRegistryPublicKey())
            }, _apiKey);
            var responseContent = _client.Crypto.Decrypt<VoucherCreateResponse.Content>(response.Payload, _privateKey);

            _client.Logger.LogDebug(LoggingEvents.Instrument,
                "Performing voucher verification request");

            await _client.PerformOperation<VoucherVerifyPayload>("v1/voucher/verify", new VoucherVerifyPayload {
                Payload = _client.Crypto.Encrypt(new VoucherVerifyPayload.Content {
                    Otc = responseContent.Otc
                }, await _client.GetRegistryPublicKey())
            });

            _client.Logger.LogDebug(LoggingEvents.Instrument,
                "Voucher creation succeeded");

            return new VoucherRequest(_client, responseContent.Otc, responseContent.Password);
        }
    }

}
