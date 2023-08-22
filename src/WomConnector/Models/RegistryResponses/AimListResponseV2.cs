using System.Collections.Generic;

namespace WomPlatform.Connector.Models.RegistryResponses {

    public class AimListResponseV2 {

        public Aim[] Aims { get; set; }

        public class Aim {

            public string Code { get; set; }

            public Dictionary<string, string> Titles { get; set; }

            public int Order { get; set; }

        }

    }

}
