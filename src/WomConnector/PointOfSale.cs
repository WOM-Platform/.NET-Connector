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

        /// <summary>
        /// Retrieves the POS identifier.
        /// </summary>
        public Identifier Identifier {
            get {
                return _id;
            }
        }

        /// <summary>
        /// Create a new payment request.
        /// </summary>
        /// <param name="amount">Amount of vouchers that are required to confirm the payment.</param>
        /// <param name="pocketAckUrl">Confirmation URL invoked by the Pocket.</param>
        /// <param name="posAckUrl">Confirmation URL used by the Registry.</param>
        /// <param name="filter">Filter used to determine which vouchers satisfy the payment.</param>
        /// <param name="isPersistent"></param>
        /// <param name="nonce">Unique nonce of the payment request, is auto-generated if null.</param>
        /// <param name="password">User password required to confirm the payment, is auto-generated if null.</param>
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

            _client.Logger.LogDebug(LoggingEvents.PointOfSale,
                "Performing payment creation request");

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
            var responseContent = _client.Crypto.Decrypt<PaymentRegisterResponse.Content>(response.Payload, _privateKey);

            _client.Logger.LogDebug(LoggingEvents.PointOfSale, "Performing payment verification request");

            await _client.PerformOperation("v1/payment/verify", new PaymentVerifyPayload {
                Payload = _client.Crypto.Encrypt(new PaymentVerifyPayload.Content {
                    Otc = responseContent.Otc
                }, await _client.GetRegistryPublicKey())
            });

            _client.Logger.LogDebug(LoggingEvents.PointOfSale, "Voucher creation succeeded");

            return new PaymentRequest(_client, responseContent.Otc, responseContent.Password);
        }

    }

}
