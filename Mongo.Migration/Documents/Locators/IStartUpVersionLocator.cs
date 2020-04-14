using System;

namespace Mongo.Migration.Documents.Locators
{
    public interface IStartUpVersionLocator : ILocator<DocumentVersion, Type>
    {
    }
}