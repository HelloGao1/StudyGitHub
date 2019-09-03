using System;
using System.Net.Http;

namespace Partner.Data.Integration.Utils
{
    public class APIHelper
    {
        /// <summary>
        /// Call Fhir Server API
        /// </summary>
        /// <param name="token"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string CallFhirApi(string token, string url)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/xml");
            client.DefaultRequestHeaders.Add("Authorization", token);

            var json = client.GetStringAsync(url).GetAwaiter().GetResult();
            return json;
        }
    }
}