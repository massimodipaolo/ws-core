﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Ws.Core.Extensions.Data.Mongo;

public class Extension : Base.Extension
{
    private Options options => GetOptions<Options>();

    public override void Execute(WebApplicationBuilder builder, IServiceProvider serviceProvider = null)
    {
        base.Execute(builder, serviceProvider);

        var connections = options?.Connections;
        if (connections?.Any() == true)
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
                try
                {
                    var cm = new BsonClassMap(tKey.Key);
                    cm.AutoMap();
                    cm.MapIdMember(tKey.Key.GetMember("Id").Single());
                    cm.IdMemberMap.SetSerializer(tKey.Value);
                    if (!BsonClassMap.GetRegisteredClassMaps().Contains(cm))
                        BsonClassMap.RegisterClassMap(cm);
                }
                catch(Exception ex) {
                    serviceProvider?.GetService<Microsoft.Extensions.Logging.ILogger<Ws.Core.Extensions.Data.Mongo.Extension>>()?.LogWarning(ex, "");
                }
            }

            var hcBuilder = builder.Services.AddHealthChecks();
            foreach (var conn in connections)
                hcBuilder.AddMongoDb(conn.ConnectionString, name: $"mongodb-{conn.Name}", tags: new[] { "db", "mongodb" });

            builder.Services.Configure<Options>(_ =>
            {
                _.Connections = connections;
            });

            builder.Services.TryAddTransient(typeof(IRepository<,>), typeof(Repository.Mongo<,>));
        }
    }
}