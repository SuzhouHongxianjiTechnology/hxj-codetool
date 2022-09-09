namespace Mint.Database.Remote
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Mint.Database.Configurations;

    public static class HttpRequest
    {
        // private const string ROOT_URL = @"http://10.158.22.18/api/graph/";
        private static string ROOT_URL = DatabaseSettings.Settings.ProductionURL;

        public static async Task<T> GetAsync<T>(string url)
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(ROOT_URL + url);
                return await DeserializeAsync<T>(response);
            }
        }

        public static async Task<string> GetAsync(string url)
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(ROOT_URL + url);
                return await ParseStringAsync(response);
            }
        }

        public static async Task<string> GetStirngAsync(string url)
        {
            using (var client = new HttpClient())
            {
                return await client.GetStringAsync(ROOT_URL + url);
            }
        }

        public static async Task<T> PostAsync<T>(string url, string paramters)
        {
            using (var client = new HttpClient())
            {
                StringContent content = new StringContent(paramters);
                HttpResponseMessage response = await client.PostAsync(ROOT_URL + url, content);
                return await DeserializeAsync<T>(response);
            }
        }

        private static async Task<string> ParseStringAsync(HttpResponseMessage response)
        {
            if (response?.StatusCode != HttpStatusCode.OK)
            {
                return string.Empty;
            }

            JsonDocument document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return document.RootElement.GetProperty("data").ToString();
        }

        private static async Task<T> DeserializeAsync<T>(HttpResponseMessage response)
        {
            if (HttpStatusCode.OK != response?.StatusCode)
            {
                throw new Exception($"Response error, code: '{response?.StatusCode}'.");
            }

            JsonDocument document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            string data = document.RootElement.GetProperty("data").ToString();
            if (string.IsNullOrEmpty(data))
            {
                throw new Exception("Response json is missing property: 'data'.");
            }

            return JsonSerializer.Deserialize<T>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
