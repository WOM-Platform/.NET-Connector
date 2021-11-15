using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WomPlatform.Connector.Models;

namespace WomPlatform.Connector {

    public class Pocket {

        private class VoucherEqualityComparer : IEqualityComparer<Voucher> {

            public bool Equals(Voucher x, Voucher y) {
                return x.Id == y.Id;
            }

            public int GetHashCode(Voucher obj) {
                return obj.Id.GetHashCode();
            }

        }

        private readonly Client _client;
        private readonly HashSet<Voucher> _vouchers;

        internal Pocket(Client c) {
            _client = c;
            _vouchers = new HashSet<Voucher>(new VoucherEqualityComparer());
        }

        public int VoucherCount => _vouchers.Count;

        /// <summary>
        /// Access read-only copy of vouchers.
        /// </summary>
        public IReadOnlyList<Voucher> Vouchers {
            get {
                return _vouchers.ToArray();
            }
        }

        /// <summary>
        /// Adds existing vouchers to the pocket.
        /// </summary>
        /// <param name="vouchers">Enumeration of existing vouchers.</param>
        /// <returns>True if all vouchers were added, false if a collision was detected.</returns>
        public bool AddVouchers(IEnumerable<Voucher> vouchers) {
            bool ok = true;
            foreach(var v in vouchers) {
                if(!_vouchers.Add(v)) {
                    ok = false;
                }
            }

            return ok;
        }

        /// <summary>
        /// Collect vouchers from a given voucher generation request.
        /// </summary>
        /// <param name="otc">One-time code of the generation request.</param>
        /// <param name="password">Password of the generation request.</param>
        public async Task CollectVouchers(Guid otc, string password) {
            _client.Logger.LogInformation(LoggingEvents.Pocket,
                "Acquiring vouchers from request {0}", otc);

            var sessionKey = _client.Crypto.GenerateSessionKey();

            _client.Logger.LogDebug(LoggingEvents.Pocket,
                "Performing voucher redeem request");

            var response = await _client.PerformOperation<VoucherRedeemResponse>("v1/voucher/redeem", new VoucherRedeemPayload {
                Payload = _client.Crypto.Encrypt(new VoucherRedeemPayload.Content {
                    Otc = otc,
                    Password = password,
                    SessionKey = sessionKey.ToBase64()
                }, await _client.GetRegistryPublicKey())
            });
            var responseContent = _client.Crypto.Decrypt<VoucherRedeemResponse.Content>(response.Payload, sessionKey);

            _client.Logger.LogDebug(LoggingEvents.Pocket,
                "Redeemed {0} vouchers from source #{2} {3}",
                responseContent.Vouchers.Length, responseContent.SourceId, responseContent.SourceName);

            foreach(var v in responseContent.Vouchers) {
                _vouchers.Add(new Voucher(v.Id, v.Secret, v.Aim, v.Latitude, v.Longitude, v.Timestamp));
                _client.Logger.LogTrace(LoggingEvents.Pocket,
                    "Collected voucher #{0} for aim {1}",
                    v.Id.ToString(), v.Aim);
            }
        }

        public async Task<string> PayWithRandomVouchers(Guid otc, string password) {
            _client.Logger.LogInformation(LoggingEvents.Pocket,
                "Getting information about payment {0}", otc);

            var sessionKey = _client.Crypto.GenerateSessionKey();

            _client.Logger.LogDebug(LoggingEvents.Pocket,
                "Performing payment information request");

            var response = await _client.PerformOperation<PaymentInfoResponse>("v1/payment/info", new PaymentInfoPayload {
                Payload = _client.Crypto.Encrypt(new PaymentInfoPayload.Content {
                    Otc = otc,
                    Password = password,
                    SessionKey = sessionKey.ToBase64()
                }, await _client.GetRegistryPublicKey())
            });
            var paymentInformation = _client.Crypto.Decrypt<PaymentInfoResponse.Content>(response.Payload, sessionKey);

            var satisfyingVouchers = _vouchers.Where(v => {
                if(paymentInformation.SimpleFilter == null) {
                    return true;
                }

                if(paymentInformation.SimpleFilter.Aim != null && !v.Aim.StartsWith(paymentInformation.SimpleFilter.Aim)) {
                    // Voucher does not match aim filter
                    return false;
                }

                if(paymentInformation.SimpleFilter.Bounds != null && !paymentInformation.SimpleFilter.Bounds.Contains(v.Latitude, v.Longitude)) {
                    // Voucher not contained in geographical bounds
                    return false;
                }

                if(paymentInformation.SimpleFilter.MaxAge != null && DateTime.UtcNow.Subtract(v.Timestamp) > TimeSpan.FromDays(paymentInformation.SimpleFilter.MaxAge.Value)) {
                    // Voucher too old
                    return false;
                }

                return true;
            }).ToList();

            if(satisfyingVouchers.Count < paymentInformation.Amount) {
                _client.Logger.LogError(LoggingEvents.Pocket,
                    "Cannot perform payment, payment requested {0} vouchers and only {1} satisfy filter",
                    paymentInformation.Amount, satisfyingVouchers.Count);
                throw new InvalidOperationException("Too few vouchers satisfying payment constraints");
            }

            // TODO: pick random vouchers?
            var paymentVouchers = satisfyingVouchers.Take(paymentInformation.Amount);

            string ackUrl = await Pay(otc, password, paymentVouchers);

            foreach(var v in paymentVouchers) {
                _vouchers.Remove(v);
            }

            return ackUrl;
        }

        /// <summary>
        /// Performs a payment with a given sequence of vouchers.
        /// </summary>
        /// <remarks>
        /// Vouchers must not necessarily be contained in the Pocket instance.
        /// Vouchers are not removed nor marked as spent after confirmation.
        /// </remarks>
        public async Task<string> Pay(Guid otc, string password, IEnumerable<Voucher> vouchers) {
            _client.Logger.LogInformation(LoggingEvents.Pocket,
                "Performing payment {0}", otc);

            var sessionKey = _client.Crypto.GenerateSessionKey();

            _client.Logger.LogDebug(LoggingEvents.Pocket,
                "Performing payment confirmation request");

            var response = await _client.PerformOperation<PaymentConfirmResponse>("v1/payment/confirm", new PaymentConfirmPayload {
                Payload = _client.Crypto.Encrypt(new PaymentConfirmPayload.Content {
                    Otc = otc,
                    Password = password,
                    SessionKey = sessionKey.ToBase64(),
                    Vouchers = (from v in vouchers
                                select new PaymentConfirmPayload.VoucherInfo {
                                    Id = v.Id,
                                    Secret = v.Secret
                                }).ToArray()
                }, await _client.GetRegistryPublicKey())
            });
            var paymentConfirmation = _client.Crypto.Decrypt<PaymentConfirmResponse.Content>(response.Payload, sessionKey);

            return paymentConfirmation.AckUrl;
        }

    }

}
