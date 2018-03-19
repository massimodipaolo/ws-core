using System;
using System.Collections.Generic;
using System.Text;

namespace core.Extensions.Data
{

    public interface IEntity
    {  
    }

    public interface IEntity<TKey> :IEntity where TKey : IEquatable<TKey>
    {
        TKey Id { get; set; }
        void OnChange(EntityChangeEventContext.ActionTypes action);        
    }

    public class Entity<TKey> : IEntity<TKey> where TKey : IEquatable<TKey>
    {
        public virtual TKey Id { get; set; }

        public void OnChange(EntityChangeEventContext.ActionTypes action)
        {
           
            ExtCore.Events.Event<IEntityChangeEvent<IEntity<TKey>, TKey>, EntityChangeEventContext>
            .Broadcast(new EntityChangeEventContext()
            {
                Entity = this,
                Action = action
            });

            ExtCore.Events.Event<ISomeActionEventHandler, string>.Broadcast($"Entity {this.GetType().ToString()} ({Id.ToString()}) updated at {DateTime.Now}");
        }

    }

}