using Android.App;
using Android.OS;

namespace CzyDojade
{
    [Activity(Label = "Activity1")]
    public class MapScreen : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.map_screen);
        }
    }
}