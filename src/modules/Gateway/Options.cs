using System.ComponentModel;
using Ws.Core.Extensions.Base;

namespace Ws.Core.Extensions.Gateway;

public class Options : IOptions
{
    [Description("Regex-based role to use gateway pipeline, otherwise catchAll request")]
    [DefaultValue("string empty")]
    public string MapWhen { get; set; } = "";
    [Description("Ocelot config: https://ocelot.readthedocs.io/en/latest/features/configuration.html")]
    public Ocelot.Configuration.File.FileConfiguration Ocelot { get; set; }
    public Options() {
    }

}
