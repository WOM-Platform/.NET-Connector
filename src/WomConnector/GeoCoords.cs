using System;
using System.Collections.Generic;
using System.Text;

namespace WomPlatform.Connector {

    /// <summary>
    /// Describes a geographical coordinate as a couple of latitude and longitude measures.
    /// </summary>
    public struct GeoCoords {

        public double Latitude;

        public double Longitude;

    }

    public static class GeoCoordsExtensions {

        private const double MeanEarthRadius = 6378.16;

        /// <summary>
        /// Convert double array to geographic coordinates.
        /// </summary>
        public static GeoCoords ToCoords(this double[] vd) {
            if(vd == null) {
                throw new ArgumentNullException(nameof(vd));
            }
            if(vd.Length != 2) {
                throw new ArgumentException("Wrong length of coords vector", nameof(vd));
            }
            if(vd[0] > 90.0 || vd[0] < -90.0) {
                throw new ArgumentException("Latitude cannot be less than -90 or greater than 90");
            }
            if(vd[1] < -180.0 || vd[1] > 180.0) {
                throw new ArgumentException("Longitude cannot be less than -180 or greater than 180");
            }

            return new GeoCoords {
                Latitude = vd[0],
                Longitude = vd[1]
            };
        }

        /// <summary>
        /// Converts a degree measurement to radians.
        /// </summary>
        public static double FromDegreesToRadians(double d) {
            return d * Math.PI / 180.0;
        }

        /// <summary>
        /// Computes the approximate distance in kilometers between two geographical points.
        /// Uses a "Haversine" great circle calculation, on an ideal sphere. Not exact close to equator and poles.
        /// </summary>
        /// <param name="lat1">Latitude of first point.</param>
        /// <param name="lng1">Longitude of first point.</param>
        /// <param name="lat2">Latitude of second point.</param>
        /// <param name="lng2">Longitude of second point.</param>
        /// <returns>Distance in kilometers.</returns>
        private static double DistanceBetweenPoints(double lat1, double lng1, double lat2, double lng2) {
            double dLng = FromDegreesToRadians(lng2 - lng1);
            double dLat = FromDegreesToRadians(lat2 - lat1);

            double radLat1 = FromDegreesToRadians(lat1);
            double radLat2 = FromDegreesToRadians(lat2);

            double a = (Math.Sin(dLat / 2) * Math.Sin(dLat / 2)) + Math.Cos(radLat1) * Math.Cos(radLat2) * (Math.Sin(dLng / 2) * Math.Sin(dLng / 2));
            double angle = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return angle * MeanEarthRadius;
        }

        /// <summary>
        /// Computes distance between two geographical coordinates.
        /// </summary>
        /// <returns>Distance in kilometers.</returns>
        public static double DistanceBetween(GeoCoords a, GeoCoords b) {
            return DistanceBetweenPoints(a.Latitude, a.Longitude, b.Latitude, b.Longitude);
        }

    }

}
