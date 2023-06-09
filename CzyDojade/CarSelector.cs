using Android.App;
using Android.OS;
using Android.Widget;
using MySqlConnector;
using System.Threading.Tasks;
using Android.Content;
using Android.Preferences;
using Android.Content.PM;

namespace CzyDojade
{
    [Activity(Label = "CarSelector", Theme = "@style/AppTheme")]
    public class CarSelector : Activity
    {
        private MySqlConnection connection;
        private ImageView[] imageViews;
        private TextView[] producerModelTextViews;
        private TextView[] rangeTextViews;
        private Button[] selectCarButtons;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.car_selection);

            RequestedOrientation = ScreenOrientation.Portrait;

            InitializeViews();
            InitializeConnection();

            LoadCarsAsync();
        }

        private void InitializeViews()
        {
            // Initialize arrays
            imageViews = new ImageView[11];
            producerModelTextViews = new TextView[11];
            rangeTextViews = new TextView[11];
            selectCarButtons = new Button[11];

            // Assign TextViews, ImageViews, and Buttons
            for (int i = 0; i < 11; i++)
            {
                producerModelTextViews[i] = FindViewById<TextView>(GetTextViewCarId(i + 1));
                rangeTextViews[i] = FindViewById<TextView>(GetTextViewRangeId(i + 1));
                imageViews[i] = FindViewById<ImageView>(GetImageViewId(i + 1));
                selectCarButtons[i] = FindViewById<Button>(GetButtonSelectCarId(i + 1));
                selectCarButtons[i].Click += delegate
                {
                    InitializeConnection();
                    StartActivity(typeof(MapScreen));
                };
            }

            int GetTextViewCarId(int carIndex) => (int)typeof(Resource.Id).GetField($"textViewCar{carIndex}").GetValue(null);
            int GetTextViewRangeId(int carIndex) => (int)typeof(Resource.Id).GetField($"textViewRange{carIndex}").GetValue(null);
            int GetImageViewId(int carIndex) => (int)typeof(Resource.Id).GetField($"imageView{carIndex}").GetValue(null);
            int GetButtonSelectCarId(int carIndex) => (int)typeof(Resource.Id).GetField($"buttonSelectCar{carIndex}").GetValue(null);
        }

        private void InitializeConnection()
        {
            this.connection = new MySqlConnection("Server=db4free.net;Port=3306;Database=czy_dojade;Uid=czy_dojade;Pwd=czy_dojade;");
            this.connection.Open();
        }

        private async Task LoadCarsAsync()
        {
            Car[] cars = new Car[11];

            // Load car details asynchronously
            for (int i = 0; i < 11; i++)
            {
                cars[i] = await new Car().LoadCarDetailsAsync(this.connection, i + 1);
            }

            // Display car details
            for (int i = 0; i < 11; i++)
            {
                Car car = cars[i];
                if (car != null)
                {
                    imageViews[i].SetImageDrawable(await car.DownloadImageDrawableAsync(car.GetUrl()));
                    producerModelTextViews[i].Text = $"{car.GetProducer()} {car.GetModel()}";
                    rangeTextViews[i].Text = $"Range: {car.GetRange()} km";
                    selectCarButtons[i].Visibility = Android.Views.ViewStates.Visible;

                    int carId = car.GetId();
                    selectCarButtons[i].Click += async delegate
                    {
                        // Get the user email from Android preferences
                        ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                        string userEmail = prefs.GetString("email", "");

                        // Query the database to find the user ID based on the email
                        var query = connection.CreateCommand();
                        query.CommandText = $"SELECT id FROM uzytkownik WHERE Email = '{userEmail}'";
                        int userId = 0; // Default value if user ID is not found

                        using (var reader = await query.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                userId = reader.GetInt32(0);
                            }
                        }

                        // Update the uzytkownik_samochody table with the user ID and chosen car ID
                        var updateQuery = connection.CreateCommand();
                        string updateQueryString = $"INSERT INTO uzytkownicy_samochody (uzytkownik_id, samochod_id) VALUES ({userId}, {carId})";
                        updateQuery.CommandText = updateQueryString;
                        await updateQuery.ExecuteNonQueryAsync();

                        // Start the Settings activity
                        StartActivity(typeof(UserSettingsPage));
                    };

                }
            }

            connection.Close();
        }

    }
}
