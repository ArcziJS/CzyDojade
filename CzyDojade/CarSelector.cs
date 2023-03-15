using Android.App;
using Android.OS;
using Android.Widget;

namespace CzyDojade
{
    [Activity(Label = "CarSelector", Theme = "@style/AppTheme")]
    public class CarSelector : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.car_selection);

            Button getBackButton = FindViewById<Button>(Resource.Id.getBackButton);
            getBackButton.Click += delegate
            {
                StartActivity(typeof(MainActivity));
            };
        }
    }
}