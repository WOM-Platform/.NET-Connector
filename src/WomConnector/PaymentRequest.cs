using System;

namespace WomPlatform.Connector {

    public class PaymentRequest {

        private readonly Client _client;

        internal PaymentRequest(Client client, Guid otcPay, string pwd) {
            _client = client;
            OtcPay = otcPay;
            Password = pwd;
        }

        public Guid OtcPay { get; }

        public string Password { get; }

        public string Link {
            get => $"https://{_client.WomDomain}/payment/{OtcPay:N}";
        }

    }

}
