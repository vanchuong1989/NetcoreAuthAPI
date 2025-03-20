using API.JwtToken.Repos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace API.JwtToken.Helper
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly dbContext dbContext;

        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, 
            ILoggerFactory logger, 
            UrlEncoder encoder, 
            ISystemClock clock,
            dbContext dbContext) : base(options, logger, encoder, clock)
        {
            this.dbContext = dbContext;
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("No header found");
            }
            var headerValue =  AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);

            if (headerValue != null)
            {
                var bytes = Convert.FromBase64String(headerValue.Parameter); //convert headerValue to Base64String
                string credentials = Encoding.UTF8.GetString(bytes); //it will return to a string "admin:Test"
                string[] array = credentials.Split(":"); //array[0]="admin", array[1]="Test"
                string username = array[0];
                string password = array[1];

                //find user bases on credentials
                var user = await this.dbContext.TblUsers.FirstOrDefaultAsync(x => x.Username == username && x.Password == password);
                if (user != null)
                {
                    var claim = new[] { new Claim(ClaimTypes.Name, user.Username) };
                    var identity = new ClaimsIdentity(claim, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);

                    return AuthenticateResult.Success(ticket);
                }
                else
                {
                    return AuthenticateResult.Fail("UnAuthorized");
                }

            }
            else
            {
                return AuthenticateResult.Fail("Empty header");
            }
        }
    }
}
