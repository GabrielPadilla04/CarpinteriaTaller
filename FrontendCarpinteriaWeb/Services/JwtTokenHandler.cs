using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace FrontendCarpinteriaWeb.Services
{
    public class JwtTokenHandler : DelegatingHandler
    {
        private readonly IJSRuntime _jsRuntime;

        public JwtTokenHandler(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string? token = null;

            try
            {
                token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "auth_token");
            }
            catch { }

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}