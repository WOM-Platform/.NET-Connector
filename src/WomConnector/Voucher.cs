using System;
using System.Collections.Generic;
using System.Text;

namespace WomPlatform.Connector {

    /// <summary>
    /// In-memory representation of a voucher.
    /// </summary>
    public class Voucher {

        /// <summary>
        /// Unique voucher ID.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Voucher secret for usage.
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Simple Aim code.
        /// </summary>
        public string Aim { get; set; }

        /// <summary>
        /// Voucher generation latitude.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Voucher generation longitude.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Voucher generation timestamp.
        /// </summary>
        public DateTime Timestamp { get; set; }

    }

}
