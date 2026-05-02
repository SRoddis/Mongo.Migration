using Mongo.Migration.Documents.Attributes;

namespace Mongo.Migration.Documents.Locators;

internal class StartUpVersionLocator : AbstractLocator<DocumentVersion, StartUpVersionAttribute>, IStartUpVersionLocator
{
    protected override DocumentVersion GetAttributeValue(StartUpVersionAttribute attribute)
    {
        return attribute.Version;
    }
}