using Android.App;
using Android.OS;
using MySqlConnector;

namespace CzyDojade
{
    [Activity(Label = "UserSettingsPage")]
    public class UserSettingsPage : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.user_settings_page);

            #region MySQL connection
            MySqlConnection connection = new MySqlConnection("Server=db4free.net;Port=3306;Database=czy_dojade;Uid=czy_dojade;Pwd=czy_dojade;");
            connection.Open();
            #endregion

            #region Buttons etc.

            #endregion

            #region Functionality of Buttons

            #endregion

        }
    }
}




