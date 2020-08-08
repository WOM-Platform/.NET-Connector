using System.Collections.Generic;

namespace WomPlatform.Connector.Models {

    public class SourceLoginResultV1 {

        public List<Source> Sources { get; set; }

        public class Source {

            public Identifier Id { get; set; }

            public string Name { get; set; }

            public string Url { get; set; }

            public string PrivateKey { get; set; }

            public string PublicKey { get; set; }

        }

    }

}
