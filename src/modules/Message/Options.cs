using core.Extensions.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace core.Extensions.Message
{
    public interface IMessageConfiguration
    {
        IEnumerable<Options.EndpointOptions> Senders { get; set; }
        IEnumerable<Options.EndpointOptions> Receivers { get; set; }
    }
    public class Options : IOptions,IMessageConfiguration
    {
        public IEnumerable<EndpointOptions> Senders { get; set; }
        public IEnumerable<EndpointOptions> Receivers { get; set; }
        public class EndpointOptions
        {
            public string Address { get; set; }
            public int Port { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }            
        }
    }
}
