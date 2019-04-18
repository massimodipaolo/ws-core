using System;
using System.Collections.Generic;
using System.Text;

namespace Ws.Core.Extensions.Data.EF
{
    class Extension: Base.Extension
    {
        public Options _options => GetOptions<Options>();
    }
}
