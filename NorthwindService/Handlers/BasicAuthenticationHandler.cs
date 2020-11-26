using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace NorthwindService.Handlers
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {

        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authentication"))
            {
                return AuthenticateResult.Fail("Authorization headers were not found");
            }

            var authenticationHeaderValue = AuthenticationHeaderValue.Parse(Request.Headers["Authentication"]);
            var bytes = Convert.FromBase64String(authenticationHeaderValue.Parameter);
            string[] credentials = Encoding.UTF8.GetString(bytes).Split(":");
            string email = credentials[0];
            string password = credentials[1];

            // check the cache or the database for user.

            if (email == "john.smith@gmail.com" && password == "password") // hardcoded values to try
            {
                var claims = new[] { new Claim(ClaimTypes.Name, "john.smith@gmail.com") };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }

            return AuthenticateResult.Fail("Not implemented");
        }
    }
}
