using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using System.Threading;
using System.Threading.Tasks;

namespace CzyDojade
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private TaskCompletionSource<bool> permissionTaskCompletionSource;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.main_screen);

            RequestedOrientation = ScreenOrientation.Portrait;

            ImageButton selectCarButton = FindViewById<ImageButton>(Resource.Id.SelectCarButton);

            bool isPhoneStatePermissionGranted = ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadPhoneState) == Permission.Granted;
            bool isFineLocationPermissionGranted = ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Permission.Granted;
            bool isExternalStoragePermissionGranted = ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) == Permission.Granted;

            bool shouldRequestPermissions = false;

            if (!isPhoneStatePermissionGranted)
            {
                shouldRequestPermissions = true;
            }

            if (!isFineLocationPermissionGranted)
            {
                shouldRequestPermissions = true;
            }

            if (!isExternalStoragePermissionGranted)
            {
                shouldRequestPermissions = true;
            }

            if (shouldRequestPermissions)
            {
                permissionTaskCompletionSource = new TaskCompletionSource<bool>();

                ActivityCompat.RequestPermissions(this, new string[]
                {
            Manifest.Permission.ReadPhoneState,
            Manifest.Permission.AccessFineLocation,
            Manifest.Permission.ReadExternalStorage
                }, 1);

                await permissionTaskCompletionSource.Task;
            }
            StartActivity(typeof(LoginScreen));
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            permissionTaskCompletionSource?.TrySetResult(true);
        }
    }
}