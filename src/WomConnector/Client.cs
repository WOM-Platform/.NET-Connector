using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using RestSharp;

namespace WomPlatform.Connector {

    public class Client {

        private T LoadFromPem<T>(Stream input) where T : class {
            if(input is null) {
                throw new ArgumentException(nameof(input));
            }
            if(!input.CanRead) {
                throw new ArgumentException(nameof(input));
            }

            using(var sr = new StreamReader(input)) {
                var reader = new PemReader(sr);
                return reader.ReadObject() as T;
            }
        }

        public Client(AsymmetricKeyParameter registryKey) {
            RegistryPublicKey = registryKey ?? throw new ArgumentNullException(nameof(registryKey));
        }

        public Client(Stream registryKeyStream) {
            var key = LoadFromPem<AsymmetricKeyParameter>(registryKeyStream);
            RegistryPublicKey = key ?? throw new ArgumentException(nameof(registryKeyStream));
        }

        public AsymmetricKeyParameter RegistryPublicKey { get; private set; }

        public bool TestMode { get; set; } = false;

        private RestClient _client = null;
        internal protected RestClient RestClient {
            get {
                if(_client is null) {
                    _client = new RestClient(string.Format("http://{0}wom.social/api/v1",
                        TestMode ? "dev." : string.Empty
                    ));
                }
                return _client;
            }
        }

        public Instrument CreateInstrument(long id, AsymmetricKeyParameter instrumentPrivateKey) {
            if(instrumentPrivateKey is null) {
                throw new ArgumentNullException(nameof(instrumentPrivateKey));
            }
            if(!instrumentPrivateKey.IsPrivate) {
                throw new ArgumentException("Key must be private", nameof(instrumentPrivateKey));
            }

            return new Instrument(this, id, instrumentPrivateKey);
        }

        public Instrument CreateInstrument(long id, Stream instrumentPrivateKeyStream) {
            var pair = LoadFromPem<AsymmetricCipherKeyPair>(instrumentPrivateKeyStream);
        }

    }

}
