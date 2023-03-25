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

            #region TextViews car producer + model

            TextView TextViewCar1 = FindViewById<TextView>(Resource.Id.textViewCar1);
            TextView TextViewCar2 = FindViewById<TextView>(Resource.Id.textViewCar2);
            TextView TextViewCar3 = FindViewById<TextView>(Resource.Id.textViewCar3);
            TextView TextViewCar4 = FindViewById<TextView>(Resource.Id.textViewCar4);
            TextView TextViewCar5 = FindViewById<TextView>(Resource.Id.textViewCar5);
            TextView TextViewCar6 = FindViewById<TextView>(Resource.Id.textViewCar6);
            TextView TextViewCar7 = FindViewById<TextView>(Resource.Id.textViewCar7);
            TextView TextViewCar8 = FindViewById<TextView>(Resource.Id.textViewCar8);
            TextView TextViewCar9 = FindViewById<TextView>(Resource.Id.textViewCar9);
            TextView TextViewCar10 = FindViewById<TextView>(Resource.Id.textViewCar10);
            TextView TextViewCar11 = FindViewById<TextView>(Resource.Id.textViewCar11);

            #endregion

            #region TextViews car range

            TextView TextViewRange1 = FindViewById<TextView>(Resource.Id.textViewRange1);
            TextView TextViewRange2 = FindViewById<TextView>(Resource.Id.textViewRange2);
            TextView TextViewRange3 = FindViewById<TextView>(Resource.Id.textViewRange3);
            TextView TextViewRange4 = FindViewById<TextView>(Resource.Id.textViewRange4);
            TextView TextViewRange5 = FindViewById<TextView>(Resource.Id.textViewRange5);
            TextView TextViewRange6 = FindViewById<TextView>(Resource.Id.textViewRange6);
            TextView TextViewRange7 = FindViewById<TextView>(Resource.Id.textViewRange7);
            TextView TextViewRange8 = FindViewById<TextView>(Resource.Id.textViewRange8);
            TextView TextViewRange9 = FindViewById<TextView>(Resource.Id.textViewRange9);
            TextView TextViewRange10 = FindViewById<TextView>(Resource.Id.textViewRange10);
            TextView TextViewRange11 = FindViewById<TextView>(Resource.Id.textViewRange11);

            #endregion

            #region Buttons car select

            Button ButtonSelectCar1 = FindViewById<Button>(Resource.Id.buttonSelectCar1);
            Button ButtonSelectCar2 = FindViewById<Button>(Resource.Id.buttonSelectCar2);
            Button ButtonSelectCar3 = FindViewById<Button>(Resource.Id.buttonSelectCar3);
            Button ButtonSelectCar4 = FindViewById<Button>(Resource.Id.buttonSelectCar4);
            Button ButtonSelectCar5 = FindViewById<Button>(Resource.Id.buttonSelectCar5);
            Button ButtonSelectCar6 = FindViewById<Button>(Resource.Id.buttonSelectCar6);
            Button ButtonSelectCar7 = FindViewById<Button>(Resource.Id.buttonSelectCar7);
            Button ButtonSelectCar8 = FindViewById<Button>(Resource.Id.buttonSelectCar8);
            Button ButtonSelectCar9 = FindViewById<Button>(Resource.Id.buttonSelectCar9);
            Button ButtonSelectCar10 = FindViewById<Button>(Resource.Id.buttonSelectCar10);
            Button ButtonSelectCar11 = FindViewById<Button>(Resource.Id.buttonSelectCar11);


            #endregion

            #region MySQL connection

            MySqlConnection connection = new MySqlConnection("Server=db4free.net;Port=3306;Database=czy_dojade;Uid=czy_dojade;Pwd=czy_dojade;");
            connection.Open();

            #endregion

            #region Cars

            Car Car1 = new Car(connection, 1);
            Car Car2 = new Car(connection, 2);
            Car Car3 = new Car(connection, 3);
            Car Car4 = new Car(connection, 4);
            Car Car5 = new Car(connection, 5);
            Car Car6 = new Car(connection, 6);
            Car Car7 = new Car(connection, 7);
            Car Car8 = new Car(connection, 8);
            Car Car9 = new Car(connection, 9);
            Car Car10 = new Car(connection, 10);
            Car Car11 = new Car(connection, 11);


            #endregion

            #region display cars

            TextViewCar1.Text = Car1.GetProducer() + " " + Car1.GetModel();
            TextViewRange1.Text = "Range: " + Car1.GetRange() + " km";

            TextViewCar2.Text = Car2.GetProducer() + " " + Car2.GetModel();
            TextViewRange2.Text = "Range: " + Car2.GetRange() + " km";

            TextViewCar3.Text = Car3.GetProducer() + " " + Car3.GetModel();
            TextViewRange3.Text = "Range: " + Car3.GetRange() + " km";

            TextViewCar4.Text = Car4.GetProducer() + " " + Car4.GetModel();
            TextViewRange4.Text = "Range: " + Car4.GetRange() + " km";

            TextViewCar5.Text = Car5.GetProducer() + " " + Car5.GetModel();
            TextViewRange5.Text = "Range: " + Car5.GetRange() + " km";

            TextViewCar6.Text = Car6.GetProducer() + " " + Car6.GetModel();
            TextViewRange6.Text = "Range: " + Car6.GetRange() + " km";

            TextViewCar7.Text = Car7.GetProducer() + " " + Car7.GetModel();
            TextViewRange7.Text = "Range: " + Car7.GetRange() + " km";

            TextViewCar8.Text = Car8.GetProducer() + " " + Car8.GetModel();
            TextViewRange8.Text = "Range: " + Car8.GetRange() + " km";

            TextViewCar9.Text = Car9.GetProducer() + " " + Car9.GetModel();
            TextViewRange9.Text = "Range: " + Car9.GetRange() + " km";

            TextViewCar10.Text = Car10.GetProducer() + " " + Car10.GetModel();
            TextViewRange10.Text = "Range: " + Car10.GetRange() + " km";

            TextViewCar11.Text = Car11.GetProducer() + " " + Car11.GetModel();
            TextViewRange11.Text = "Range: " + Car11.GetRange() + " km";

            #endregion

            #region Buttons

            ButtonSelectCar1.Click += delegate
            {
                StartActivity(typeof(MainActivity));
            };

            ButtonSelectCar2.Click += delegate
            {
                StartActivity(typeof(MainActivity));
            };

            ButtonSelectCar3.Click += delegate
            {
                StartActivity(typeof(MainActivity));
            };

            ButtonSelectCar4.Click += delegate
            {
                StartActivity(typeof(MainActivity));
            };

            ButtonSelectCar5.Click += delegate
            {
                StartActivity(typeof(MainActivity));
            };

            ButtonSelectCar6.Click += delegate
            {
                StartActivity(typeof(MainActivity));
            };

            ButtonSelectCar7.Click += delegate
            {
                StartActivity(typeof(MainActivity));
            };

            ButtonSelectCar8.Click += delegate
            {
                StartActivity(typeof(MainActivity));
            };

            ButtonSelectCar9.Click += delegate
            {
                StartActivity(typeof(MainActivity));
            };

            ButtonSelectCar10.Click += delegate
            {
                StartActivity(typeof(MainActivity));
            };

            ButtonSelectCar11.Click += delegate
            {
                StartActivity(typeof(MainActivity));
            };

            #endregion

            connection.Close();
        }
    }
}