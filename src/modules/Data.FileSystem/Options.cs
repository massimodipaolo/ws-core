using System;
using System.Collections.Generic;
using Ws.Core.Extensions.Base;

namespace Ws.Core.Extensions.Data.FileSystem
{
    public class Options : IOptions
    {
        /// <summary>
        /// Relative to ContentRootPath
        /// </summary>
        public string Folder { get; set; } = "Files/Entity";
        public Ws.Core.Shared.Serialization.Options Serialization { get; set; } = new Ws.Core.Shared.Serialization.Options();
    }
}
