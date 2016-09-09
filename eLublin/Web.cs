using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using eLublin.Models;
using eLublin.Web.Models.Db;
using Newtonsoft.Json;

namespace eLublin
{
    public class Service
    {
        public static string ServiceUrl => @"http://elublin.azurewebsites.net/";

        public static HttpClient GetAuthorizedClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            return client;
        }

        public static string AccessToken { get; set; }

        public static async Task<bool> LoginAsync(string email, string password)
        {
            var client = new HttpClient();
            var response =
                await
                    client.PostAsync(new Uri(ServiceUrl + "Token"),
                        new StringContent(
                            string.Format("grant_type=password&username=" + email + "&password=" + password),
                            Encoding.UTF8, "application/x-www-form-urlencoded"));
            if (!response.IsSuccessStatusCode) return false;
            var result = await response.Content.ReadAsStringAsync();
            var tokenResult = JsonConvert.DeserializeObject<TokenResult>(result);
            Service.AccessToken = tokenResult.access_token;
            return true;
        }

        public static async Task<UserInfo> GetUserInfoAsync()
        {
            var client = GetAuthorizedClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            var response = client.GetAsync(ServiceUrl + "api/Account/UserInfo").Result;
            return JsonConvert.DeserializeObject<UserInfo>(await response.Content.ReadAsStringAsync());
        }
    }
}
