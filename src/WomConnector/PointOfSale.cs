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
        private readonly long _id;
        private readonly AsymmetricKeyParameter _privateKey;

        internal PointOfSale(Client c, long id, AsymmetricKeyParameter privateKey) {
            _client = c;
            _id = id;
            _privateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
            if(!_privateKey.IsPrivate) {
                throw new ArgumentException("Key must be private", nameof(privateKey));
            }
        }

        public async Task<(Guid Otc, string Password)> RequestPayment(int amount,
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

            var request = _client.CreateJsonPostRequest("payment/register", new PaymentRegisterPayload {
                PosId = new Identifier(_id),
                Nonce = effectiveNonce,
                Payload = _client.Crypto.Encrypt(new PaymentRegisterPayload.Content {
                    PosId = new Identifier(_id),
                    Nonce = effectiveNonce,
                    Password = password,
                    Amount = amount,
                    SimpleFilter = filter,
                    PocketAckUrl = pocketAckUrl,
                    PosAckUrl = posAckUrl
                }, _client.RegistryPublicKey)
            });

            _client.Logger.LogDebug(LoggingEvents.Instrument,
                "Performing payment creation request");

            var response = await _client.PerformRequest<PaymentRegisterResponse>(request);
            var responseContent = _client.Crypto.Decrypt<PaymentRegisterResponse.Content>(response.Payload, _privateKey);

            request = _client.CreateJsonPostRequest("payment/verify", new PaymentVerifyPayload {
                Payload = _client.Crypto.Encrypt(new PaymentVerifyPayload.Content {
                    Otc = responseContent.Otc
                }, _client.RegistryPublicKey)
            });

            _client.Logger.LogDebug(LoggingEvents.Instrument,
                "Performing payment verification request");

            await _client.RestClient.PerformRequest(request);

            _client.Logger.LogDebug(LoggingEvents.Instrument,
                "Voucher creation succeeded");

            return (responseContent.Otc, responseContent.Password);
        }

    }

}
