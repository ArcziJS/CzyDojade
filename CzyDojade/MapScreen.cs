using Android.App;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Views.InputMethods;
using Android.Widget;
using Com.Mapbox.Core.Constants;
using Com.Mapbox.Geojson.Utils;
using Com.Mapbox.Mapboxsdk.Annotations;
using Com.Mapbox.Mapboxsdk.Camera;
using Com.Mapbox.Mapboxsdk.Geometry;
using Com.Mapbox.Mapboxsdk.Maps;
using Google.Android.Material.FloatingActionButton;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CzyDojade
{
    [Activity(Label = "Activity1")]
    public class MapScreen : Activity, IOnMapReadyCallback
    {
        MapView mapView;
        LatLng routeStart;
        LatLng routeEnd;
        List<Marker> markers = new List<Marker>();

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
            var hour = DateTime.Now.Hour;
            if (hour >= 6 && hour < 18)
            {
                mapboxMap.SetStyle(new Style.Builder().FromUrl("mapbox://styles/mapbox/light-v10"));
            }
            else
            {
                mapboxMap.SetStyle(new Style.Builder().FromUrl("mapbox://styles/mapbox/dark-v10"));
            }

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

                    MoveCameraBetween(routeStart, routeEnd);

                    await GetRoute(routeStart, routeEnd);

                }
                else
                {
                    Toast.MakeText(this, "Address not found", ToastLength.Short).Show();
                }
            };

            #endregion

            void MoveCamera(LatLng position)
            {
                mapboxMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(position, 15), 5000);
                CreateMarker(position);
            }

            void MoveCameraBetween(LatLng position1, LatLng position2)
            {
                LatLngBounds.Builder builder = new LatLngBounds.Builder();
                builder.Include(position1);
                builder.Include(position2);
                LatLngBounds bounds = builder.Build();

                int padding = 100;

                mapboxMap.AnimateCamera(CameraUpdateFactory.NewLatLngBounds(bounds, padding), 5000);

                CreateMarker(position1);
                CreateMarker(position2);
            }

            void CreateMarker(LatLng position)
            {
                MarkerOptions markerOptions = new MarkerOptions();
                markerOptions.SetPosition(position);

                IconFactory iconFactory = IconFactory.GetInstance(this);
                Icon icon = iconFactory.DefaultMarker();
                markerOptions.SetIcon(icon);

                Marker marker = mapboxMap.AddMarker(markerOptions);
                markers.Add(marker);

                int maxMarkerCount = 2;

                if (markers.Count > maxMarkerCount)
                {
                    Marker oldestMarker = markers[0];
                    oldestMarker.Remove();
                    markers.RemoveAt(0);
                }
            }

            #region Destination

            async Task GetRoute(LatLng routeStart, LatLng routeEnd)
            {
                var polylines = mapboxMap.Polylines;
                foreach (var polyline in polylines)
                {
                    mapboxMap.RemovePolyline(polyline);
                }

                var baseUrl = "https://api.mapbox.com/directions/v5";
                var accessToken = "pk.eyJ1IjoiY3p5ZG9qYWRlIiwiYSI6ImNsZ3k5MjBscTA3NTUzZnBlZ3VoYXYxMGIifQ.Gh80YFg9RRgTbG9WbxvPPQ";

                var routeOptions = new List<string>()
                    {
                        "driving",
                        "driving-traffic",
                        "walking",
                        "cycling"
                    };

                foreach (var option in routeOptions)
                {
                    var url = $"{baseUrl}/mapbox/{option}/{routeStart.Longitude},{routeStart.Latitude};{routeEnd.Longitude},{routeEnd.Latitude}.json?access_token={accessToken}";

                    using (var httpClient = new HttpClient())
                    {
                        var response = await httpClient.GetAsync(url);
                        if (response.IsSuccessStatusCode)
                        {
                            var json = await response.Content.ReadAsStringAsync();
                            var responseObject = JsonConvert.DeserializeObject<MapboxDirectionsResponse>(json);
                            if (responseObject.Routes.Count > 0 && mapboxMap != null)
                            {
                                var currentRoute = responseObject.Routes[0];
                                var points = PolylineUtils.Decode(currentRoute.Geometry, Constants.Precision6)
                                    .Select(p => new LatLng(p.Latitude(), p.Longitude()))
                                    .ToList();

                                var polylineOptions = new PolylineOptions()
                                    .InvokeColor(Color.Blue)
                                    .InvokeWidth(5);

                                foreach (var point in points)
                                {
                                    polylineOptions.Add(new LatLng(point.Latitude * 10, point.Longitude * 10));
                                }

                                mapboxMap.AddPolyline(polylineOptions);
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, response.ReasonPhrase, ToastLength.Short).Show();
                        }
                    }
                }
            }

            #endregion

            #region Bottom navigation bar
            ImageButton SettingsButton = FindViewById<ImageButton>(Resource.Id.menu_settings);
            SettingsButton.Click += delegate
            {
                StartActivity(typeof(UserSettingsPage));
                OverridePendingTransition(0, 0);
            };


            #endregion
        }

    }

}