using System;

namespace Mongo.Migration.Documents.Locators
{
    internal interface ICurrentVersionLocator : ILocator<DocumentVersion, Type>
    {
    }
}