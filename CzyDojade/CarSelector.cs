using Android.App;
using Android.OS;
using Android.Widget;
using MySqlConnector;

namespace CzyDojade
{
    [Activity(Label = "CarSelector", Theme = "@style/AppTheme")]
    public class CarSelector : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.car_selection);

            TextView Car = FindViewById<TextView>(Resource.Id.textView1);
            TextView Range = FindViewById<TextView>(Resource.Id.textView2);
            Button getBackButton = FindViewById<Button>(Resource.Id.getBackButton);

            MySqlConnection connection = new MySqlConnection("Server=db4free.net;Port=3306;Database=samochody;Uid=czydojade;Pwd=czydojade;");
            connection.Open();

            var query = connection.CreateCommand();
            query.CommandText = "SELECT marka, model, zasieg FROM cars";

            var result = query.ExecuteReader();

            result.Read();

            string producer = result.GetString("marka").ToString();
            string model = result.GetString("model").ToString();
            string car = producer + " " + model;


            string range = result.GetString("zasieg").ToString();

            Car.Text = car;

            Range.Text = "Range: " + range + " km";

            connection.Close();


            getBackButton.Click += delegate
            {
                StartActivity(typeof(MainActivity));
            };

            RelativeLayout layout = FindViewById<RelativeLayout>(Resource.Id.relativeLayout1);


            ImageView image = FindViewById<ImageView>(Resource.Id.imageView1);


        }
    }
}