using Android.Gms.Maps.Model;
using Com.Mapbox.Api.Directions.V5;
using Com.Mapbox.Geojson;
using Com.Mapbox.Geojson.Utils;
using Com.Mapbox.Mapboxsdk;
using Com.Mapbox.Mapboxsdk.Camera;
using Com.Mapbox.Mapboxsdk.Maps;
using Com.Mapbox.Services.Android.Navigation.V5.Navigation;
using Com.Mapbox.Services.Android.Navigation.V5.Route;
using System;
using System.Collections.Generic;


namespace CzyDojade
{
    internal class MapRouteGenerator
    {
        private MapboxMap mapboxMap;
        private string accessToken;

        public MapRouteGenerator(MapboxMap mapboxMap, string accessToken)
        {
            this.mapboxMap = mapboxMap;
            this.accessToken = accessToken;
        }

        public void GenerateRoute(LatLng routeStart, LatLng routeEnd)
        {
            MapboxDirections client = MapboxDirections.InvokeBuilder()
                .Origin(Point.FromLngLat(routeStart.Longitude, routeStart.Latitude))
                .Destination(Point.FromLngLat(routeEnd.Longitude, routeEnd.Latitude))
                .AccessToken(accessToken)
                .Build();

            client.EnqueueCall(new DirectionsResponseCallback(this));
        }

        private class DirectionsResponseCallback : Java.Lang.Object, ICallback
        {
            private MapRouteGenerator parent;

            public DirectionsResponseCallback(MapRouteGenerator parent)
            {
                this.parent = parent;
            }

            public void OnFailure(Java.Lang.Object p0)
            {
                // Handle route generation failure
            }

            public void OnResponse(Java.Lang.Object p0)
            {
                var response = (Response<DirectionsResponse>)p0;
                if (response.Body() != null && response.Body().Routes().Count > 0)
                {
                    DirectionsRoute route = response.Body().Routes()[0];
                    LineString lineString = LineString.FromPolyline(route.Geometry(), Constants.Precision6);
                    List<LatLng> polylinePoints = lineString.Coordinates;

                    parent.mapboxMap.InvokeStyle(mapStyle =>
                    {
                        mapStyle.AddPolyline(new PolylineOptions()
                            .Add(polylinePoints.ToArray())
                            .InvokeWidth(5f)
                            .InvokeColor(Android.Graphics.Color.Red));
                    });

                    parent.mapboxMap.InvokeCamera(CameraUpdateFactory.NewLatLngBounds(
                        new LatLngBounds.Builder()
                            .Includes(polylinePoints)
                            .Build(), 50));
                }
            }
        }
    }
}
