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

        public override void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            base.Execute(serviceCollection, serviceProvider);

            BsonClassMap.RegisterClassMap<core.Extensions.Data.Entity>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id);
                cm.IdMemberMap.SetSerializer(new StringSerializer(BsonType.ObjectId));
            });

            var connections = _options?.Connections;
            if (connections != null && connections.Any())
            {
                serviceCollection.Configure<Options>(_ =>
                {
                    _.Connections = connections;
                });
                serviceCollection.TryAddTransient(typeof(IRepository<>), typeof(Repository.Mongo<>));
            }
        }
    }
}