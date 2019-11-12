using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
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

            // Do not dispose reader in order not to dispose input stream (handled externally)
            var sr = new StreamReader(input);
            var reader = new PemReader(sr);
            return reader.ReadObject() as T;
        }

        private Client(ILoggerFactory loggerFactory) {
            Logger = loggerFactory.CreateLogger<Client>();
            Crypto = new CryptoProvider(loggerFactory);
        }

        public Client(ILoggerFactory loggerFactory, AsymmetricKeyParameter registryKey)
            : this(loggerFactory) {

            RegistryPublicKey = registryKey ?? throw new ArgumentNullException(nameof(registryKey));
        }

        public Client(ILoggerFactory loggerFactory, Stream registryKeyStream)
            : this(loggerFactory) {

            var key = LoadFromPem<AsymmetricKeyParameter>(registryKeyStream);
            RegistryPublicKey = key ?? throw new ArgumentException(nameof(registryKeyStream));
        }

        public AsymmetricKeyParameter RegistryPublicKey { get; private set; }

        internal protected ILogger<Client> Logger { get; }

        public CryptoProvider Crypto { get; private set; }

        /// <summary>
        /// Gets or sets whether to use a development or production instance of the Registry.
        /// </summary>
        public bool TestMode { get; set; } = false;

        private RestClient _client = null;
        internal protected RestClient RestClient {
            get {
                if(_client is null) {
                    Logger.LogDebug(LoggingEvents.Client, "Creating new REST client in {0} mode", TestMode ? "test" : "production");

                    _client = new RestClient(string.Format("http://{0}wom.social/api/v1",
                        TestMode ? "dev." : string.Empty
                    ));
                }
                return _client;
            }
        }

        /// <summary>
        /// Creates a new <see cref="Instrument"/> instance.
        /// </summary>
        /// <param name="id">Unique ID of the instrument.</param>
        /// <param name="instrumentPrivateKey">Private key instance.</param>
        public Instrument CreateInstrument(long id, AsymmetricKeyParameter instrumentPrivateKey) {
            if(instrumentPrivateKey is null) {
                throw new ArgumentNullException(nameof(instrumentPrivateKey));
            }
            if(!instrumentPrivateKey.IsPrivate) {
                throw new ArgumentException("Key must be private", nameof(instrumentPrivateKey));
            }

            return new Instrument(this, id, instrumentPrivateKey);
        }

        /// <summary>
        /// Creates a new <see cref="Instrument"/> instance.
        /// </summary>
        /// <param name="id">Unique ID of the instrument.</param>
        /// <param name="instrumentPrivateKeyStream">Stream of the instrument's private key.</param>
        public Instrument CreateInstrument(long id, Stream instrumentPrivateKeyStream) {
            var pair = LoadFromPem<AsymmetricCipherKeyPair>(instrumentPrivateKeyStream);
            if(pair is null) {
                throw new ArgumentException("Invalid key stream", nameof(instrumentPrivateKeyStream));
            }
            if(pair.Private is null) {
                throw new ArgumentException("No private key", nameof(instrumentPrivateKeyStream));
            }

            return new Instrument(this, id, pair.Private);
        }

        /// <summary>
        /// Creates a new <see cref="Pocket"/> instance.
        /// </summary>
        public Pocket CreatePocket() {
            return new Pocket(this);
        }

        /// <summary>
        /// Creates a new <see cref="PointOfSale"/> instance.
        /// </summary>
        /// <param name="id">Unique ID of the POS.</param>
        /// <param name="instrumentPrivateKey">Private key instance.</param>
        public PointOfSale CreatePos(long id, AsymmetricKeyParameter instrumentPrivateKey) {
            if(instrumentPrivateKey is null) {
                throw new ArgumentNullException(nameof(instrumentPrivateKey));
            }
            if(!instrumentPrivateKey.IsPrivate) {
                throw new ArgumentException("Key must be private", nameof(instrumentPrivateKey));
            }

            return new PointOfSale(this, id, instrumentPrivateKey);
        }

        /// <summary>
        /// Creates a new <see cref="PointOfSale"/> instance.
        /// </summary>
        /// <param name="id">Unique ID of the POS.</param>
        /// <param name="instrumentPrivateKeyStream">Stream of the POS private key.</param>
        public PointOfSale CreatePos(long id, Stream posPrivateyKeyStream) {
            var pair = LoadFromPem<AsymmetricCipherKeyPair>(posPrivateyKeyStream);
            if(pair is null) {
                throw new ArgumentException("Invalid key stream", nameof(posPrivateyKeyStream));
            }
            if(pair.Private is null) {
                throw new ArgumentException("No private key", nameof(posPrivateyKeyStream));
            }

            return new PointOfSale(this, id, pair.Private);
        }

    }

}
