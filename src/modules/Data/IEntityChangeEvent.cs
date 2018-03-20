using System;
using ExtCore.Events;

namespace core.Extensions.Data
{
    public interface IEntityChangeEvent<T,TKey> : IEventHandler<EntityChangeEventContext> where T : IEntity<TKey> where TKey : IEquatable<TKey>
    {
    }

    public interface IEntityChangeEvent<TKey> : IEventHandler<EntityChangeEventContext> where TKey : IEquatable<TKey>
    {
    }

    public interface IEntityChangeEvent : IEventHandler<EntityChangeEventContext>
    {
    }

}
