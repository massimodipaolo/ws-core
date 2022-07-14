using System;
using System.Collections.Generic;
using System.Text;

namespace Ws.Core.Extensions.Spa
{
    public interface IDiscriminator
    {
        string Value { get; }
    }

    /// <summary>
    /// Use in DI to map implementation to type
    /// </summary>
    /// <example>
    /// builder.Services.TryAddSingleton(typeof(IDiscriminator<![CDATA[<FooType>]]>), typeof(ABTestingDiscriminator<![CDATA[<FooType>]]>));
    /// </example>
    /// <typeparam name="T"></typeparam>
#pragma warning disable S2326 // Unused type parameters should be removed
    public interface IDiscriminator<T> : IDiscriminator where T : class
#pragma warning restore S2326 // Unused type parameters should be removed
    {
    }

    public class Discriminator : IDiscriminator
    {
        public Discriminator() { }
        public string Value => "default";
    }

    public class Discriminator<T> : Discriminator, IDiscriminator<T> where T : class
    {
        public Discriminator() : base() { }
    }
}
