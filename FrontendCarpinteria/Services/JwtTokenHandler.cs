using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace FrontendCarpinteria.Services
{
    public class JwtTokenHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string? token = null;

            try
            {
                token = await SecureStorage.Default.GetAsync("auth_token");
            }
            catch { }

            if (string.IsNullOrEmpty(token))
            {
                token = Preferences.Default.Get("auth_token_fallback", string.Empty);
            }

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}