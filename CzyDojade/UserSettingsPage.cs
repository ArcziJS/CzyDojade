using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Widget;
using MySqlConnector;
using System;

namespace CzyDojade
{
    [Activity(Label = "UserSettingsPage")]
    public class UserSettingsPage : Activity
    {
        private string userEmail;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.user_settings_page);

            // Retrieve the email address of the logged-in user from shared preferences
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            userEmail = prefs.GetString("email", "");

            #region MySQL connection
            MySqlConnection connection = new MySqlConnection("Server=db4free.net;Port=3306;Database=czy_dojade;Uid=czy_dojade;Pwd=czy_dojade;");
            connection.Open();
            #endregion

            #region Display user data
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT Login FROM uzytkownik WHERE Email = @email";
            command.Parameters.AddWithValue("@email", userEmail);
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var nameTextView = FindViewById<TextView>(Resource.Id.user_name);
                    nameTextView.Text = $"{reader.GetString(0)}";

                }
            }

            var emailTextView = FindViewById<TextView>(Resource.Id.email);
            emailTextView.Text = $"{userEmail}";
            #endregion

            #region Buttons etc.
            var logoutButton = FindViewById<Button>(Resource.Id.logout);
            #endregion

            #region Functionality of Buttons
            logoutButton.Click += (sender, e) =>
            {
                // Clear the shared preferences to end the user's session
                ISharedPreferencesEditor editor = prefs.Edit();
                editor.Clear();
                editor.Commit();

                // Start the LoginScreen activity
                var intent = new Intent(this, typeof(LoginScreen));
                intent.AddFlags(ActivityFlags.ClearTop);
                StartActivity(intent);
            };
            #endregion
        }
    }
}
