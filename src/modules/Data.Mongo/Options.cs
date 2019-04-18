using System;
using System.Collections.Generic;
using Ws.Core.Extensions.Base;

namespace Ws.Core.Extensions.Data.Mongo
{
    public class Options : IOptions
    {
        public IEnumerable<Extensions.Data.DbConnection> Connections { get; set; }
    }
}
