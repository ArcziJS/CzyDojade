using Android.App;
using Android.OS;
using Android.Widget;

namespace CzyDojade
{
    [Activity(Label = "RegisterScreen")]
    public class RegisterScreen : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.register_screen);

            #region Buttons
            Button ButtonRegister = FindViewById<Button>(Resource.Id.ButtonRegister);
            Button ButtonRegister2 = FindViewById<Button>(Resource.Id.ButtonRegister2);
            #endregion

            #region Functionality of Buttons
            ButtonRegister2.Click += delegate
            {
                StartActivity(typeof(LoginScreen)); 
            };
            #endregion

        }
    }
}