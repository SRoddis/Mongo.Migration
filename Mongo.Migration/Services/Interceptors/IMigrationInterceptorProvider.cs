using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Services.Interceptors;

public interface IMigrationInterceptorProvider : IBsonSerializationProvider
{
}