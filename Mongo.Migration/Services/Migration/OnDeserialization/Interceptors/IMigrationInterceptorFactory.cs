using System;
using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Services.Migration.OnDeserialization.Interceptors
{
    internal interface IMigrationInterceptorFactory
    {
        IBsonSerializer Create(Type type);
    }
}