using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Security;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CzyDojade
{
    internal class Car
    {
        int id = 11;
        string producer = "My own";
        string model = "car";
        string range = "???";
        string url;
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
                //this.url = result.GetString("obrazek").ToString();

                result.Close();
            }
        }

        public int GetId() { return id; }
        public string GetProducer() { return producer; }
        public string GetModel() { return model; }
        public string GetRange() { return range; }
        public string GetUrl() { return url; }

    }
}