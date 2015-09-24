using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.SDK.Rest
{
    public interface IRestClient
    {
        string InstanceUrl { get; }
        string AccessToken { get; }
        Task<IRestResponse> SendAsync(HttpMethod method, string url);
        void SendAsync(RestRequest request, AsyncRequestCallback callback);
        Task<IRestResponse> SendAsync(RestRequest request);
    }
}
