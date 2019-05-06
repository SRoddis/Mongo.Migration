using System;
using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Services.Interceptors
{
    internal interface IMigrationInterceptorFactory
    {
        IBsonSerializer Create(Type type);
    }
}