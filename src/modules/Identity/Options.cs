using core.Extensions.Base;
using System;
using System.Collections.Generic;
using System.Text;
using IdentityServer4.Models;

namespace core.Extensions.Identity
{
    class Options : IOptions
    {
        public InMemoryOptions InMemory { get; set; }
        public bool DeveloperSigningCredential { get; set; } = false;
        public bool JwtBearerClientAuthentication { get; set; } = false;
        public IEnumerable<IdentityServer4.Test.TestUser> TestUsers { get; set; }
        public class InMemoryOptions
        {
            public bool Enable { get; set; } = true;
            public IEnumerable<IdentityResource> IdentityResources { get; set; }
            public IEnumerable<ApiResource> ApiResources { get; set; }
            public IEnumerable<Client> Clients { get; set; }
            public bool PersistedGrants { get; set; } = false;
            public bool Caching { get; set; } = false;
        }
    }
}