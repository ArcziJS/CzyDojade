using Android.Graphics.Drawables;
using MySqlConnector;
using System.IO;
using System.Net;

namespace CzyDojade
{
    internal class Car
    {
        int id = 11;
        string producer = "My own";
        string model = "car";
        string range = "???";
        string url = "https://i.imgur.com/YHaGvp0.png";
        /// <summary>
        /// Create a new car object with atributes fetched from database.
        /// </summary>
        /// <param name="connection">MySQL connection object for fetching car details purposes.</param>
        /// <param name="carId">Car ID needed in MySQL query string to get certain car.</param>
        public Car(MySqlConnection connection, int carId)
        {


            if (carId < 11)
            {
                var query = connection.CreateCommand();
                string queryString = "SELECT * FROM samochody where id=" + carId;
                query.CommandText = queryString;

                var result = query.ExecuteReader();

                result.Read();

                this.id = carId;
                this.producer = result.GetString("marka").ToString();
                this.model = result.GetString("model").ToString();
                this.range = result.GetString("zasieg").ToString();
                this.url = result.GetString("ikona").ToString();

                result.Close();
            }
        }


        public Drawable DownloadImageDrawable(string imageUrl)
        {
            var webClient = new WebClient();
            var imageBytes = webClient.DownloadData(imageUrl);

            if (imageBytes != null && imageBytes.Length > 0)
            {
                var imageStream = new MemoryStream(imageBytes);
                return Drawable.CreateFromStream(imageStream, null);
            }

            return (Drawable)Resource.Drawable.frame_outline;
        }



        public int GetId() { return id; }
        public string GetProducer() { return producer; }
        public string GetModel() { return model; }
        public string GetRange() { return range; }

        public string GetUrl() { return url; }

    }
}