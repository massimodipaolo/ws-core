using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using Microsoft.Extensions.DependencyModel;

namespace core.Extensions.Data.Mongo
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

                BsonClassMap.RegisterClassMap<core.Extensions.Data.Entity<int>>(cm =>
               {
                   cm.AutoMap();
                   cm.MapIdMember(c => c.Id);
                   cm.IdMemberMap.SetSerializer(new Int32Serializer(BsonType.Int32));
               });

                BsonClassMap.RegisterClassMap<core.Extensions.Data.Entity<long>>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                    cm.IdMemberMap.SetSerializer(new Int64Serializer(BsonType.Int64));
                });


                BsonClassMap.RegisterClassMap<core.Extensions.Data.Entity<Guid>>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                    cm.IdMemberMap.SetSerializer(new GuidSerializer(BsonType.String));
                });

                BsonClassMap.RegisterClassMap<core.Extensions.Data.Entity<string>>(cm =>
                {
                    cm.AutoMap();
                    cm.MapIdMember(c => c.Id);
                    cm.IdMemberMap.SetSerializer(new StringSerializer(BsonType.String));
                    //cm.IdMemberMap.SetSerializer(new StringSerializer(BsonType.ObjectId));
                });

                serviceCollection.Configure<Options>(_ =>
                {
                    _.Connections = connections;
                });

                serviceCollection.TryAddTransient(typeof(IRepository<,>), typeof(Repository.Mongo<,>));
            }
        }
    }
}