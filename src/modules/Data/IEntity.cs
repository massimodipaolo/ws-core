using System;
using System.Collections.Generic;
using System.Text;

namespace core.Data
{
    public interface IEntity
    {
        string Id { get; set; }
    }
    public class Entity : IEntity
    {
        //[MongoDB.Bson.Serialization.Attributes.BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
    }
}