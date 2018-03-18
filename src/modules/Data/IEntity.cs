using System;
using System.Collections.Generic;
using System.Text;

namespace core.Extensions.Data
{
    public interface IEntity<TKey> where TKey : IEquatable<TKey>
    {
        TKey Id { get; set; }
        void OnChange(EntityChangeEventContext<TKey>.ActionTypes action);
    }

    public class Entity<TKey> : IEntity<TKey> where TKey : IEquatable<TKey>
    {
        public virtual TKey Id { get; set; }

        public void OnChange(EntityChangeEventContext<TKey>.ActionTypes action)
        {
            ExtCore.Events.Event<IEntityChangeEvent<TKey>, EntityChangeEventContext<TKey>>
            .Broadcast(new EntityChangeEventContext<TKey>()
            {
                Entity = this,
                Action = action
            });
        }
    }

}