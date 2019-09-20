using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Crypto;
using RestSharp;

namespace WomPlatform.Connector {

    public class Instrument {

        private readonly Client _client;
        private readonly long _id;
        private readonly AsymmetricKeyParameter _privateKey;

        internal Instrument(Client c, long id, AsymmetricKeyParameter privateKey) {
            _client = c;
            _id = id;
            _privateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
            if(!_privateKey.IsPrivate) {
                throw new ArgumentException("Key must be private", nameof(privateKey));
            }
        }



    }

}
