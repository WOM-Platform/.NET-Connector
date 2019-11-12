using System;
using System.Collections.Generic;
using System.Text;

namespace WomPlatform.Connector {

    /// <summary>
    /// Describes a geographical bound.
    /// </summary>
    public struct GeoBounds {

        public GeoCoords LeftTop { get; set; }

        public GeoCoords RightBottom { get; set; }

    }

    public static class GeoBoundsExtensions {

        /// <summary>
        /// Checks whether a geographic point is within geographic bounds.
        /// Considers bounds spanning around the geosphere.
        /// </summary>
        public static bool Contains(this GeoBounds bounds, GeoCoords point) {
            bool inLatRange = (bounds.LeftTop.Latitude > bounds.RightBottom.Latitude) ?
                (point.Latitude <= bounds.LeftTop.Latitude && point.Latitude >= bounds.RightBottom.Latitude) :
                (point.Latitude <= bounds.LeftTop.Latitude || point.Latitude >= bounds.RightBottom.Latitude);
            bool inLngRange = (bounds.LeftTop.Longitude < bounds.RightBottom.Longitude) ?
                (point.Longitude >= bounds.LeftTop.Longitude && point.Longitude <= bounds.RightBottom.Longitude) :
                (point.Longitude >= bounds.LeftTop.Longitude || point.Longitude <= bounds.RightBottom.Longitude);

            return inLatRange && inLngRange;
        }

        /// <summary>
        /// Create a geo bounds instance from data model bounds.
        /// </summary>
        public static GeoBounds ToGeo(this Models.Bounds modelBounds) => new GeoBounds {
            LeftTop = modelBounds.LeftTop.ToCoords(),
            RightBottom = modelBounds.RightBottom.ToCoords()
        };

        /// <summary>
        /// Check whether data model bounds contain a geographical point.
        /// </summary>
        public static bool Contains(this Models.Bounds modelBounds, double latitude, double longitude) {
            return modelBounds.ToGeo().Contains(new GeoCoords {
                Latitude = latitude,
                Longitude = longitude
            });
        }

    }

}
