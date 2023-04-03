using Android.App;
using Android.OS;
using Android.Widget;
using MySqlConnector;
using static Android.Provider.ContactsContract.CommonDataKinds;

namespace CzyDojade
{
    [Activity(Label = "RegisterScreen")]
    public class RegisterScreen : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.register_screen);

            #region MySQL connection
            MySqlConnection connection = new MySqlConnection("Server=db4free.net;Port=3306;Database=czy_dojade;Uid=czy_dojade;Pwd=czy_dojade;");
            connection.Open();
            #endregion

            #region Buttons and EditTexts
            Button ButtonRegister = FindViewById<Button>(Resource.Id.ButtonRegister);
            Button ButtonRegister2 = FindViewById<Button>(Resource.Id.ButtonRegister2);
            EditText Username = FindViewById<EditText>(Resource.Id.Username);
            EditText Email = FindViewById<EditText>(Resource.Id.Email);
            EditText Password = FindViewById<EditText>(Resource.Id.Password);
            EditText ConfirmPassword = FindViewById<EditText>(Resource.Id.ConfirmPassword);
            #endregion

            #region Functionality of Buttons
            ButtonRegister2.Click += delegate
            {
                StartActivity(typeof(LoginScreen));
            };

            ButtonRegister.Click += delegate
            {
                if (Password.Text != ConfirmPassword.Text) // Check if passwords match
                {
                    Toast.MakeText(this, "Passwords don't match", ToastLength.Short).Show();
                    return;
                }

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "INSERT INTO uzytkownik (Login, Email, Haslo) VALUES (@username, @email, @password)";
                command.Parameters.AddWithValue("@username", Username.Text);
                command.Parameters.AddWithValue("@last_name", "");
                command.Parameters.AddWithValue("@email", Email.Text);
                command.Parameters.AddWithValue("@password", Password.Text);
                command.ExecuteNonQuery();
                StartActivity(typeof(MapScreen));
            };
            #endregion

        }
    }
}




