using System;

namespace Mongo.Migration.Documents.Locators
{
    internal interface ICollectionVersionLocator : ILocator<DocumentVersion, Type>
    {
    }
}