using System;
using System.Linq;
using System.Text;

namespace WomPlatform.Connector {

    public static class StringExtensions {

        public static string ToBase64(this byte[] b) {
            return Convert.ToBase64String(b, Base64FormattingOptions.None);
        }

        public static string ToBase64(this string s) {
            return Convert.ToBase64String(s.ToBytes(), Base64FormattingOptions.None);
        }

        public static byte[] FromBase64(this string s) {
            return Convert.FromBase64String(s);
        }

        private static readonly uint[] _hexLookup = Enumerable.Range(0, 256).Select(i => {
            string s = i.ToString("X2");
            return s[0] + ((uint)s[1] << 16);
        }).ToArray();

        public static string ToHexString(this byte[] bytes) {
            var result = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++) {
                var val = _hexLookup[bytes[i]];
                result[2 * i] = (char)val;
                result[2 * i + 1] = (char)(val >> 16);
            }
            return new string(result);
        }

        public static byte[] ToBytes(this string s) {
            return Encoding.UTF8.GetBytes(s);
        }

        public static string AsUtf8String(this byte[] b) {
            return Encoding.UTF8.GetString(b);
        }

        public static bool LengthIsBetween(this string s, int min, int max) {
            if (s is null)
                return false;

            return s.Length >= min && s.Length <= max;
        }

    }

}
