using ExtCore.Infrastructure.Actions;

namespace Ws.Core.Extensions;

public static class InjectorInstance
{
    public static IEnumerable<IConfigureBuilder> UnionInjector(this IEnumerable<IConfigureBuilder> items)
    => _unionInject<IConfigureBuilder>(items);

    public static IEnumerable<IConfigureApp> UnionInjector(this IEnumerable<IConfigureApp> items)
    => _unionInject<IConfigureApp>(items);

    private static IEnumerable<T> _unionInject<T>(IEnumerable<T> list) where T : class, IConfigureAction
    {
        Type injectorType = typeof(Ws.Core.Extensions.Injector);
        list = list.Where(_ => _.GetType() != injectorType);

        IEnumerable<Ws.Core.Extensions.Injector> injectors = Ws.Core.Extensions.Injector.List();
        if (injectors != null && injectors.Any())
            list = list
            .Union((IEnumerable<T>)injectors)
            //.OrderBy(_ => _.Priority)
            ;
        return list;
    }
}
