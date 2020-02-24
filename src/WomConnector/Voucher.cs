using System;
using System.Collections.Generic;
using System.Text;
using WomPlatform.Connector.Models;

namespace WomPlatform.Connector {

    /// <summary>
    /// In-memory representation of a voucher.
    /// </summary>
    public sealed class Voucher {

        public Voucher(Identifier id, string secret, string aim, double lat, double lng, DateTime timestamp) {
            Id = id;
            Secret = secret;
            Aim = aim;
            Latitude = lat;
            Longitude = lng;
            Timestamp = timestamp;
        }

        /// <summary>
        /// Unique voucher ID.
        /// </summary>
        public Identifier Id { get; private set; }

        /// <summary>
        /// Voucher secret for usage.
        /// </summary>
        public string Secret { get; private set; }

        /// <summary>
        /// Simple Aim code.
        /// </summary>
        public string Aim { get; private set; }

        /// <summary>
        /// Voucher generation latitude.
        /// </summary>
        public double Latitude { get; private set; }

        /// <summary>
        /// Voucher generation longitude.
        /// </summary>
        public double Longitude { get; private set; }

        /// <summary>
        /// Voucher generation timestamp.
        /// </summary>
        public DateTime Timestamp { get; private set; }

    }

}
