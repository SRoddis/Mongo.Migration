using System;
using Mongo.Migration.Documents;
using Mongo.Migration.Documents.VersionProviders;

namespace Mongo.Migration.Services.DiContainer
{
    internal interface IComponentRegistry
    {
        void RegisterComponents<TBaseDocument>(IDocumentVersionProvider<TBaseDocument> documentVersionProvider);

        TComponent Get<TComponent>() where TComponent : class;
    }
}