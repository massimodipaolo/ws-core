using System;
namespace core.Extensions.Data
{
    public class EntityChangeEventContext<TKey> where TKey : IEquatable<TKey>
    {
        public ActionTypes Action { get; set; }

        public Entity<TKey> Entity { get; set; }

        public enum ActionTypes
        {
            Create,
            Update,
            Delete
        }

        public EntityChangeEventContext()
        {
        }
    }
}
