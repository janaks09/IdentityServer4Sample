using IdentityServer4.Models;
using System.Collections.Generic;

namespace Idsrv4Sample.Configuration
{
    public class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new List<Client>()
            {
                new Client()
                {
                    ClientId = "myawesomeapp",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("myawesomeapp-secret".Sha256())
                    },
                    ClientName = "my awesome app",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,

                    AllowedScopes = new List<string>
                    {
                        StandardScopes.OpenId.Name,
                        StandardScopes.Profile.Name,
                        StandardScopes.OfflineAccess.Name,
                        StandardScopes.Phone.Name
                    }
                }
            };
        }
    }
}
