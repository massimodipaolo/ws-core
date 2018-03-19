using System;
namespace core.Extensions.Data
{

    public class EntityChangeEventContext
    {        
        public IEntity Entity { get; set; }
        public ActionTypes Action { get; set; }

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

