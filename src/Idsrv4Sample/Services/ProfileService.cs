using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;

namespace Idsrv4Sample.Services
{
    public class ProfileService<TUser>: IProfileService
        where TUser: class 
    {
        private readonly UserManager<TUser> _userManager;

        public ProfileService(UserManager<TUser> userManager )
        {
            _userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _userManager.FindByIdAsync(context.Subject.GetSubjectId());

            var claims = await getClaims(user);

            if (!context.AllClaimsRequested)
            {
                claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();
            }

            context.IssuedClaims = claims;
        }
        
        public async Task IsActiveAsync(IsActiveContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.Subject == null) throw new ArgumentNullException(nameof(context.Subject));

            context.IsActive = false;

            var subject = context.Subject;
            var user = await _userManager.FindByIdAsync(context.Subject.GetSubjectId());

            if (user != null)
            {
                var securityStampChanged = false;

                if (_userManager.SupportsUserSecurityStamp)
                {
                    var securityStamp = (
                        from claim in subject.Claims
                        where claim.Type == "security-stamp"
                        select claim.Value
                        ).SingleOrDefault();

                    if (securityStamp != null)
                    {
                        var latestSecurityStamp = await _userManager.GetSecurityStampAsync(user);
                        securityStampChanged = securityStamp != latestSecurityStamp;
                    }
                }

                context.IsActive = !securityStampChanged && !await _userManager.IsLockedOutAsync(user);
            }
        }

        private async Task<List<Claim>> getClaims(TUser user)
        {
            var  claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, await _userManager.GetUserIdAsync(user)),
                new Claim(JwtClaimTypes.Name, await _userManager.GetUserNameAsync(user))
            };

            if (_userManager.SupportsUserClaim)
            {
                claims.AddRange(await _userManager.GetClaimsAsync(user));
            }

            if (_userManager.SupportsUserRole)
            {
                var roles = await _userManager.GetRolesAsync(user);
                claims.AddRange(roles.Select(role => new Claim(JwtClaimTypes.Role, role)));
            }

            return claims;
        }
    }
}
