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
            AsymmetricKeyParameter pubKey = null;
            using(var fs = new FileStream("keys/registry.pub", FileMode.Open)) {
                pubKey = KeyUtil.LoadKeyParameterFromPem(fs);
            }

            return new Client(new LoggerFactory(), pubKey) {
                TestMode = true
            };
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

        public static Instrument CreateInstrument(long sourceId, string sourceKeyPath) {
            AsymmetricCipherKeyPair keys = null;
            using(var fs = new FileStream(sourceKeyPath, FileMode.Open)) {
                keys = KeyUtil.LoadCipherKeyPairFromPem(fs);
            }

            return Client.CreateInstrument(sourceId, keys.Private);
        }

        public static Pocket CreatePocket() {
            return Client.CreatePocket();
        }

    }

}
