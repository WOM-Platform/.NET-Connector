﻿using System;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using RestSharp;

namespace WomPlatform.Connector {

    public class Client {

        /// <summary>
        /// Create common JSON serializer settings.
        /// </summary>
        public static JsonSerializerSettings JsonSettings {
            get {
                var ret = new JsonSerializerSettings {
                    ContractResolver = new DefaultContractResolver {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    },
                    Culture = CultureInfo.InvariantCulture,
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                    DateParseHandling = DateParseHandling.DateTime,
                    Formatting = Formatting.None,
                    NullValueHandling = NullValueHandling.Ignore
                };
                return ret;
            }
        }

        private static JsonSerializerSettings _jsonSettings = null;
        internal static JsonSerializerSettings JsonSettingsCache {
            get {
                if(_jsonSettings == null) {
                    _jsonSettings = JsonSettings;
                }
                return _jsonSettings;
            }
        }

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

        /// <summary>
        /// Default public WOM domain (wom.social).
        /// </summary>
        public const string DefaultWomDomain = "wom.social";

        /// <summary>
        /// Gets the client's WOM domain.
        /// </summary>
        public string WomDomain { get; }

        private Client(string womDomain, ILoggerFactory loggerFactory) {
            WomDomain = womDomain;
            Logger = loggerFactory.CreateLogger<Client>();
            Crypto = new CryptoProvider(loggerFactory);
        }

        /// <summary>
        /// Creates a new WOM client with a given public Registry key.
        /// </summary>
        public Client(ILoggerFactory loggerFactory, AsymmetricKeyParameter registryKey)
            : this(DefaultWomDomain, loggerFactory) {

            RegistryPublicKey = registryKey ?? throw new ArgumentNullException(nameof(registryKey));
        }

        /// <summary>
        /// Creates a new WOM client with a stream to a public Registry key.
        /// </summary>
        public Client(ILoggerFactory loggerFactory, Stream registryKeyStream)
            : this(DefaultWomDomain, loggerFactory) {

            var key = LoadFromPem<AsymmetricKeyParameter>(registryKeyStream);
            RegistryPublicKey = key ?? throw new ArgumentException(nameof(registryKeyStream));
        }

        /// <summary>
        /// Creates a new WOM client with a given public Registry key.
        /// </summary>
        public Client(string womDomain, ILoggerFactory loggerFactory, AsymmetricKeyParameter registryKey)
            : this(womDomain, loggerFactory) {

            RegistryPublicKey = registryKey ?? throw new ArgumentNullException(nameof(registryKey));
        }

        /// <summary>
        /// Creates a new WOM client with a stream to a public Registry key.
        /// </summary>
        public Client(string womDomain, ILoggerFactory loggerFactory, Stream registryKeyStream)
            : this(womDomain, loggerFactory) {

            var key = LoadFromPem<AsymmetricKeyParameter>(registryKeyStream);
            RegistryPublicKey = key ?? throw new ArgumentException(nameof(registryKeyStream));
        }

        /// <summary>
        /// The Registry's public key.
        /// </summary>
        public AsymmetricKeyParameter RegistryPublicKey { get; private set; }

        internal protected ILogger<Client> Logger { get; }

        public CryptoProvider Crypto { get; private set; }

        private RestClient _client = null;
        internal protected RestClient RestClient {
            get {
                if(_client is null) {
                    Logger.LogDebug(LoggingEvents.Client, "Creating new REST client for WOM domain {0}", WomDomain);

                    _client = new RestClient(string.Format("http://{0}/api/v1",
                        WomDomain
                    ));
                    _client.UseSerializer(() => new JsonRestSerializer());
                }
                return _client;
            }
        }

        /// <summary>
        /// Creates a new <see cref="Instrument"/> instance.
        /// </summary>
        /// <param name="id">Unique ID of the instrument.</param>
        /// <param name="instrumentPrivateKey">Private key instance.</param>
        [Obsolete]
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
        [Obsolete]
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
        /// Creates a new <see cref="Instrument"/> instance.
        /// </summary>
        /// <param name="id">Unique ID of the instrument.</param>
        /// <param name="instrumentPrivateKey">Private key instance.</param>
        public Instrument CreateInstrument(string id, AsymmetricKeyParameter instrumentPrivateKey) {
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
        public Instrument CreateInstrument(string id, Stream instrumentPrivateKeyStream) {
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
        [Obsolete]
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
        [Obsolete]
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

        /// <summary>
        /// Creates a new <see cref="PointOfSale"/> instance.
        /// </summary>
        /// <param name="id">Unique ID of the POS.</param>
        /// <param name="instrumentPrivateKey">Private key instance.</param>
        public PointOfSale CreatePos(string id, AsymmetricKeyParameter instrumentPrivateKey) {
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
        public PointOfSale CreatePos(string id, Stream posPrivateyKeyStream) {
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
