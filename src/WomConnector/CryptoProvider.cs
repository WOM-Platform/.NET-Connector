using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Globalization;

namespace WomPlatform.Connector {

    /// <summary>
    /// Provides access to auxiliary high-level cryptographic functions.
    /// </summary>
    public class CryptoProvider {

        public Random Generator { get; } = new Random();

        public CryptoProvider(ILoggerFactory loggerFactory) {
            Logger = loggerFactory.CreateLogger<CryptoProvider>();
        }

        protected ILogger<CryptoProvider> Logger { get; }

        /// <summary>
        /// Decrypts a byte payload using a given key.
        /// </summary>
        public byte[] Decrypt(byte[] payload, AsymmetricKeyParameter key) {
            var engine = new Pkcs1Encoding(new RsaEngine());
            engine.Init(false, key);

            int inBlockSize = engine.GetInputBlockSize();
            int outBlockSize = engine.GetOutputBlockSize();
            int blocks = (int)Math.Ceiling(payload.Length / (double)inBlockSize);
            int outputLength = 0;
            byte[] output = new byte[blocks * outBlockSize];
            for (int i = 0; i < blocks; ++i) {
                int offset = i * inBlockSize;
                int blockLength = Math.Min(inBlockSize, payload.Length - offset);
                var cryptoBlock = engine.ProcessBlock(payload, offset, blockLength);
                cryptoBlock.CopyTo(output, i * outBlockSize);
                outputLength += cryptoBlock.Length;

                Logger.LogTrace(LoggingEvents.Cryptography,
                    "Decrypt {0}th block (offset {1} length {2}) to {3} bytes (offset {4})",
                    i + 1, offset, blockLength, cryptoBlock.Length, i * outBlockSize);
            }

            if (outputLength != output.Length) {
                // Rescale output array
                byte[] tmp = new byte[outputLength];
                Array.Copy(output, tmp, outputLength);
                output = tmp;
            }

            Logger.LogTrace(LoggingEvents.Cryptography,
                "Decrypted {0} bytes into {1} blocks of {3} bytes, output {4} bytes",
                payload.Length, blocks, outBlockSize, output.Length);

            return output;
        }

        /// <summary>
        /// Encrypts a byte payload using a given key.
        /// </summary>
        public byte[] Encrypt(byte[] payload, AsymmetricKeyParameter key) {
            var engine = new Pkcs1Encoding(new RsaEngine());
            engine.Init(true, key);

            int inBlockSize = engine.GetInputBlockSize();
            int outBlockSize = engine.GetOutputBlockSize();
            int blocks = (int)Math.Ceiling(payload.Length / (double)inBlockSize);
            int outputLength = 0;
            byte[] output = new byte[blocks * outBlockSize];
            for (int i = 0; i < blocks; ++i) {
                int offset = i * inBlockSize;
                int blockLength = Math.Min(inBlockSize, payload.Length - offset);
                var cryptoBlock = engine.ProcessBlock(payload, offset, blockLength);
                cryptoBlock.CopyTo(output, i * outBlockSize);
                outputLength += cryptoBlock.Length;

                Logger.LogTrace(LoggingEvents.Cryptography,
                    "Encrypt {0}th block (offset {1} length {2}) to {3} bytes (offset {4})",
                    i + 1, offset, blockLength, cryptoBlock.Length, i * outBlockSize);
            }

            if (outputLength != output.Length) {
                // Rescale output array
                byte[] tmp = new byte[outputLength];
                Array.Copy(output, tmp, outputLength);
                output = tmp;
            }

            Logger.LogTrace(LoggingEvents.Cryptography,
                "Encrypted {0} bytes into {1} blocks of {2} bytes, output {3} bytes",
                payload.Length, blocks, outBlockSize, output.Length);

            return output;
        }

        /// <summary>
        /// Decrypts a base64 payload and verifies it.
        /// If successful, the data is parsed as an UTF8 JSON string.
        /// </summary>
        /// <typeparam name="T">Type of the object to decrypt.</typeparam>
        public T DecryptAndVerify<T>(string payload,
            AsymmetricKeyParameter senderPublicKey, AsymmetricKeyParameter receiverPrivateKey) {
            if (senderPublicKey.IsPrivate) {
                throw new ArgumentException("Public key of sender required for verification", nameof(senderPublicKey));
            }
            if (!receiverPrivateKey.IsPrivate) {
                throw new ArgumentException("Private key of receiver required for decryption", nameof(receiverPrivateKey));
            }

            var payloadBytes = payload.FromBase64();
            var decryptBytes = Decrypt(payloadBytes, receiverPrivateKey);
            var verifyBytes = Decrypt(decryptBytes, senderPublicKey);

            Logger.LogTrace(LoggingEvents.Cryptography,
                "Decrypt and verify {3} chars (bytes {0} => {1} => {2})",
                payloadBytes.Length, decryptBytes.Length, verifyBytes.Length, payload.Length);

            return JsonConvert.DeserializeObject<T>(verifyBytes.AsUtf8String(), Client.JsonSettings);
        }

        /// <summary>
        /// Signs and encrypts an object payload.
        /// If successful, payload is encoded as a base64 string.
        /// </summary>
        /// <typeparam name="T">Type of the object to sign and encrypt.</typeparam>
        public string SignAndEncrypt<T>(T payload,
            AsymmetricKeyParameter senderPrivateKey, AsymmetricKeyParameter receiverPublicKey) {
            if (!senderPrivateKey.IsPrivate) {
                throw new ArgumentException("Private key of sender required for signing", nameof(senderPrivateKey));
            }
            if (receiverPublicKey.IsPrivate) {
                throw new ArgumentException("Public key of receiver required for encryption", nameof(receiverPublicKey));
            }

            var payloadBytes = JsonConvert.SerializeObject(payload, Client.JsonSettings).ToBytes();
            var signedBytes = Encrypt(payloadBytes, senderPrivateKey);
            var encryptedBytes = Encrypt(signedBytes, receiverPublicKey);

            Logger.LogTrace(LoggingEvents.Cryptography,
                "Sign and encrypt object (bytes {0} => {1} => {2})",
                payloadBytes.Length, signedBytes.Length, encryptedBytes.Length);

            return encryptedBytes.ToBase64();
        }

        /// <summary>
        /// Signs an object payload.
        /// If successful, payload is encoded as a base64 string.
        /// </summary>
        /// <typeparam name="T">Type of the object to sign.</typeparam>
        public string Sign<T>(T payload, AsymmetricKeyParameter senderPrivateKey) {
            if (!senderPrivateKey.IsPrivate) {
                throw new ArgumentException("Private key of sender required for signing", nameof(senderPrivateKey));
            }

            var payloadBytes = JsonConvert.SerializeObject(payload, Client.JsonSettings).ToBytes();
            var signedBytes = Encrypt(payloadBytes, senderPrivateKey);

            Logger.LogTrace(LoggingEvents.Cryptography,
                "Sign object (bytes {0} => {1})",
                payloadBytes.Length, signedBytes.Length);

            return signedBytes.ToBase64();
        }

        /// <summary>
        /// Verifies a base64 payload.
        /// If successful, the data is parsed as an UTF8 JSON string.
        /// </summary>
        /// <typeparam name="T">Type of the object to decrypt.</typeparam>
        public T Verify<T>(string payload, AsymmetricKeyParameter senderPublicKey) {
            if (senderPublicKey.IsPrivate) {
                throw new ArgumentException("Public key of sender required for verification", nameof(senderPublicKey));
            }

            var payloadBytes = payload.FromBase64();
            var verifyBytes = Decrypt(payloadBytes, senderPublicKey);

            Logger.LogTrace(LoggingEvents.Cryptography,
                "Verify {2} chars (bytes {0} => {1})",
                payloadBytes.Length, verifyBytes.Length, payload.Length);

            return JsonConvert.DeserializeObject<T>(verifyBytes.AsUtf8String(), Client.JsonSettings);
        }

        /// <summary>
        /// Encrypts an object payload.
        /// If successful, payload is encoded as a base64 string.
        /// </summary>
        /// <typeparam name="T">Type of the object to sign.</typeparam>
        public string Encrypt<T>(T payload, AsymmetricKeyParameter receiverPublicKey) {
            if (receiverPublicKey.IsPrivate) {
                throw new ArgumentException("Public key of receiver required for encryption", nameof(receiverPublicKey));
            }

            var payloadBytes = JsonConvert.SerializeObject(payload, Client.JsonSettings).ToBytes();
            var signedBytes = Encrypt(payloadBytes, receiverPublicKey);

            Logger.LogTrace(LoggingEvents.Cryptography,
                "Encrypt object (bytes {0} => {1})",
                payloadBytes.Length, signedBytes.Length);

            return signedBytes.ToBase64();
        }

        /// <summary>
        /// Decrypts a base64 payload.
        /// If successful, the data is parsed as an UTF8 JSON string.
        /// </summary>
        /// <typeparam name="T">Type of the object to decrypt.</typeparam>
        public T Decrypt<T>(string payload, AsymmetricKeyParameter receiverPrivateKey) {
            if (!receiverPrivateKey.IsPrivate) {
                throw new ArgumentException("Private key of receiver required for decryption", nameof(receiverPrivateKey));
            }

            var payloadBytes = payload.FromBase64();
            var decryptedBytes = Decrypt(payloadBytes, receiverPrivateKey);

            Logger.LogTrace(LoggingEvents.Cryptography,
                "Decrypt {2} chars (bytes {0} => {1})",
                payloadBytes.Length, decryptedBytes.Length, payload.Length);

            return JsonConvert.DeserializeObject<T>(decryptedBytes.AsUtf8String(), Client.JsonSettings);
        }

        private const int AesBlockSize = 128 / 8;
        private const int AesMinKeySize = 256 / 8;

        private IBufferedCipher CreateAesCipher(bool encrypt, byte[] initialVector, byte[] key) {
            if (key is null || key.Length != AesMinKeySize) {
                throw new ArgumentException(string.Format("AES key is not {0} bytes long", AesMinKeySize));
            }
            if (initialVector is null || initialVector.Length != AesBlockSize) {
                throw new ArgumentException(string.Format("AES initial vector is not {0} bytes long", AesBlockSize));
            }

            var cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new AesEngine()), new Pkcs7Padding());
            cipher.Init(encrypt, new ParametersWithIV(new KeyParameter(key), initialVector));

            return cipher;
        }

        /// <summary>
        /// Encrypts an object payload.
        /// If successful, payload is encoded as a base64 string.
        /// </summary>
        /// <param name="sessionKey">Temporary session key of 256 bits.</param>
        public string Encrypt<T>(T payload, byte[] sessionKey) {
            var payloadBytes = JsonConvert.SerializeObject(payload, Client.JsonSettings).ToBytes();

            byte[] iv = new byte[AesBlockSize];
            Generator.NextBytes(iv);

            var cipher = CreateAesCipher(true, iv, sessionKey);

            int outputSize = cipher.GetOutputSize(payloadBytes.Length);
            byte[] output = new byte[outputSize + AesBlockSize];
            Array.Copy(iv, output, iv.Length);

            Logger.LogTrace(LoggingEvents.Cryptography,
                "Encrypting {0} bytes to {1} bytes with {2}",
                payloadBytes.Length, outputSize, cipher.AlgorithmName);

            int outputLength = cipher.ProcessBytes(payloadBytes, output, AesBlockSize);
            int finalLength = cipher.DoFinal(output, AesBlockSize + outputLength);

            return output.ToBase64();
        }

        public T Decrypt<T>(string payload, byte[] sessionKey) {
            var payloadBytes = payload.FromBase64();

            if (payloadBytes.Length < AesBlockSize) {
                throw new ArgumentException("AES payload too short to contain IV");
            }
            byte[] iv = new byte[AesBlockSize];
            Array.Copy(payloadBytes, iv, AesBlockSize);

            var cipher = CreateAesCipher(false, iv, sessionKey);

            int outputSize = cipher.GetOutputSize(payloadBytes.Length);
            byte[] output = new byte[outputSize];

            Logger.LogTrace(LoggingEvents.Cryptography,
                "Decrypting {0} bytes to {1} bytes with {2}",
                payloadBytes.Length, outputSize, cipher.AlgorithmName);

            int outputLength = cipher.ProcessBytes(payloadBytes, AesBlockSize, payloadBytes.Length - AesBlockSize, output, 0);
            int finalLength = cipher.DoFinal(output, outputLength);

            return JsonConvert.DeserializeObject<T>(output.AsUtf8String(), Client.JsonSettings);
        }

        public const int SessionKeyLength = 256 / 8; // 256 bit

        /// <summary>
        /// Generate a new random session key of <see cref="SessionKeyLength"/> length.
        /// </summary>
        public byte[] GenerateSessionKey() {
            byte[] sessionKey = new byte[SessionKeyLength];
            Generator.NextBytes(sessionKey);
            return sessionKey;
        }

    }

}
