using System;
using ExtCore.Events;

namespace core.Extensions.Data
{
    public interface IEntityChangeEvent<TKey> : IEventHandler<EntityChangeEventContext<TKey>> where TKey : IEquatable<TKey>
    {
    }
}
