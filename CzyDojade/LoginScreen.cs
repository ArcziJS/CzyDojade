using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Widget;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using MySqlConnector;
using System;

#pragma warning disable CS0618

namespace CzyDojade
{
    [Activity(Label = "LoginScreen", Theme = "@style/AppTheme")]
    public class LoginScreen : Activity
    {
        private ISharedPreferences prefs;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.login_screen);

            RequestedOrientation = ScreenOrientation.Portrait;

           

            MySqlConnection connection = new MySqlConnection("Server=db4free.net;Port=3306;Database=czy_dojade;Uid=czy_dojade;Pwd=czy_dojade;");
            connection.Open();

            prefs = PreferenceManager.GetDefaultSharedPreferences(this);

            Button ButtonLogin = FindViewById<Button>(Resource.Id.ButtonLogin1);
            Button ButtonLogin2 = FindViewById<Button>(Resource.Id.ButtonLogin2);
            Button ButtonLogin3 = FindViewById<Button>(Resource.Id.ButtonLogin3);
            EditText Email = FindViewById<EditText>(Resource.Id.Email);
            EditText Password = FindViewById<EditText>(Resource.Id.Password);

            ButtonLogin.Click += delegate
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM uzytkownik WHERE Email = @email AND Haslo = @password";
                command.Parameters.AddWithValue("@email", Email.Text);
                command.Parameters.AddWithValue("@password", Password.Text);
                int count = Convert.ToInt32(command.ExecuteScalar());

                if (count == 1)
                {
                    ISharedPreferencesEditor editor = prefs.Edit();
                    editor.PutBoolean("loggedIn", true);
                    editor.PutString("email", Email.Text);
                    editor.Commit();

                    StartActivity(typeof(MapScreen));
                }
                else
                {
                    Toast.MakeText(this, "Wrong email or password. Please try again.", ToastLength.Short).Show();
                }
            };

            ButtonLogin2.Click += delegate
            {
                StartActivity(typeof(RegisterScreen));
            };

            ButtonLogin3.Click += delegate
            {
                StartActivity(typeof(RestorePassword));
            };
        }

        protected override void OnResume()
        {
            base.OnResume();

            bool loggedIn = prefs.GetBoolean("loggedIn", false);
            if (loggedIn)
            {
                StartActivity(typeof(MapScreen));
            }
        }
    }
}
