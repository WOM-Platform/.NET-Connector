using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WomPlatform.Connector.Models {

    /// <summary>
    /// Request payload for payment verification.
    /// </summary>
    public class PaymentVerifyPayload {

        /// <summary>
        /// Encrypted payload (instance of <see cref="Content" />).
        /// </summary>
        public string Payload { get; set; }

        /// <summary>
        /// Inner payload encrypted by source.
        /// </summary>
        public class Content {

            public Guid Otc { get; set; }

        }

    }

}
