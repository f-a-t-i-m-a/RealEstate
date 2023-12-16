using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Identity.MongoDB;
using Compositional.Composer;
using JahanJooy.RealEstateAgency.Domain.Users;
using JahanJooy.RealEstateAgency.Util.DataAccess;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;

namespace JahanJooy.RealEstateAgency.ShishDong.Server.Providers
{
    [Contract]
    [Component]
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        [ComponentPlug]
        public DbManager DbManager { get; set; }

        private readonly string _publicClientId;

        public ApplicationOAuthProvider()
        {
            _publicClientId = "jj.haftdong";
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var store = new UserStore<ApplicationUser>(DbManager.ApplicationUser);
            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(store);

            ApplicationUser user = userManager.FindAsync(context.UserName, context.Password).Result;

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            if (!user.IsEnabled)
            {
                context.SetError("invalid_grant", "The account is suspended.");
                return;
            }

//            if (!user.IsVerified)
//            {
//                context.SetError("invalid_grant", "The account is not verified.");
//                return;
//            }

            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
               OAuthDefaults.AuthenticationType);
            ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                CookieAuthenticationDefaults.AuthenticationType);

            AuthenticationProperties properties = CreateProperties(user.UserName);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
    }
}