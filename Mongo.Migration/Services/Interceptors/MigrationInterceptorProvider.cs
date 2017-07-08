using System;
using System.Linq;
using Mongo.Migration.Models;
using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Services.Interceptors
{
    public class MigrationInterceptorProvider : IBsonSerializationProvider
    {
        public IBsonSerializer GetSerializer(Type type)
        {
            if (IsNoMigrateDocument(type))
            {
                return null;
            }

            return CreateMigrationInterceptorInstance(type);
        }

        private static bool IsNoMigrateDocument(Type type)
        {
            return !type.GetInterfaces().Contains(typeof (IDocument));
        }

        private static IBsonSerializer CreateMigrationInterceptorInstance(Type type)
        {
            Type genericType = typeof (MigrationInterceptor<>).MakeGenericType(type);
            object interceptor = Activator.CreateInstance(genericType);
            return interceptor as IBsonSerializer;
        }
    }
}