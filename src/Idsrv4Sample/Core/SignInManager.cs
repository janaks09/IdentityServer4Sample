using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DartsCool.Idsvr.Core
{
    public class IdSrvSignInManager<TUser>: SignInManager<TUser>
        where TUser: class 
    {

        private readonly IHttpContextAccessor contextAccessor;
        private readonly IdentityOptions options;

        public IdSrvSignInManager(
            UserManager<TUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<TUser>  claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<TUser>> logger)
            :base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger)
        {
            this.contextAccessor = contextAccessor;
            this.options = optionsAccessor.Value;
        }

        public HttpContext Context
        {
            get
            {
                var context = contextAccessor?.HttpContext;
                if (context == null)
                {
                    throw  new InvalidOperationException("HttpContext must not be null");
                }

                return context;
            }
        }

        public override async Task SignInAsync(TUser user, AuthenticationProperties authenticationProperties, string authenticationMethod = null)
        {
            var userPrincipal = await CreateUserPrincipalAsync(user);

            userPrincipal.Identities.First().AddClaims(new  []
            {
                new Claim(JwtClaimTypes.IdentityProvider, authenticationMethod ?? Constants.BuiltInIdentityProvider), 
                new Claim(JwtClaimTypes.AuthenticationTime, DateTime.UtcNow.ToEpochTime().ToString())
            });

            await Context.Authentication.SignInAsync(options.Cookies.ApplicationCookieAuthenticationScheme,
                userPrincipal,
                authenticationProperties ?? new AuthenticationProperties());
        }

    }
}
