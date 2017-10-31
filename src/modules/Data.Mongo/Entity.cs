using System;
namespace core.Extensions.Data.Mongo
{
    public class Entity : core.Extensions.Data.Entity
    {
        [MongoDB.Bson.Serialization.Attributes.BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public override string Id { get; set; }
    }
}