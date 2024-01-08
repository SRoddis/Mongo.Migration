using System;

namespace Mongo.Migration.Migrations.Adapters;

public interface IContainerProvider
{
    object GetInstance(Type type);
}