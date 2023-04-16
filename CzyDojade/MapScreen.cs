using Android.App;
using Android.OS;
using Android.Widget;
using Com.Mapbox.Mapboxsdk;
using Com.Mapbox.Mapboxsdk.Camera;
using Com.Mapbox.Mapboxsdk.Geometry;
using Com.Mapbox.Mapboxsdk.Maps;
using System.Linq;
//using Mapbox.Services.Geocoding;
using Xamarin.Essentials;

namespace CzyDojade
{
    [Activity(Label = "Activity1")]
    public class MapScreen : Activity, IOnMapReadyCallback
    {
        MapView mapView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Mapbox.GetInstance(this, "pk.eyJ1IjoiY3p5ZG9qYWRlIiwiYSI6ImNsZ2RyZmk2azBiaDUzZXEzY3ZqdXhlOWYifQ.DXoRCQQlVsnu4Ujv3-r7OQ");

            SetContentView(Resource.Layout.map_screen);

            mapView = FindViewById<MapView>(Resource.Id.mapView);
            mapView.OnCreate(savedInstanceState);
            mapView.GetMapAsync(this);
        }

        public void OnMapReady(MapboxMap mapboxMap)
        {
            mapboxMap.SetStyle(new Style.Builder().FromUrl("mapbox://styles/mapbox/dark-v10"));


            CameraPosition cameraPosition = new CameraPosition.Builder()
                .Target(new LatLng(53.123326, 23.08638))
                .Zoom(18)
                .Build();

            mapboxMap.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition), 5000);

            #region Search

            SearchView searchView = FindViewById<SearchView>(Resource.Id.searchView);
            searchView.QueryTextSubmit += async (sender, args) =>
            {
                var results = await Geocoding.GetLocationsAsync(args.Query);

                var location = results.FirstOrDefault();
                var position = new LatLng(location.Latitude, location.Longitude);
                mapboxMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(position, 15),5000);
            };

            #endregion
        }





    }
}