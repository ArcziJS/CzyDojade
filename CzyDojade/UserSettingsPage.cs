﻿using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using MySqlConnector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
//using Windows.UI.Xaml.Controls;

#pragma warning disable CS0618

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
            command.CommandText = "SELECT Login, Avatar FROM uzytkownik WHERE Email = @Email";
            command.Parameters.AddWithValue("@Email", userEmail);
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var nameTextView = FindViewById<TextView>(Resource.Id.user_name);
                    nameTextView.Text = $"{reader.GetString(0)}";

                    var avatarImageView = FindViewById<ImageView>(Resource.Id.avatar);

                    if (!reader.IsDBNull(1))
                    {
                        var avatarUri = new Uri(reader.GetString(1));
                        LoadAvatarFromUriAsync(avatarUri, avatarImageView);
                    }
                }
            }


            var emailTextView = FindViewById<TextView>(Resource.Id.email);
            emailTextView.Text = $"{userEmail}";

            // Retrieve the user ID from the database using the user's email
            int userId;
            using (var getUserIdCommand = connection.CreateCommand())
            {
                getUserIdCommand.CommandText = "SELECT id FROM uzytkownik WHERE Email = @Email";
                getUserIdCommand.Parameters.AddWithValue("@Email", userEmail);
                userId = Convert.ToInt32(getUserIdCommand.ExecuteScalar());
            }

            // Retrieve the matching car IDs for the user from the database
            List<int> carIds = new List<int>();
            using (var getCarIdsCommand = connection.CreateCommand())
            {
                getCarIdsCommand.CommandText = "SELECT samochod_id FROM uzytkownicy_samochody WHERE uzytkownik_id = @UserId";
                getCarIdsCommand.Parameters.AddWithValue("@UserId", userId);
                using (var reader = getCarIdsCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        carIds.Add(reader.GetInt32(0));
                    }
                }
            }

            // Get the car_section LinearLayout from the layout
            LinearLayout carSection = FindViewById<LinearLayout>(Resource.Id.car_section);

            // Retrieve the active car ID from the database
            int activeCarId;
            using (var getActiveCarIdCommand = connection.CreateCommand())
            {
                getActiveCarIdCommand.CommandText = "SELECT samochod_id FROM uzytkownicy_samochody WHERE uzytkownik_id = @UserId AND is_active = true";
                getActiveCarIdCommand.Parameters.AddWithValue("@UserId", userId);
                activeCarId = Convert.ToInt32(getActiveCarIdCommand.ExecuteScalar());
            }

            // Iterate over the list of car IDs and retrieve the car data from the database
            foreach (int carId in carIds)
            {
                using (var getCarDataCommand = connection.CreateCommand())
                {
                    getCarDataCommand.CommandText = "SELECT marka, model, zasieg, ikona FROM samochody WHERE id = @CarId";
                    getCarDataCommand.Parameters.AddWithValue("@CarId", carId);
                    using (var reader = getCarDataCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Create a new LinearLayout to hold car details and options
                            LinearLayout carLayout = new LinearLayout(this);
                            carLayout.Orientation = Orientation.Vertical;
                            carLayout.LayoutParameters = new LinearLayout.LayoutParams(
                                ViewGroup.LayoutParams.MatchParent,
                                ViewGroup.LayoutParams.WrapContent
                            );
                            carLayout.SetPadding(16, 16, 16, 16);
                            carLayout.SetBackgroundColor(Color.Rgb(44, 62, 80));

                            // Create a LinearLayout to hold the car information and options
                            LinearLayout carInfoOptionsLayout = new LinearLayout(this);
                            carInfoOptionsLayout.LayoutParameters = new LinearLayout.LayoutParams(
                                ViewGroup.LayoutParams.MatchParent,
                                ViewGroup.LayoutParams.WrapContent
                            );
                            carInfoOptionsLayout.Orientation = Orientation.Horizontal;
                            carInfoOptionsLayout.SetPadding(16, 0, 16, 0);

                            // Create a LinearLayout to hold the car information
                            LinearLayout carInfoLayout = new LinearLayout(this);
                            carInfoLayout.LayoutParameters = new LinearLayout.LayoutParams(
                                ViewGroup.LayoutParams.WrapContent,
                                ViewGroup.LayoutParams.WrapContent
                            );
                            carInfoLayout.Orientation = Orientation.Vertical;

                            // Create a TextView for the car make and model
                            TextView carMakeModel = new TextView(this);
                            carMakeModel.LayoutParameters = new LinearLayout.LayoutParams(
                                ViewGroup.LayoutParams.WrapContent,
                                ViewGroup.LayoutParams.WrapContent
                            );
                            carMakeModel.Text = $"{reader.GetString(0)} {reader.GetString(1)}";
                            carMakeModel.SetTextAppearance(this, Android.Resource.Style.TextAppearanceMedium);
                            carMakeModel.SetTextColor(Color.White);
                            carMakeModel.SetTypeface(carMakeModel.Typeface, TypefaceStyle.Bold);
                            carInfoLayout.AddView(carMakeModel);

                            // Create a TextView for the car range
                            TextView carRange = new TextView(this);
                            carRange.LayoutParameters = new LinearLayout.LayoutParams(
                                ViewGroup.LayoutParams.WrapContent,
                                ViewGroup.LayoutParams.WrapContent
                            );
                            carRange.Text = $"Zasięg: {reader.GetString(2)} km";
                            carRange.SetTextAppearance(this, Android.Resource.Style.TextAppearanceSmall);
                            carRange.SetTextColor(Color.White);
                            carInfoLayout.AddView(carRange);

                            carInfoOptionsLayout.AddView(carInfoLayout);

                            // Create a LinearLayout for the options menu
                            LinearLayout optionsLayout = new LinearLayout(this);
                            optionsLayout.LayoutParameters = new LinearLayout.LayoutParams(
                                ViewGroup.LayoutParams.WrapContent,
                                ViewGroup.LayoutParams.WrapContent
                            );
                            optionsLayout.Orientation = Orientation.Horizontal;
                            optionsLayout.SetGravity(GravityFlags.End | GravityFlags.CenterVertical);

                            // Create the triple-dot menu button
                            Button optionsButton = new Button(this);
                            optionsButton.LayoutParameters = new LinearLayout.LayoutParams(
                                ViewGroup.LayoutParams.WrapContent,
                                ViewGroup.LayoutParams.WrapContent
                            );
                            optionsButton.Text = "⋮";
                            optionsButton.SetTextAppearance(this, Android.Resource.Style.TextAppearanceLarge);
                            optionsButton.SetTextColor(Color.White);
                            optionsButton.Background = null;

                            bool isActiveCar = false;

                            // Add a green indication for the currently active car
                            if (carId == activeCarId)
                            {
                                isActiveCar = true;
                                carLayout.SetBackgroundColor(Color.Rgb(0, 128, 0));
                            }
                            else
                            {
                                isActiveCar = false;
                                carLayout.SetBackgroundColor(Color.Rgb(44, 62, 80));
                            }

                            // Add click event handler for the options button
                            optionsButton.Click += (sender, e) =>
                            {
                                // Create a popup menu
                                PopupMenu popupMenu = new PopupMenu(this, optionsButton);
                                popupMenu.MenuInflater.Inflate(Resource.Menu.car_options_menu, popupMenu.Menu);

                                // Set click event handler for menu items
                                popupMenu.MenuItemClick += (s, args) =>
                                {
                                    switch (args.Item.ItemId)
                                    {
                                        case Resource.Id.menu_set_active:
                                            // Handle set as active option
                                            SetCarAsActive(carId);
                                            StartActivity(typeof(UserSettingsPage));
                                            break;
                                        case Resource.Id.menu_delete:
                                            // Handle delete option
                                            DeleteCar(carId); // Pass the carId to the DeleteCar method
                                            StartActivity(typeof(UserSettingsPage));
                                            break;
                                    }
                                };

                                // Show the popup menu
                                popupMenu.Show();
                            };

                            // Add the options button to the options layout
                            optionsLayout.AddView(optionsButton);

                            carInfoOptionsLayout.AddView(optionsLayout);

                            // Add margin to the carLayout
                            LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(
                                ViewGroup.LayoutParams.MatchParent,
                                ViewGroup.LayoutParams.WrapContent
                            );
                            layoutParams.SetMargins(32, 0, 32, 16);
                            carLayout.LayoutParameters = layoutParams;

                            // Add the carInfoOptionsLayout to the carLayout
                            carLayout.AddView(carInfoOptionsLayout);

                            // Add the carLayout to the car_section LinearLayout
                            carSection.AddView(carLayout);
                        }
                    }
                }
            }

            void DeleteCar(int carId)
            {
                MySqlConnection connection = new MySqlConnection("Server=db4free.net;Port=3306;Database=czy_dojade;Uid=czy_dojade;Pwd=czy_dojade;");
                connection.Open();

                using (var deleteCommand = connection.CreateCommand())
                {
                    deleteCommand.CommandText = "DELETE FROM uzytkownicy_samochody WHERE uzytkownik_id = @UserId AND samochod_id = @CarId";
                    deleteCommand.Parameters.AddWithValue("@UserId", userId);
                    deleteCommand.Parameters.AddWithValue("@CarId", carId);
                    deleteCommand.ExecuteNonQuery();
                }
            }

            // Set the selected car as active and deactivate other cars owned by the user
            void SetCarAsActive(int carId)
            {
                #region MySQL connection
                MySqlConnection connection = new MySqlConnection("Server=db4free.net;Port=3306;Database=czy_dojade;Uid=czy_dojade;Pwd=czy_dojade;");
                connection.Open();
                #endregion

                // Retrieve the user ID from the database using the user's email
                int userId;
                using (var getUserIdCommand = connection.CreateCommand())
                {
                    getUserIdCommand.CommandText = "SELECT id FROM uzytkownik WHERE Email = @Email";
                    getUserIdCommand.Parameters.AddWithValue("@Email", userEmail);
                    userId = Convert.ToInt32(getUserIdCommand.ExecuteScalar());
                }

                // Set the is_active column to false for all cars owned by the user
                using (var deactivateCarsCommand = connection.CreateCommand())
                {
                    deactivateCarsCommand.CommandText = "UPDATE uzytkownicy_samochody SET is_active = false WHERE uzytkownik_id = @UserId";
                    deactivateCarsCommand.Parameters.AddWithValue("@UserId", userId);
                    deactivateCarsCommand.ExecuteNonQuery();
                }

                // Set the is_active column to true for the selected car
                using (var setActiveCarCommand = connection.CreateCommand())
                {
                    setActiveCarCommand.CommandText = "UPDATE uzytkownicy_samochody SET is_active = true WHERE uzytkownik_id = @UserId AND samochod_id = @CarId";
                    setActiveCarCommand.Parameters.AddWithValue("@UserId", userId);
                    setActiveCarCommand.Parameters.AddWithValue("@CarId", carId);
                    setActiveCarCommand.ExecuteNonQuery();
                }
            }


            #endregion

            #region Buttons etc.
            var logoutButton = FindViewById<Button>(Resource.Id.logout);
            var avatarButton = FindViewById<Button>(Resource.Id.change_avatar);
            var mapButton = FindViewById<ImageButton>(Resource.Id.menu_map);
            var carSelectorButton = FindViewById<Button>(Resource.Id.select_car);
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

            // Define the click event handler for the 'change avatar' button
            avatarButton.Click += (sender, e) =>
            {
                // Create a new intent to open the image picker
                var imagePickerIntent = new Intent(Intent.ActionPick);
                imagePickerIntent.SetType("image/*");

                // Start the image picker activity
                StartActivityForResult(imagePickerIntent, 0);
            };

            // Define the click event handler for the 'map' button
            mapButton.Click += (sender, e) =>
            {
                int activeCarRange;
                //using (var getActiveCarIdCommand = connection.CreateCommand())
                //{
                //    getActiveCarIdCommand.CommandText = "SELECT samochod_id FROM uzytkownicy_samochody WHERE uzytkownik_id = @UserId AND is_active = true";
                //    getActiveCarIdCommand.Parameters.AddWithValue("@UserId", userId);
                //    activeCarId = Convert.ToInt32(getActiveCarIdCommand.ExecuteScalar());
                //}
                using (var getCarRangeCommand = connection.CreateCommand())
                {
                    getCarRangeCommand.CommandText = "SELECT zasieg FROM samochody WHERE id = @CarId";
                    getCarRangeCommand.Parameters.AddWithValue("@CarId", activeCarId);
                    activeCarRange = Convert.ToInt32(getCarRangeCommand.ExecuteScalar());
                }

                    ISharedPreferencesEditor editor = prefs.Edit();
                editor.PutInt("range", activeCarRange);
                editor.Commit();
                // Start the Map activity
                var intent = new Intent(this, typeof(MapScreen));
                StartActivity(intent);
                OverridePendingTransition(0, 0);
            };

            carSelectorButton.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(CarSelector));
                StartActivity(intent);
                OverridePendingTransition(0, 0);
            };
            #endregion
        }

        private async Task<Android.Graphics.Bitmap> LoadImageFromUriAsync(Uri uri)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(uri);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to load image from {uri}. Status code: {response.StatusCode}");
                }

                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    return await BitmapFactory.DecodeStreamAsync(stream);
                }
            }
        }


        private async void LoadAvatarFromUriAsync(Uri uri, ImageView imageView)
        {
            try
            {
                var bitmap = await LoadImageFromUriAsync(uri);
                imageView.SetImageBitmap(bitmap);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        // Handle the result of the image picker activity
        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok && requestCode == 0)
            {
                // Get the selected image URI
                var imageUri = data.Data;

                try
                {
                    // Get the bitmap from the selected image URI
                    var bitmap = BitmapFactory.DecodeStream(ContentResolver.OpenInputStream(imageUri));

                    // Upload the image to Imgur
                    var imgurLink = await UploadImageToImgur(bitmap);

                    // Save the Imgur link to your PHP database for the current user
                    SaveImgurLinkForCurrentUser(imgurLink);

                    // Update the avatar view with the new bitmap
                    var avatarImageView = FindViewById<ImageView>(Resource.Id.avatar);
                    avatarImageView.SetImageBitmap(bitmap);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        // Uploads the given image bitmap to Imgur and returns the Imgur link for the image
        private async Task<string> UploadImageToImgur(Android.Graphics.Bitmap bitmap)
        {
            // Convert the bitmap to a byte array
            using (var stream = new MemoryStream())
            {
                bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, stream);
                var byteArray = stream.ToArray();

                // Build the HTTP request to upload the image to Imgur
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Client-ID 8de2d2b8282b56b" + " " + "Bearer fa5fa8a59abcd7cc68f6c1bb9e640106ece18dc8");
                    using (var content = new MultipartFormDataContent())
                    {
                        content.Add(new ByteArrayContent(byteArray), "image", "image.jpg");

                        var response = await httpClient.PostAsync("https://api.imgur.com/3/image", content);
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new Exception($"Failed to upload image to Imgur. Status code: {response.StatusCode}");
                        }

                        // Parse the response to get the Imgur link for the uploaded image
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<ImgurUploadResponse>(responseContent);
                        var imgurLink = responseData.Data.Link;

                        return imgurLink;
                    }
                }
            }
        }



        // Saves the given Imgur link as the user's avatar in your PHP database
        private void SaveImgurLinkForCurrentUser(string imgurLink)
        {
            MySqlConnection connection = new MySqlConnection("Server=db4free.net;Port=3306;Database=czy_dojade;Uid=czy_dojade;Pwd=czy_dojade;");
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE uzytkownik SET Avatar = @avatarLink WHERE Email = @userEmail";
            command.Parameters.AddWithValue("@avatarLink", imgurLink);
            command.Parameters.AddWithValue("@userEmail", userEmail);
            command.ExecuteNonQuery();
        }


        // Definition of the ImgurUploadResponse class used to parse the Imgur API response
        public class ImgurUploadResponse
        {
            public ImgurUploadData Data { get; set; }
        }

        public class ImgurUploadData
        {
            public string Link { get; set; }
        }
    }
}


