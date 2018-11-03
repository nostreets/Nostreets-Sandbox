using System.Threading.Tasks;
using Microsoft.Owin.Security.OAuth;

namespace Nostreets_Sandbox.App_Start
{
    internal class ApplicationOAuthProvider : IOAuthAuthorizationServerProvider
    {
        private string publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            this.publicClientId = publicClientId;
        }

        public Task AuthorizationEndpointResponse(OAuthAuthorizationEndpointResponseContext context)
        {
            throw new System.NotImplementedException();
        }

        public Task AuthorizeEndpoint(OAuthAuthorizeEndpointContext context)
        {
            throw new System.NotImplementedException();
        }

        public Task GrantAuthorizationCode(OAuthGrantAuthorizationCodeContext context)
        {
            throw new System.NotImplementedException();
        }

        public Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            throw new System.NotImplementedException();
        }

        public Task GrantCustomExtension(OAuthGrantCustomExtensionContext context)
        {
            throw new System.NotImplementedException();
        }

        public Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            throw new System.NotImplementedException();
        }

        public Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            throw new System.NotImplementedException();
        }

        public Task MatchEndpoint(OAuthMatchEndpointContext context)
        {
            throw new System.NotImplementedException();
        }

        public Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            throw new System.NotImplementedException();
        }

        public Task TokenEndpointResponse(OAuthTokenEndpointResponseContext context)
        {
            throw new System.NotImplementedException();
        }

        public Task ValidateAuthorizeRequest(OAuthValidateAuthorizeRequestContext context)
        {
            throw new System.NotImplementedException();
        }

        public Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            throw new System.NotImplementedException();
        }

        public Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            throw new System.NotImplementedException();
        }

        public Task ValidateTokenRequest(OAuthValidateTokenRequestContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}