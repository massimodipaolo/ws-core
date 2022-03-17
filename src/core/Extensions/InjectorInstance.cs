using ExtCore.Infrastructure;
using ExtCore.Infrastructure.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ws.Core.Extensions
{
    public static class InjectorInstance
    {
        public static IEnumerable<IConfigureServicesAction> UnionInjector(this IEnumerable<IConfigureServicesAction> items)
        => _unionInject<IConfigureServicesAction>(items);

        public static IEnumerable<IConfigureAction> UnionInjector(this IEnumerable<IConfigureAction> items)
        => _unionInject<IConfigureAction>(items);

        private static IEnumerable<T> _unionInject<T>(IEnumerable<T> list) where T : class
        {
            Type injectorType = typeof(Ws.Core.Extensions.Base.Injector);
            list = list.Where(_ => _.GetType() != injectorType);

            IEnumerable<Ws.Core.Extensions.Base.Injector> injectors = Ws.Core.Extensions.Base.Injector.List();
            if (injectors != null && injectors.Any())
                list = list
                .Union((IEnumerable<T>)injectors)
                ;
            return list;
        }
    }
}
