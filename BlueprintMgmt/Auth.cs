using Microsoft.CommonLib;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
//using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlueprintMgmt
{
    public class Auth
    {

        private IConfigReader _config;
        public Auth(IConfigReader config)
        {
            _config = config;
        }

        public async Task<string> Authenticate(string resourceUri)
        {
            var tenantId = _config["ida:TenantId"];
            var authority = String.Format(_config["ida:AuthorityFormat"], tenantId);

            var authenticationContext =
                new AuthenticationContext(authority);
            var credential = new ClientCredential(clientId: _config["ida:ClientId"], clientSecret: _config["ida:ClientSecret"]);

            var result = await authenticationContext.AcquireTokenAsync(resource: resourceUri,
                clientCredential: credential);

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            return result.AccessToken;
        }
        public async Task<string> GetAccessToken(string resource)
        {
            var tenantId = _config["ida:TenantId"];
            var authority = String.Format(_config["ida:AuthorityFormat"], tenantId);

            var authContext = new AuthenticationContext(authority);
            var parameters = new PlatformParameters(PromptBehavior.Always);
            var clientId = _config["ida:ClientId"];
            var replyUrl = _config["ida:ReplyUrl"];
            //AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientId, new Uri(replyUrl), parameters, UserIdentifier.AnyUser);
            var credential = new ClientCredential(clientId: _config["ida:ClientId"], clientSecret: _config["ida:ClientSecret"]);
            var result = await authContext.AcquireTokenAsync(resource: resource,
                clientCredential: credential);

            if (result == null)
                throw new InvalidOperationException("Failed to obtain access token.");

            return result.AccessToken;
        }

    }
}
