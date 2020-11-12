using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using Microsoft.Extensions.DependencyModel;

namespace Ws.Core.Extensions.Data.Mongo
{
    public class Extension : Base.Extension
    {
        private Options _options => GetOptions<Options>();

        //private static T BsonClassMap<core.Extensions.Data.Entity<T>> map => default(T);

        public override void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            base.Execute(serviceCollection, serviceProvider);

            var connections = _options?.Connections;
            if (connections != null && connections.Any())
            {
                // Mappings
                var tKeys = new KeyValuePair<Type, IBsonSerializer>[] {
                new KeyValuePair<Type,IBsonSerializer>(typeof(Entity<int>),new Int32Serializer(BsonType.Int32)),
                new KeyValuePair<Type,IBsonSerializer>(typeof(Entity<long>),new Int64Serializer(BsonType.Int64)),
                new KeyValuePair<Type,IBsonSerializer>(typeof(Entity<Guid>),new GuidSerializer(BsonType.String)),
                new KeyValuePair<Type,IBsonSerializer>(typeof(Entity<string>),new StringSerializer(BsonType.String))
                };
                foreach (var tKey in tKeys)
                {
                    var cm = new BsonClassMap(tKey.Key);
                    cm.AutoMap();
                    cm.MapIdMember(tKey.Key.GetMember("Id").Single());
                    cm.IdMemberMap.SetSerializer(tKey.Value);
                    BsonClassMap.RegisterClassMap(cm);
                }

                var hcBuilder = serviceCollection.AddHealthChecks();
                foreach (var conn in connections)
                    hcBuilder.AddMongoDb(conn.ConnectionString, name: $"mongodb-{conn.Name}");

                serviceCollection.Configure<Options>(_ =>
                {
                    _.Connections = connections;
                });

                serviceCollection.TryAddTransient(typeof(IRepository<,>), typeof(Repository.Mongo<,>));
            }
        }
    }
}