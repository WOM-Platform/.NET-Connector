using System.Collections.Generic;

namespace WomPlatform.Connector.Models {

    public class MerchantLoginResultV2 {

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Email { get; set; }

        public List<Merchant> Merchants { get; set; }

        public class Merchant {

            public Identifier Id { get; set; }

            public string Name { get; set; }

            public string FiscalCode { get; set; }

            public string Address { get; set; }

            public string ZipCode { get; set; }

            public string City { get; set; }

            public string Country { get; set; }

            public List<Pos> Pos { get; set; }

        }

        public class Pos {

            public Identifier Id { get; set; }

            public string Name { get; set; }

            public string PrivateKey { get; set; }

            public string PublicKey { get; set; }

        }

    }

}
