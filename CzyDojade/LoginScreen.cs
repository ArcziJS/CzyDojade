using Android.App;
using Android.OS;
using Android.Widget;
using MySqlConnector;


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
            #endregion

            #region Functionality of Buttons
            ButtonLogin.Click += delegate
            {
                StartActivity(typeof(MapScreen));  
            };
            ButtonLogin2.Click += delegate
            {
                StartActivity(typeof(RegisterScreen));  
            };
            ButtonLogin6.Click += delegate
            {
                StartActivity(typeof(CarSelector));
            };
            //ButtonLogin3.Click += delegate
            //{
            //    StartActivity(typeof(MainActivity));
            //};
            #endregion

        }
    }
}