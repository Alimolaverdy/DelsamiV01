//using Newtonsoft.Json;
//using System;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Net.Http.Json;
//using System.Text;
//using System.Threading.Tasks;
//using static LinqToDB.DataProvider.SqlServer.SqlFn;

//namespace Delsami.Plugin.Payment.Tejarat.Helpers
//{
//    public class HttpRequestHelper
//    {
//        public static async Task<string> PostAsync(string url, string value)
//        {
//            using (var httpClient = new HttpClient())
//            {
//                httpClient.BaseAddress = new Uri(url);

//                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
//                using (var response =
//                    await httpClient.PostAsync(url, new StringContent(value, Encoding.UTF8, "application/json")))
//                {
//                    using (var content = response.Content)
//                    {
//                        var result = await content.ReadAsStringAsync();

//                        return result;
//                    }
//                }
//            }


//            //var client = new HttpClient();

//            ////client.BaseAddress = new Uri("http://localhost:55587/");
//            //client.DefaultRequestHeaders.Accept.Clear();
//            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

//            //var newValue = JsonConvert.DeserializeObject(value);

//            //try
//            //{

//            //    var response = await client.PostAsJsonAsync(url, newValue);
//            //    var content = await response.Content.ReadAsStringAsync();

//            //    return content;
//            //}
//            //catch (Exception ex)
//            //{
//            //    throw ex;
//            //}
//        }
//    }
//}
