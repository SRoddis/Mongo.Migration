using System;

namespace Mongo.Migration.Documents.Locators
{
    internal interface IVersionLocator : ILocator<DocumentVersion, Type>
    {
    }
}