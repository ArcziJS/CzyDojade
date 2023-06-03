using Com.Mapbox.Mapboxsdk.Geometry;
using System;

namespace CzyDojade
{
    static class BearingCalculator
    {
        public static double CalculateBearing(LatLng start, LatLng end)
        {
            double lat1 = DegreesToRadians(start.Latitude);
            double lon1 = DegreesToRadians(start.Longitude);
            double lat2 = DegreesToRadians(end.Latitude);
            double lon2 = DegreesToRadians(end.Longitude);

            double y = Math.Sin(lon2 - lon1) * Math.Cos(lat2);
            double x = (Math.Cos(lat1) * Math.Sin(lat2)) - (Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(lon2 - lon1));
            double bearing = Math.Atan2(y, x);

            bearing = RadiansToDegrees(bearing);
            bearing = (bearing + 360) % 360;

            return bearing;
        }

        static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        static double RadiansToDegrees(double radians)
        {
            return radians * 180 / Math.PI;
        }
    }
}