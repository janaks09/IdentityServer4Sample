using DartsCool.Idsvr.Core;
using DartsCool.Idsvr.Services;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace DartsCool.Idsvr.Extensions
{
    public static class StartupExtensions
    {

        public static IIdentityServerBuilder ConfigureAspNetIdentity<TUser>(this IIdentityServerBuilder builder)
            where TUser: class
        {
            var services = builder.Services;

            services.AddTransient<IProfileService, ProfileService<TUser>>();
            services.AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator<TUser>>();

            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserIdClaimType = JwtClaimTypes.Subject;
                options.ClaimsIdentity.UserNameClaimType = JwtClaimTypes.Name;
                options.ClaimsIdentity.RoleClaimType = JwtClaimTypes.Role;
            });

            services.Configure<IdentityServerOptions>(
                options =>
                {
                    options.AuthenticationOptions.PrimaryAuthenticationScheme = Constants.PrimaryAuthenticationType;
                });

            return builder;
        }
    }
}
