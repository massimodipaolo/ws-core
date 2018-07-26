using System;
using System.Collections.Generic;
using System.Text;

namespace core.Extensions.Data.EF
{
    class Extension: Base.Extension
    {
        public Options _options => GetOptions<Options>();
    }
}
