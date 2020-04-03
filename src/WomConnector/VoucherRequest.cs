using System;

namespace WomPlatform.Connector {

    public class VoucherRequest {

        private readonly Client _client;

        internal VoucherRequest(Client client, Guid otcGen, string pwd) {
            _client = client;
            OtcGen = otcGen;
            Password = pwd;
        }

        public Guid OtcGen { get; }

        public string Password { get; }

        public string Link {
            get => $"https://{_client.WomDomain}/vouchers/{OtcGen:N}";
        }

    }

}
