using System;
using System.Collections.Generic;
using System.Text;

namespace Ws.Core.Extensions.Base
{
    public interface IDiscriminator
    {
        string Value { get; }
    }
    public interface IDiscriminator<T> : IDiscriminator where T : class
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
