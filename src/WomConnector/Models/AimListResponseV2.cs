using System;
using System.Collections.Generic;
using System.Text;

namespace WomPlatform.Connector.Models {

    public class AimListResponseV2 {

        public List<Aim> Aims { get; set; }

        public class Aim {

            public string Code { get; set; }

            public Dictionary<string, string> Titles { get; set; }

            public int Order { get; set; }

        }

    }

}
