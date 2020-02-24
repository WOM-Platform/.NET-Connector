using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WomPlatform.Connector.Models {

    public class PaymentInfoResponse {

        /// <summary>
        /// Encrypted payload (instance of <see cref="Content" />).
        /// </summary>
        public string Payload { get; set; }

        /// <summary>
        /// Payload encrypted with session key.
        /// </summary>
        public class Content {

            /// <summary>
            /// Unique ID of the POS.
            /// </summary>
            public Identifier PosId { get; set; }

            /// <summary>
            /// Name of the POS.
            /// </summary>
            public string PosName { get; set; }

            /// <summary>
            /// Amount of vouchers to consume for payment.
            /// </summary>
            public int Amount { get; set; }

            /// <summary>
            /// Simple filter conditions that vouchers must satisfy. May be null.
            /// </summary>
            public SimpleFilter SimpleFilter { get; set; }

            /// <summary>
            /// Gets whether the payment is persistent.
            /// </summary>
            public bool Persistent { get; set; }

        }

    }

}
