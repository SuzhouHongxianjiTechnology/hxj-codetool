namespace Mint.Database.Connection
{
    using System.Net;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    public static class HttpRequest
    {
        private const string ROOT_URL = @"http://10.158.22.18/api/graph/";

        private static HttpClient Client = new HttpClient();

        public static async Task<T> GetAsync<T>(string url)
        {
            var response = await Client.GetAsync(ROOT_URL + url);
            return await DeserializeAsync<T>(response);
        }

        public static async Task<string> GetStringAsync(string url)
        {
            var response = await Client.GetAsync(ROOT_URL + url);
            return await ParseStringAsync(response);
        }

        public static async Task<T> PostAsync<T>(string url, string paramters)
        {
            var content = new StringContent(paramters);
            var response = await Client.PostAsync(ROOT_URL + url, content);
            return await DeserializeAsync<T>(response);
        }

        private static async Task<string> ParseStringAsync(HttpResponseMessage response)
        {
            if (response?.StatusCode != HttpStatusCode.OK)
            {
                return default;
            }

            var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return document.RootElement.GetProperty("data").ToString();
        }

        private static async Task<T> DeserializeAsync<T>(HttpResponseMessage response)
        {
            if (HttpStatusCode.OK != response?.StatusCode)
            {
                return default;
            }

            var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            var data = document.RootElement.GetProperty("data").ToString();
            if (string.IsNullOrEmpty(data))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
