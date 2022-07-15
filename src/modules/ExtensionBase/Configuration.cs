using Microsoft.Extensions.DependencyInjection;

namespace Ws.Core.Extensions.Base;

public class Configuration
{
    public static string Folder { get; set; } = "Extensions";
    public static string SectionRoot { get; set; } = "extConfig";
    public bool EnableShutDownOnChange { get; set; } = false;
    public IDictionary<string,Assembly> Assemblies { get; set; }
    public IEnumerable<Injector> Injectors { get; set; }

    public class Assembly
    {
        public Assembly() { }
        public string Name { get; set; }
        public int Priority { get; set; } = 0;
    }
    public class Injector : Assembly, IOptions
    {
        public Injector(): base() {}
        public ServiceOption[] Services { get; set; }
        public DecoratorOption[] Decorators { get; set; }
        public MiddlewareOption[] Middlewares { get; set; }

        public class DecoratorOption
        {
            public string ServiceType { get; set; }
            public string ImplementationType { get; set; }
        }
        public class ServiceOption: DecoratorOption
        {
            public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;
            public bool OverrideIfAlreadyRegistered { get; set; } = true;
        }
        public class MiddlewareOption
        {
            /// <summary>
            /// Middleware delegate class, must include:
            /// - A public constructor with a parameter of type RequestDelegate.
            /// - A public method named Invoke or InvokeAsync that return a Task and accept a first parameter of type HttpContext.
            /// Additional parameters for the constructor and Invoke/InvokeAsync are populated by dependency injection (DI).            
            /// </summary>
            public string Type { get; set; }
            /// <summary>
            /// Branches the request pipeline based on matches of the given request path
            /// </summary>
            public MapOption Map { get; set; }
            public class MapOption
            {
                /// <summary>
                /// The request path to match (starts with)
                /// </summary>
                public string PathMatch { get; set; }
                /// <summary>
                /// If false, matched path would be removed from Request.Path and added to Request.PathBase
                /// </summary>
                public bool PreserveMatchedPathSegment { get; set; } = true;
            }
        }
    }

}
