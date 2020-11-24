using System;
using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Services.Interceptors
{
    public interface IMigrationInterceptorFactory
    {
        IBsonSerializer Create(Type type);
    }
}