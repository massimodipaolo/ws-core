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
        public class InMemoryOptions
        {
            public IEnumerable<IdentityResource> IdentityResources { get; set; }
            public IEnumerable<ApiResource> ApiResources { get; set; }
            public IEnumerable<Client> Clients { get; set; }
            public bool PersistedGrants { get; set; }
            public bool Caching { get; set; }
        }
    }
}