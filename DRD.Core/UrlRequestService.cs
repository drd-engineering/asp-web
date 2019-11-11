using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net;

namespace DRD.Service
{
    public class UrlRequestService
    {
        public JObject GetRequest(string uri, string param)
        {
            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                client.BaseAddress = new Uri(uri);

                HttpResponseMessage response = client.GetAsync(param).Result;
                response.EnsureSuccessStatusCode();
                string result = response.Content.ReadAsStringAsync().Result;
                if (result.Equals("null") || result == null)
                {
                    result = "{}";
                }
                JObject json = JObject.Parse(result);
                return json;
            }
        }

        public JObject GetRequest<T>(string uri, string param, T obj)
        {
            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                client.BaseAddress = new Uri(uri);

                //HttpResponseMessage response = client.GetAsync(param).Result;
                HttpResponseMessage response = client.PostAsJsonAsync(param, obj).Result;
                response.EnsureSuccessStatusCode();
                string result = response.Content.ReadAsStringAsync().Result;
                if (result.Equals("null") || result == null)
                {
                    result = "{}";
                }
                JObject json = JObject.Parse(result);
                return json;
            }
        }

        public JArray GetRequests(string uri, string param)
        {
            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                client.BaseAddress = new Uri(uri);

                HttpResponseMessage response = client.GetAsync(param).Result;
                response.EnsureSuccessStatusCode();
                string result = response.Content.ReadAsStringAsync().Result;
                if (result.Equals("null") || result == null)
                {
                    result = "[{}]";
                }
                JArray jsons = JsonConvert.DeserializeObject<JArray>(result);
                return jsons;
            }
        }
    }
}
