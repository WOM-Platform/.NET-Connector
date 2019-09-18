using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WomPlatform.Connector.Models {

    public class SimpleFilter {

        public string Aim { get; set; }

        public Bounds Bounds { get; set; }

        public long? MaxAge { get; set; }

    }

}
