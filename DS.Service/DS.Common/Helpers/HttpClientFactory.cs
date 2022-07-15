using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DS.Common.Helpers
{
    public class HttpClientFactoryCustom
    {
        public static HttpClient CreateHttpClient(string url, string apiId, string apiSecret)
        {
            HttpClient client;
            client = new HttpClient(new HMACDelegatingHandler(apiId, apiSecret));
            var uri = new Uri(url);
            client.BaseAddress = uri;
            return client;
        }
    }
}
