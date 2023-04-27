using Android.App;
using Android.OS;
using Android.Widget;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Com.Mapbox.Mapboxsdk;
using Xamarin.Essentials;
using Com.Mapbox.Mapboxsdk.Maps;
using Com.Mapbox.Mapboxsdk.Location;
using Com.Mapbox.Mapboxsdk.Camera;
using Com.Mapbox.Mapboxsdk.Geometry;
using Android.Views.InputMethods;

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
            LocationComponent a;
            

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
            AutoCompleteTextView searchView = FindViewById<AutoCompleteTextView>(Resource.Id.searchView);
            searchView.TextChanged += async (sender, e) =>
            {
                string query = searchView.Text;

                JObject response = await Geocoding.ForwardGeocodeAsync(query, languages: new string[] { "pl" });

                JArray features = (JArray)response["features"];
                string[] suggestions = features.Select(f => (string)f["place_name"]).ToArray();

                ArrayAdapter adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, suggestions);
                searchView.Adapter = adapter;
            };

            searchView.ItemClick += async (sender, args) =>
            {

                string selectedAddress = searchView.Adapter.GetItem(args.Position).ToString();

                JObject response = await Geocoding.ForwardGeocodeAsync(selectedAddress);
                   
                JArray features = (JArray)response["features"];
                if (features.Count > 0)
                {
                    JObject firstResult = (JObject)features[0];

                    JArray coordinates = (JArray)firstResult["geometry"]["coordinates"];
                    double longitude = (double)coordinates[0];
                    double latitude = (double)coordinates[1];
                    var position = new LatLng(latitude, longitude);

                    mapboxMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(position, 15), 5000);
                }
                else
                {
                    Toast.MakeText(this, "Address not found", ToastLength.Short).Show();
                }
            };

            searchView.EditorAction += async (sender, args) =>
            {
                if (args.ActionId == ImeAction.Done)
                {
                    string query = searchView.Text;

                    JObject response = await Geocoding.ForwardGeocodeAsync(query);

                    JArray features = (JArray)response["features"];
                    if (features.Count > 0)
                    {
                        JObject firstResult = (JObject)features[0];

                        JArray coordinates = (JArray)firstResult["geometry"]["coordinates"];
                        double longitude = (double)coordinates[0];
                        double latitude = (double)coordinates[1];
                        var position = new LatLng(latitude, longitude);

                        mapboxMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(position, 15), 5000);
                    }
                    else
                    {
                        Toast.MakeText(this, "Address not found", ToastLength.Short).Show();
                    }
                }
            };


            Button searchButton = FindViewById<Button>(Resource.Id.searchButton);
            searchButton.Click += async (sender, args) =>
            {
                string query = searchView.Text;

                JObject response = await Geocoding.ForwardGeocodeAsync(query);

                JArray features = (JArray)response["features"];
                if (features.Count > 0)
                {
                    JObject firstResult = (JObject)features[0];

                    JArray coordinates = (JArray)firstResult["geometry"]["coordinates"];
                    double longitude = (double)coordinates[0];
                    double latitude = (double)coordinates[1];
                    var position = new LatLng(latitude, longitude);

                    mapboxMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(position, 15), 5000);
                }
                else
                {
                    Toast.MakeText(this, "Address not found", ToastLength.Short).Show();
                }
            };


            #endregion
        }





    }
}