using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CzyDojade
{
    public class Geocoding
    {
        static readonly HttpClient client = new HttpClient();
        const string baseUrl = "https://api.mapbox.com/geocoding/v5/mapbox.places/";
        const string accessToken = "pk.eyJ1IjoiY3p5ZG9qYWRlIiwiYSI6ImNsZ3k5MjBscTA3NTUzZnBlZ3VoYXYxMGIifQ.Gh80YFg9RRgTbG9WbxvPPQ";

        public static async Task<JObject> ForwardGeocodeAsync(string query)
        {
            var response = await client.GetAsync($"{baseUrl}{query}.json?access_token={accessToken}");
            var responseContent = await response.Content.ReadAsStringAsync();
            return JObject.Parse(responseContent);
        }
        public static async Task<JObject> ForwardGeocodeAsync(string query, string[] languages = null)
        {
            string url = $"{baseUrl}{query}.json?access_token={accessToken}";

            if (languages != null && languages.Length > 0)
            {
                string languageParameter = string.Join(",", languages);
                url += $"&language={languageParameter}";
            }

            var response = await client.GetAsync(url);
            var responseContent = await response.Content.ReadAsStringAsync();
            return JObject.Parse(responseContent);
        }
    }
}