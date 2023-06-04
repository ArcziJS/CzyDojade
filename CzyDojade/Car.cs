using Android.Graphics.Drawables;
using MySqlConnector;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace CzyDojade
{
    internal class Car
    {
        private int id = 11;
        private string producer = "My own";
        private string model = "car";
        private string range = "???";
        private string url = "https://i.imgur.com/YHaGvp0.png";

        public async Task<Car> LoadCarDetailsAsync(MySqlConnection connection, int carId)
        {
            if (carId < 11)
            {
                var query = connection.CreateCommand();
                string queryString = "SELECT * FROM samochody WHERE id = " + carId;
                query.CommandText = queryString;

                using (var result = await query.ExecuteReaderAsync())
                {
                    if (await result.ReadAsync())
                    {
                        this.id = carId;
                        this.producer = result.GetString("marka");
                        this.model = result.GetString("model");
                        this.range = result.GetString("zasieg");
                        this.url = result.GetString("ikona");
                    }
                }
            }

            return this;
        }

        public async Task<Drawable> DownloadImageDrawableAsync(string imageUrl)
        {
            using (var webClient = new WebClient())
            {
                var imageBytes = await webClient.DownloadDataTaskAsync(imageUrl);

                if (imageBytes != null && imageBytes.Length > 0)
                {
                    using (var imageStream = new MemoryStream(imageBytes))
                    {
                        return Drawable.CreateFromStream(imageStream, null);
                    }
                }
            }

            // Return a default drawable if the image download fails
            return (Drawable)Resource.Drawable.frame_outline;
        }

        public int GetId() { return id; }
        public string GetProducer() { return producer; }
        public string GetModel() { return model; }
        public string GetRange() { return range; }
        public string GetUrl() { return url; }
    }
}
