using System;
using Mongo.Migration.Documents;

namespace Mongo.Migration.Services.DiContainer
{
    internal interface IComponentRegistry
    {
        void RegisterComponents<TBaseDocument>(Func<TBaseDocument, DocumentVersion> versionGetter, Action<TBaseDocument, DocumentVersion> versionSetter);

        TComponent Get<TComponent>() where TComponent : class;
    }
}