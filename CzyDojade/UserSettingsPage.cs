using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Provider;
using Android.Widget;
using MySqlConnector;
using Newtonsoft.Json;

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
            #endregion

            #region Buttons etc.
            var logoutButton = FindViewById<Button>(Resource.Id.logout);
            var avatarButton = FindViewById<Button>(Resource.Id.change_avatar);
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
            avatarButton.Click += async (sender, e) =>
            {
                // Create a new intent to open the image picker
                var imagePickerIntent = new Intent(Intent.ActionPick);
                imagePickerIntent.SetType("image/*");

                // Start the image picker activity
                StartActivityForResult(imagePickerIntent, 0);
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


