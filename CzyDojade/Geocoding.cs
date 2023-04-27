﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CzyDojade
{
    public class Geocoding
    {
        static readonly HttpClient client = new HttpClient();
        const string baseUrl = "https://api.mapbox.com/geocoding/v5/mapbox.places/";
        const string accessToken = "pk.eyJ1IjoiY3p5ZG9qYWRlIiwiYSI6ImNsZ2RyZmk2azBiaDUzZXEzY3ZqdXhlOWYifQ.DXoRCQQlVsnu4Ujv3-r7OQ";

        public static async Task<JObject> ForwardGeocodeAsync(string query)
        {
            var response = await client.GetAsync($"{baseUrl}{query}.json?access_token={accessToken}");
            var responseContent = await response.Content.ReadAsStringAsync();
            return JObject.Parse(responseContent);
        }
        public static async Task<JObject> ForwardGeocodeAsync(string query, string[] languages = null)
        {
            // Build the URL for the forward geocoding request
            string url = $"{baseUrl}{query}.json?access_token={accessToken}";

            // Add the language parameter if it is provided
            if (languages != null && languages.Length > 0)
            {
                string languageParameter = string.Join(",", languages);
                url += $"&language={languageParameter}";
            }

            // Perform the forward geocoding request
            var response = await client.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            return JObject.Parse(responseContent);
        }
    }
}