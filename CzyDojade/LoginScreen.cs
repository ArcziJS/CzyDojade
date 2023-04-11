using Android.App;
using Android.Content.Res;
using Android.OS;
using Android.Widget;
using MySqlConnector;
using System;

namespace CzyDojade
{
    [Activity(Label = "LoginScreen", Theme = "@style/AppTheme")]
    public class LoginScreen : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.login_screen);
            #region SQL Connect

            MySqlConnection connection = new MySqlConnection("Server=db4free.net;Port=3306;Database=czy_dojade;Uid=czy_dojade;Pwd=czy_dojade;");
            connection.Open();

            #endregion

            #region Buttons
            Button ButtonLogin = FindViewById<Button>(Resource.Id.ButtonLogin1);
            Button ButtonLogin2 = FindViewById<Button>(Resource.Id.ButtonLogin2);
            Button ButtonLogin3 = FindViewById<Button>(Resource.Id.ButtonLogin3);
            Button ButtonLogin6 = FindViewById<Button>(Resource.Id.ButtonLogin6);
            EditText Email = FindViewById<EditText>(Resource.Id.Email);
            EditText Password = FindViewById<EditText>(Resource.Id.Password);

            #endregion

            #region Functionality of Buttons
            //ButtonLogin.Click += delegate
            //{
            //    StartActivity(typeof(MapScreen));  
            //};

            ButtonLogin.Click += delegate
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM uzytkownik WHERE Email = @email AND Haslo = @password";
                command.Parameters.AddWithValue("@email", Email.Text);
                command.Parameters.AddWithValue("@password", Password.Text);
                int count = Convert.ToInt32(command.ExecuteScalar());

                if (count == 1)
                {
                    StartActivity(typeof(MapScreen));
                }
                else
                {
                    Toast.MakeText(this, "Wrong email or password. Please try again.", ToastLength.Short).Show();
                }

                //MySqlCommand command = connection.CreateCommand();
                //command.CommandText = "SELECT FROM uzytkownik (Email, Haslo) VALUES (@textEmailAddress, @login_Password)";
                //command.Parameters.AddWithValue("@textEmailAddress", textEmailAddress.Text);
                //command.Parameters.AddWithValue("@login_Password", login_Password.Text);

                //int rows = Convert.ToInt32(command.ExecuteScalar());

                //if (rows == 1)
                //{
                //    StartActivity(typeof(MapScreen));
                //}
                //else
                //{
                //    Toast.MakeText(this, "Try again wrong email or password!", ToastLength.Short).Show();
                //}


            };
            
            //ButtonLogin6.Click += delegate
            //{
            //    StartActivity(typeof(CarSelector));
            //};
         
            #endregion

        }
    }
}