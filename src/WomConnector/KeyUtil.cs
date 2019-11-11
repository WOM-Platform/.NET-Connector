using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;

namespace WomPlatform.Connector {

    /// <summary>
    /// Key loading utilities.
    /// </summary>
    public static class KeyUtil {

        /// <summary>
        /// Load object from PEM stream.
        /// Stream is disposed after usage.
        /// </summary>
        public static T LoadFromPem<T>(Stream s) where T : class {
            using(var txReader = new StreamReader(s)) {
                var reader = new PemReader(txReader);
                return reader.ReadObject() as T;
            }
        }

        /// <summary>
        /// Load asymmetric cipher pair (private and public) from PEM stream.
        /// </summary>
        public static AsymmetricCipherKeyPair LoadCipherKeyPairFromPem(Stream s) {
            return LoadFromPem<AsymmetricCipherKeyPair>(s);
        }

        /// <summary>
        /// Load single key parameter (either private or public) from PEM stream.
        /// </summary>
        public static AsymmetricKeyParameter LoadKeyParameterFromPem(Stream s) {
            return LoadFromPem<AsymmetricKeyParameter>(s);
        }

    }

}
