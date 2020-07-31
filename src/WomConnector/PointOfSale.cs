using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Crypto;
using WomPlatform.Connector.Models;

namespace WomPlatform.Connector {

    public class PointOfSale {

        private readonly Client _client;
        private readonly Identifier _id;
        private readonly AsymmetricKeyParameter _privateKey;

        internal PointOfSale(Client c, Identifier id, AsymmetricKeyParameter privateKey) {
            _client = c;
            _id = id;
            _privateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
            if(!_privateKey.IsPrivate) {
                throw new ArgumentException("Key must be private", nameof(privateKey));
            }
        }

        public async Task<PaymentRequest> RequestPayment(int amount,
            string pocketAckUrl, string posAckUrl = null,
            SimpleFilter filter = null, bool isPersistent = false,
            string nonce = null, string password = null) {

            if(amount <= 0) {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }
            if(string.IsNullOrWhiteSpace(pocketAckUrl)) {
                throw new ArgumentNullException(nameof(pocketAckUrl));
            }

            _client.Logger.LogInformation(LoggingEvents.PointOfSale,
                "Creating payment request for {0} vouchers", amount);

            var effectiveNonce = nonce ?? Guid.NewGuid().ToString("N");

            var response = await _client.PerformOperation<PaymentRegisterResponse>("v1/payment/register", new PaymentRegisterPayload {
                PosId = _id,
                Nonce = effectiveNonce,
                Payload = _client.Crypto.Encrypt(new PaymentRegisterPayload.Content {
                    PosId = _id,
                    Nonce = effectiveNonce,
                    Password = password,
                    Amount = amount,
                    SimpleFilter = filter,
                    PocketAckUrl = pocketAckUrl,
                    PosAckUrl = posAckUrl
                }, await _client.GetRegistryPublicKey())
            });

            _client.Logger.LogDebug(LoggingEvents.Instrument,
                "Performing payment creation request");

            var responseContent = _client.Crypto.Decrypt<PaymentRegisterResponse.Content>(response.Payload, _privateKey);

            await _client.PerformOperation("v1/payment/verify", new PaymentVerifyPayload {
                Payload = _client.Crypto.Encrypt(new PaymentVerifyPayload.Content {
                    Otc = responseContent.Otc
                }, await _client.GetRegistryPublicKey())
            });

            _client.Logger.LogDebug(LoggingEvents.Instrument,
                "Performing payment verification request");

            _client.Logger.LogDebug(LoggingEvents.Instrument,
                "Voucher creation succeeded");

            return new PaymentRequest(_client, responseContent.Otc, responseContent.Password);
        }

    }

}
