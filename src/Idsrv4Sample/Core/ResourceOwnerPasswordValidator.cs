using System;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;

namespace DartsCool.Idsvr.Core
{
    public class ResourceOwnerPasswordValidator<TUser>: IResourceOwnerPasswordValidator 
        where TUser: class
    {
        private readonly UserManager<TUser> _userManager;

        public ResourceOwnerPasswordValidator(UserManager<TUser> userManager )
        {
            _userManager = userManager;
        }

        public async Task<CustomGrantValidationResult> ValidateAsync(string userName, string password, ValidatedTokenRequest request)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                return new CustomGrantValidationResult(await _userManager.GetUserIdAsync(user), "password");
            }

            return  new CustomGrantValidationResult("Invalid Username or Password.");
        }
    }
}
