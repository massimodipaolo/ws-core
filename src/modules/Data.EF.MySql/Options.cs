using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using Ws.Core.Extensions.Base;

namespace Ws.Core.Extensions.Data.EF.MySql;

public class Options : IOptions
{
    public IEnumerable<Extensions.Data.DbConnection>? Connections { get; set; }
    [DefaultValue(ServiceLifetime.Scoped)]
    public ServiceLifetime ServiceLifetime { get; set; } = ServiceLifetime.Scoped;
}