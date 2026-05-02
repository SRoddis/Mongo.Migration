using System.Collections.Frozen;
using Mongo.Migration.Documents.Attributes;

namespace Mongo.Migration.Documents.Locators;

internal class RuntimeVersionLocator : AbstractLocator<DocumentVersion, RuntimeVersionAttribute>, IRuntimeVersionLocator
{
    private readonly FrozenDictionary<Type, DocumentVersion> _codeDefinedDictionary;

    internal RuntimeVersionLocator(IEnumerable<KeyValuePair<Type, DocumentVersion>> alreadyDefinedRuntimeVersions)
    {
        _codeDefinedDictionary = alreadyDefinedRuntimeVersions.ToFrozenDictionary();
    }

    protected override DocumentVersion GetAttributeValue(RuntimeVersionAttribute attribute)
    {
        return attribute.Version;
    }

    public override DocumentVersion? GetLocateOrNull(Type identifier)
    {
        if (_codeDefinedDictionary.TryGetValue(identifier, out DocumentVersion version))
        {
            return version;
        }

        return base.GetLocateOrNull(identifier);
    }
}