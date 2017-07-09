using System;

namespace Mongo.Migration.Documents.Locators
{
    internal interface IVersionLocator
    {
        DocumentVersion? GetCurrentVersion(Type type);

        void LoadVersions();
    }
}