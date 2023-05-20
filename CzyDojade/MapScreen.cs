using Android.App;
using Android.OS;
using Android.Views.InputMethods;
using Android.Widget;
using Com.Mapbox.Mapboxsdk;
using Com.Mapbox.Mapboxsdk.Annotations;
using Com.Mapbox.Mapboxsdk.Camera;
using Com.Mapbox.Mapboxsdk.Geometry;
using Com.Mapbox.Mapboxsdk.Maps;
using Com.Mapbox.Api.Directions.V5;
using Com.Mapbox.Android.Telemetry.Location;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
//using Com.Mapbox.Core.Constants;
using Android.Graphics;
using Com.Mapbox.Api.Directions.V5.Models;
using Com.Mapbox.Geojson.Utils;
using Com.Mapbox.Core.Constants;
using Android.Locations;
using Com.Mapbox.Android.Core.Location;
//using Com.Mapbox.Api;
//using Com.Mapbox.Core.Utils;
//using Com.Mapbox.Api.Directions.V5.Models;
//using Com.Mapbox.Api.Directions.V5;
//using Com.Mapbox.Geojson;
using Com.Mapbox.Android.Core.Location;
using Google.Android.Material.FloatingActionButton;

namespace CzyDojade
{
    [Activity(Label = "Activity1")]
    public class MapScreen : Activity, IOnMapReadyCallback
    {
        MapView mapView;
        LatLng routeStart;
        LatLng routeEnd;
        //MapRouteGenerator routeGenerator;
        

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Com.Mapbox.Mapboxsdk.Mapbox.GetInstance(this, "pk.eyJ1IjoiY3p5ZG9qYWRlIiwiYSI6ImNsZ3k5MjBscTA3NTUzZnBlZ3VoYXYxMGIifQ.Gh80YFg9RRgTbG9WbxvPPQ");            

            SetContentView(Resource.Layout.map_screen);

            mapView = FindViewById<MapView>(Resource.Id.mapView);
            mapView.OnCreate(savedInstanceState);
            mapView.GetMapAsync(this);

            //get access to android location, that will later be used to get current gps location
            LocationManager locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteria = new Criteria();
            string provider = locationManager.GetBestProvider(criteria, false);
            Location location = locationManager.GetLastKnownLocation(provider);
            
        }

        public void OnMapReady(MapboxMap mapboxMap)
        {
            mapboxMap.SetStyle(new Style.Builder().FromUrl("mapbox://styles/mapbox/dark-v10"));

            //routeGenerator = new MapRouteGenerator(mapboxMap, "pk.eyJ1IjoiY3p5ZG9qYWRlIiwiYSI6ImNsZ3k5MjBscTA3NTUzZnBlZ3VoYXYxMGIifQ.Gh80YFg9RRgTbG9WbxvPPQ");
        

        CameraPosition cameraPosition = new CameraPosition.Builder()
                .Target(new LatLng(53.123326, 23.08638))
                .Zoom(18)
                .Build();

            mapboxMap.AnimateCamera(CameraUpdateFactory.NewCameraPosition(cameraPosition), 5000);

            #region Return to current gps location button   
            //use floating action button with id returnToMyLocation to return to current gps location
            FloatingActionButton returnToMyLocation = FindViewById<FloatingActionButton>(Resource.Id.returnToMyLocation);
            returnToMyLocation.Click += (sender, e) =>
            {
                LocationManager locationManager = (LocationManager)GetSystemService(LocationService);
                Criteria criteria = new Criteria();
                string provider = locationManager.GetBestProvider(criteria, false);
                Location location = locationManager.GetLastKnownLocation(provider);

                if (location != null)
                {
                    LatLng myLocation = new LatLng(location.Latitude, location.Longitude);
                    MoveCamera(myLocation);
                }
            };
            


  


            #endregion

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
                    CreateMarker(routeEnd);

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
                        CreateMarker(routeEnd);

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

                    CreateMarker(routeEnd);
                    //routeGenerator.GenerateRoute(routeStart, routeEnd);

                    await GetRoute(routeStart, routeEnd);
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
                CreateMarker(position);
            }

            void CreateMarker(LatLng position)
            {
                MarkerOptions markerOptions = new MarkerOptions();
                markerOptions.SetPosition(position);
                mapboxMap.AddMarker(markerOptions);
            }

            async Task GetRoute(LatLng routeStart, LatLng routeEnd)
            {
                //var origin = Position.FromLngLat(routeStart.Longitude, routeStart.Latitude);
                //var destination = Position.FromLngLat(routeEnd.Longitude, routeEnd.Latitude);

                var baseUrl = "https://api.mapbox.com/directions/v5";
                var url = $"{baseUrl}/{DirectionsCriteria.ProfileDriving}/{routeStart.Longitude},{routeStart.Latitude};{routeEnd.Longitude},{routeEnd.Latitude}.json?access_token={Mapbox.AccessToken}";

                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var directionsResponse = JsonConvert.DeserializeObject<DirectionsResponse>(json);
                    if (directionsResponse.Routes().Count > 0)
                    {
                        var currentRoute = directionsResponse.Routes()[0];
                        if (mapboxMap != null)
                        {
                            var points = PolylineUtils.Decode(currentRoute.Geometry(), Constants.Precision6)
                    .Select(p => new LatLng(p.Latitude(), p.Longitude()))
                    .ToList();

                            mapboxMap.AddPolyline(new PolylineOptions()
                                .AddAll((Java.Lang.IIterable)points)
                                .InvokeColor(Color.Blue)
                                .InvokeWidth(5));
                        }
                    }
                }
            }

            #endregion
        }
    }
}