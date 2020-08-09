using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using RestSharp;
using WomPlatform.Connector.Models;

namespace WomPlatform.Connector {

    /// <summary>
    /// WOM client.
    /// </summary>
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

        private T LoadFromPem<T>(string pem) where T : class {
            using(StringReader sr = new StringReader(pem)) {
                var reader = new PemReader(sr);
                return reader.ReadObject() as T;
            }
        }

        /// <summary>
        /// Default public WOM domain (wom.social).
        /// </summary>
        public const string DefaultWomDomain = "wom.social";

        /// <summary>
        /// Gets the client's WOM domain.
        /// </summary>
        public string WomDomain { get; }

        /// <summary>
        /// Creates a new WOM client on a given WOM domain.
        /// </summary>
        public Client(string womDomain, ILoggerFactory loggerFactory) {
            WomDomain = womDomain;
            Logger = loggerFactory.CreateLogger<Client>();
            Crypto = new CryptoProvider(loggerFactory);
            _registryPublicKey = null;
        }

        /// <summary>
        /// Creates a new WOM client on the default WOM domain (wom.social).
        /// </summary>
        public Client(ILoggerFactory loggerFactory)
            : this(DefaultWomDomain, loggerFactory) {
        }

        /// <summary>
        /// Creates a new WOM client with a given public Registry key.
        /// </summary>
        public Client(ILoggerFactory loggerFactory, AsymmetricKeyParameter registryKey)
            : this(DefaultWomDomain, loggerFactory) {

            _registryPublicKey = registryKey ?? throw new ArgumentNullException(nameof(registryKey));
        }

        /// <summary>
        /// Creates a new WOM client with a stream to a public Registry key.
        /// </summary>
        public Client(ILoggerFactory loggerFactory, Stream registryKeyStream)
            : this(DefaultWomDomain, loggerFactory) {

            var key = LoadFromPem<AsymmetricKeyParameter>(registryKeyStream);
            _registryPublicKey = key ?? throw new ArgumentException(nameof(registryKeyStream));
        }

        /// <summary>
        /// Creates a new WOM client with a given public Registry key.
        /// </summary>
        public Client(string womDomain, ILoggerFactory loggerFactory, AsymmetricKeyParameter registryKey)
            : this(womDomain, loggerFactory) {

            _registryPublicKey = registryKey ?? throw new ArgumentNullException(nameof(registryKey));
        }

        /// <summary>
        /// Creates a new WOM client with a stream to a public Registry key.
        /// </summary>
        public Client(string womDomain, ILoggerFactory loggerFactory, Stream registryKeyStream)
            : this(womDomain, loggerFactory) {

            var key = LoadFromPem<AsymmetricKeyParameter>(registryKeyStream);
            _registryPublicKey = key ?? throw new ArgumentException(nameof(registryKeyStream));
        }

        private AsymmetricKeyParameter _registryPublicKey = null;

        /// <summary>
        /// Get the Registry's public key.
        /// </summary>
        public async Task<AsymmetricKeyParameter> GetRegistryPublicKey() {
            if(_registryPublicKey != null) {
                return _registryPublicKey;
            }

            var pemKey = await FetchRegistryPublicKey();
            _registryPublicKey = LoadFromPem<AsymmetricKeyParameter>(pemKey);
            return _registryPublicKey;
        }

        internal protected ILogger<Client> Logger { get; }

        /// <summary>
        /// Get the client's crypto provider.
        /// </summary>
        public CryptoProvider Crypto { get; private set; }

        /// <summary>
        /// REST client for HTTP requests.
        /// </summary>
        private RestClient _httpClient = null;
        protected RestClient HttpClient {
            get {
                if(_httpClient is null) {
                    Logger.LogDebug(LoggingEvents.Client, "Creating new HTTP client for WOM domain {0}", WomDomain);

                    _httpClient = new RestClient(string.Format("http://{0}/api",
                        WomDomain
                    ));
                    _httpClient.UseSerializer(() => new JsonRestSerializer());
                }
                return _httpClient;
            }
        }

        /// <summary>
        /// REST client for HTTPS requests.
        /// </summary>
        private RestClient _httpsClient = null;
        protected RestClient HttpsClient {
            get {
                if(_httpsClient is null) {
                    Logger.LogDebug(LoggingEvents.Client, "Creating new HTTPS client for WOM domain {0}", WomDomain);

                    _httpsClient = new RestClient(string.Format("https://{0}/api",
                        WomDomain
                    ));
                    _httpsClient.UseSerializer(() => new JsonRestSerializer());
                }
                return _httpsClient;
            }
        }

        internal protected async Task<IRestResponse> PerformOperation(string urlPath, object jsonBody) {
            var request = new RestRequest(urlPath, Method.POST) {
                RequestFormat = DataFormat.Json
            };
            request.AddHeader("Accept", "application/json");
            if(jsonBody != null) {
                request.AddJsonBody(jsonBody);
            }

            return await PerformRequest(HttpClient, request);
        }

        /// <summary>
        /// Performs a Registry operation, as a POST request with JSON body
        /// and expecting a JSON response.
        /// </summary>
        internal protected async Task<T> PerformOperation<T>(string urlPath, object jsonBody) {
            var response = await PerformOperation(urlPath, jsonBody);

            return JsonConvert.DeserializeObject<T>(response.Content);
        }

        /// <summary>
        /// Perform a simple client request and verifies that the response is #200.
        /// </summary>
        internal protected async Task<IRestResponse> PerformRequest(RestClient client, RestRequest request) {
            Logger.LogTrace(LoggingEvents.Communication,
                "HTTP request {0} {1}",
                request.Method.ToString(),
                client.BuildUri(request));

            if(request.Body != null) {
                Logger.LogTrace(LoggingEvents.Communication,
                    "Request body ({0}): {1}",
                    request.Body.ContentType,
                    request.Body.Value);
            }

            var response = await client.ExecuteAsync(request).ConfigureAwait(false);

            Logger.LogTrace(LoggingEvents.Communication,
                "HTTP response {0}",
                response.StatusCode);

            Logger.LogTrace(LoggingEvents.Communication,
                "Response body ({0}): {1}",
                response.ContentType,
                response.Content);

            if(response.StatusCode != System.Net.HttpStatusCode.OK) {
                throw new InvalidOperationException(string.Format("API status code {0}", response.StatusCode));
            }

            return response;
        }

        /// <summary>
        /// Loads the registry's public key from the server.
        /// </summary>
        public async Task<string> FetchRegistryPublicKey() {
            var response = await PerformRequest(HttpsClient, new RestRequest("v1/auth/key", Method.GET));
            return response.Content;
        }

        /// <summary>
        /// Login as a merchant and retrieve list of POS.
        /// </summary>
        public async Task<MerchantLoginResultV2> LoginAsMerchant(string username, string password) {
            var request = new RestRequest("v2/auth/merchant", Method.POST);
            request.AddHeader("Authorization",
                string.Format("Basic {0}", $"{username}:{password}".ToBase64())
            );
            var response = await PerformRequest(HttpsClient, request);

            return JsonConvert.DeserializeObject<MerchantLoginResultV2>(response.Content);
        }

        /// <summary>
        /// Login as an instrument and retrieve list of sources.
        /// </summary>
        public async Task<SourceLoginResultV1> LoginAsSource(string username, string password) {
            var request = new RestRequest("v1/auth/sources", Method.GET);
            request.AddHeader("Authorization",
                string.Format("Basic {0}", $"{username}:{password}".ToBase64())
            );
            var response = await PerformRequest(HttpsClient, request);

            return JsonConvert.DeserializeObject<SourceLoginResultV1>(response.Content);
        }

        /// <summary>
        /// Fetch list of aims.
        /// </summary>
        public async Task<AimListResponseV2> GetAims() {
            var request = new RestRequest("v2/aims", Method.GET);
            var response = await PerformRequest(HttpsClient, request);

            return JsonConvert.DeserializeObject<AimListResponseV2>(response.Content);
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
