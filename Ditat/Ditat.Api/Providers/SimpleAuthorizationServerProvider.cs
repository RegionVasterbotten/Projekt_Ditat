using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Ditat.Api.Providers
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            if (!ValidateUser(context.UserName, context.Password))
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("sub", context.UserName));
            identity.AddClaim(new Claim("role", "user"));
            bool isAdmin = false;
            if (context.UserName == "testadmin")
            {
                identity.AddClaim(new Claim("isAdmin", "true"));
                isAdmin = true;
            }
            else
                identity.AddClaim(new Claim("isAdmin", "false"));

            var props = new AuthenticationProperties(new Dictionary<string, string>
            {
                {"isAdmin", isAdmin.ToString().ToLower()}
            });
            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);
        }

        /// <summary>
        /// Needed for token response authentication properties (isAdmin)
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }
            return Task.FromResult<object>(null);
        }


        /*
        public override Task MatchEndpoint(OAuthMatchEndpointContext context)
        {
            SetCORSPolicy(context.OwinContext);
            if (context.Request.Method == "OPTIONS")
            {
                context.RequestCompleted();
                return Task.FromResult(0);
            }

            return base.MatchEndpoint(context);
        }

        private void SetCORSPolicy(IOwinContext context)
        {
            string allowedUrls = ConfigurationManager.AppSettings["allowedOrigins"];

            if (!String.IsNullOrWhiteSpace(allowedUrls))
            {
                var list = allowedUrls.Split(',');
                if (list.Length > 0)
                {

                    string origin = context.Request.Headers.Get("Origin");
                    var found = list.Where(item => item == origin).Any();
                    if (found)
                    {
                        context.Response.Headers.Add("Access-Control-Allow-Origin",
                                                     new string[] { origin });
                    }
                }

            }
            context.Response.Headers.Add("Access-Control-Allow-Origin",
                                         new string[] { "*" });
            context.Response.Headers.Add("Access-Control-Allow-Headers",
                                   new string[] { "Authorization", "Content-Type" });
            context.Response.Headers.Add("Access-Control-Allow-Methods",
                                   new string[] { "OPTIONS", "POST" });

        }
        */

        private bool ValidateUser (string user, string password)
        {
            //TODO read/match user from database here
            if (user == "testuser") {
                if (password=="XXXXXXXX")
                {
                    return true;
                }
            }
            if (user == "testadmin")
            {
                if (password == "xxxx")
                {
                    return true;
                }
            }
            return false;
        }
    }
}