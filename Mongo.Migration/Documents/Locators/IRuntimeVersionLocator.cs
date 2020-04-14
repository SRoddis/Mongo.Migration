using System;

namespace Mongo.Migration.Documents.Locators
{
    public interface IRuntimeVersionLocator : ILocator<DocumentVersion, Type>
    {
    }
}