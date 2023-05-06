using Android.App;
using Android.OS;
using Android.Views.InputMethods;
using Android.Widget;
using Com.Mapbox.Mapboxsdk;
using Com.Mapbox.Mapboxsdk.Annotations;
using Com.Mapbox.Mapboxsdk.Camera;
using Com.Mapbox.Mapboxsdk.Geometry;
using Com.Mapbox.Mapboxsdk.Maps;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Com.Mapbox.Api;
using Com.Mapbox.Core.Utils;
using Com.Mapbox.Api.Directions.V5.Models;
using Com.Mapbox.Api.Directions.V5;
using Com.Mapbox.Geojson;

namespace CzyDojade
{
    [Activity(Label = "Activity1")]
    public class MapScreen : Activity, IOnMapReadyCallback
    {
        MapView mapView;
        LatLng routeStart;
        LatLng routeEnd;
        MapRouteGenerator routeGenerator;
        

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Com.Mapbox.Mapboxsdk.Mapbox.GetInstance(this, "pk.eyJ1IjoiY3p5ZG9qYWRlIiwiYSI6ImNsZ3k5MjBscTA3NTUzZnBlZ3VoYXYxMGIifQ.Gh80YFg9RRgTbG9WbxvPPQ");            

            SetContentView(Resource.Layout.map_screen);

            mapView = FindViewById<MapView>(Resource.Id.mapView);
            mapView.OnCreate(savedInstanceState);
            mapView.GetMapAsync(this);

            
        }

        public void OnMapReady(MapboxMap mapboxMap)
        {
            mapboxMap.SetStyle(new Style.Builder().FromUrl("mapbox://styles/mapbox/dark-v10"));

            routeGenerator = new MapRouteGenerator(mapboxMap, "pk.eyJ1IjoiY3p5ZG9qYWRlIiwiYSI6ImNsZ3k5MjBscTA3NTUzZnBlZ3VoYXYxMGIifQ.Gh80YFg9RRgTbG9WbxvPPQ");
        

        CameraPosition cameraPosition = new CameraPosition.Builder()
                .Target(new LatLng(53.123326, 23.08638))
                .Zoom(18)
                .Build();

            mapboxMap.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition), 5000);



            #region Search
            AutoCompleteTextView searchViewSource = FindViewById<AutoCompleteTextView>(Resource.Id.searchViewSource);
            AutoCompleteTextView searchViewDestination = FindViewById<AutoCompleteTextView>(Resource.Id.searchViewDestination);

            searchViewSource.TextChanged += async (sender, e) =>
            {
                string query = searchViewSource.Text;

                JObject response = await Geocoding.ForwardGeocodeAsync(query, languages: new string[] { "pl" });

                JArray features = (JArray)response["features"];
                string[] suggestions = features.Select(f => (string)f["place_name"]).ToArray();

                ArrayAdapter adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, suggestions);
                searchViewSource.Adapter = adapter;
            };

            searchViewSource.ItemClick += async (sender, args) =>
            {

                string selectedAddress = searchViewSource.Adapter.GetItem(args.Position).ToString();

                JObject response = await Geocoding.ForwardGeocodeAsync(selectedAddress);
                   
                JArray features = (JArray)response["features"];
                if (features.Count > 0)
                {
                    JObject firstResult = (JObject)features[0];

                    JArray coordinates = (JArray)firstResult["geometry"]["coordinates"];
                    double longitude = (double)coordinates[0];
                    double latitude = (double)coordinates[1];
                    routeStart = new LatLng(latitude, longitude);

                    MoveCamera(routeStart);
                }
                else
                {
                    Toast.MakeText(this, "Address not found", ToastLength.Short).Show();
                }
            };

            searchViewSource.EditorAction += async (sender, args) =>
            {
                if (args.ActionId == ImeAction.Done)
                {
                    string query = searchViewSource.Text;

                    JObject response = await Geocoding.ForwardGeocodeAsync(query);

                    JArray features = (JArray)response["features"];
                    if (features.Count > 0)
                    {
                        JObject firstResult = (JObject)features[0];

                        JArray coordinates = (JArray)firstResult["geometry"]["coordinates"];
                        double longitude = (double)coordinates[0];
                        double latitude = (double)coordinates[1];
                        routeStart = new LatLng(latitude, longitude);

                        MoveCamera(routeStart);
                    }
                    else
                    {
                        Toast.MakeText(this, "Address not found", ToastLength.Short).Show();
                    }
                }
            };

            searchViewDestination.TextChanged += async (sender, e) =>
            {
                string query = searchViewDestination.Text;

                JObject response = await Geocoding.ForwardGeocodeAsync(query, languages: new string[] { "pl" });

                JArray features = (JArray)response["features"];
                string[] suggestions = features.Select(f => (string)f["place_name"]).ToArray();

                ArrayAdapter adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, suggestions);
                searchViewDestination.Adapter = adapter;
            };

            searchViewDestination.ItemClick += async (sender, args) =>
            {

                string selectedAddress = searchViewDestination.Adapter.GetItem(args.Position).ToString();

                JObject response = await Geocoding.ForwardGeocodeAsync(selectedAddress);

                JArray features = (JArray)response["features"];
                if (features.Count > 0)
                {
                    JObject firstResult = (JObject)features[0];

                    JArray coordinates = (JArray)firstResult["geometry"]["coordinates"];
                    double longitude = (double)coordinates[0];
                    double latitude = (double)coordinates[1];
                    routeEnd = new LatLng(latitude, longitude);
                }
                else
                {
                    Toast.MakeText(this, "Address not found", ToastLength.Short).Show();
                }
            };

            searchViewDestination.EditorAction += async (sender, args) =>
            {
                if (args.ActionId == ImeAction.Done)
                {
                    string query = searchViewDestination.Text;

                    JObject response = await Geocoding.ForwardGeocodeAsync(query);

                    JArray features = (JArray)response["features"];
                    if (features.Count > 0)
                    {
                        JObject firstResult = (JObject)features[0];

                        JArray coordinates = (JArray)firstResult["geometry"]["coordinates"];
                        double longitude = (double)coordinates[0];
                        double latitude = (double)coordinates[1];
                        routeEnd = new LatLng(latitude, longitude);
                    }
                    else
                    {
                        Toast.MakeText(this, "Address not found", ToastLength.Short).Show();
                    }
                }
            };


            Button searchButtonSource = FindViewById<Button>(Resource.Id.searchButtonSource);
            Button searchButtonDestination = FindViewById<Button>(Resource.Id.searchButtonDestination);

            searchButtonSource.Click += async (sender, args) =>
            {
                string query = searchViewSource.Text;

                JObject response = await Geocoding.ForwardGeocodeAsync(query);

                JArray features = (JArray)response["features"];
                if (features.Count > 0)
                {
                    JObject firstResult = (JObject)features[0];

                    JArray coordinates = (JArray)firstResult["geometry"]["coordinates"];
                    double longitude = (double)coordinates[0];
                    double latitude = (double)coordinates[1];
                    routeStart = new LatLng(latitude, longitude);

                    MoveCamera(routeStart);
                }
                else
                {
                    Toast.MakeText(this, "Address not found", ToastLength.Short).Show();
                }
            };

            searchButtonDestination.Click += async (sender, args) =>
            {
                string query = searchViewDestination.Text;

                JObject response = await Geocoding.ForwardGeocodeAsync(query);

                JArray features = (JArray)response["features"];
                if (features.Count > 0)
                {
                    JObject firstResult = (JObject)features[0];

                    JArray coordinates = (JArray)firstResult["geometry"]["coordinates"];
                    double longitude = (double)coordinates[0];
                    double latitude = (double)coordinates[1];
                    routeEnd = new LatLng(latitude, longitude);

                    routeGenerator.GenerateRoute(routeStart, routeEnd);
                }
                else
                {
                    Toast.MakeText(this, "Address not found", ToastLength.Short).Show();
                }
            };


            #endregion
            #region Destination

            void MoveCamera(LatLng position)
            {
                mapboxMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(position, 15), 5000);
            }

            

            #endregion
        }
    }
}