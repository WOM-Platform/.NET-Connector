using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Crypto;
using WomPlatform.Connector;

namespace WomConnector.Tester {

    internal static class Util {

        private class Logger : ILogger {

            public IDisposable BeginScope<TState>(TState state) {
                return null;
            }

            public bool IsEnabled(LogLevel logLevel) {
                return true;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
                Console.WriteLine("{0} {1} {2}", logLevel, eventId, formatter(state, exception));
            }

        }

        private class LoggerFactory : ILoggerFactory {

            public void AddProvider(ILoggerProvider provider) {

            }

            public ILogger CreateLogger(string categoryName) {
                return new Logger();
            }

            public void Dispose() {

            }

        }

        public static Client CreateClient() {
            return new Client("dev.wom.social", new LoggerFactory());
        }

        private static Client _client = null;

        public static Client Client {
            get {
                if(_client == null) {
                    _client = CreateClient();
                }
                return _client;
            }
        }

        public static Instrument CreateInstrument(string sourceId, string sourceKeyPath) {
            AsymmetricCipherKeyPair keys = null;
            using(var fs = new FileStream(sourceKeyPath, FileMode.Open)) {
                keys = KeyUtil.LoadCipherKeyPairFromPem(fs);
            }

            return Client.CreateInstrument(sourceId, keys.Private);
        }

        public static Instrument GenerateInstrument() {
            return CreateInstrument("5e74203f5f21bb265a2d26bd", "keys/source1.pem");
        }

        public static Pocket CreatePocket() {
            return Client.CreatePocket();
        }

        public static PointOfSale CreatePos(string posId, string posKeyPath) {
            AsymmetricCipherKeyPair keys = null;
            using(var fs = new FileStream(posKeyPath, FileMode.Open)) {
                keys = KeyUtil.LoadCipherKeyPairFromPem(fs);
            }

            return Client.CreatePos(posId, keys.Private);
        }

        public static PointOfSale GeneratePos() {
            return CreatePos("5e74205c5f21bb265a2d26d8", "keys/pos1.pem");
        }

    }

}
