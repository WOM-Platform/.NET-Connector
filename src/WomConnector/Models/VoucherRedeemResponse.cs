using System;

namespace WomPlatform.Connector.Models {

    public class VoucherRedeemResponse {

        /// <summary>
        /// Encrypted payload, encoded as an <see cref="Content" /> instance.
        /// </summary>
        public string Payload { get; set; }

        /// <summary>
        /// Payload encrypted with session key.
        /// </summary>
        public class Content {

            /// <summary>
            /// ID of the source.
            /// </summary>
            public Identifier SourceId { get; set; }

            /// <summary>
            /// Name of the source.
            /// </summary>
            public string SourceName { get; set; }

            /// <summary>
            /// List of redeemed vouchers.
            /// </summary>
            public VoucherInfo[] Vouchers { get; set; }

        }

        /// <summary>
        /// Encapsulates info about a single voucher.
        /// </summary>
        public class VoucherInfo {

            /// <summary>
            /// Unique voucher ID.
            /// </summary>
            public Identifier Id { get; set; }

            /// <summary>
            /// Voucher secret for usage.
            /// </summary>
            public string Secret { get; set; }

            /// <summary>
            /// Simple Aim code.
            /// </summary>
            public string Aim { get; set; }

            public double Latitude { get; set; }

            public double Longitude { get; set; }

            public DateTime Timestamp { get; set; }

        }

    }

}
