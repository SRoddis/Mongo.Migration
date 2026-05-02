using Mongo.Migration.Documents.Attributes;

namespace Mongo.Migration.Documents.Locators;

public class CollectionLocator : AbstractLocator<CollectionLocationInformation, CollectionLocationAttribute>, ICollectionLocator
{
    protected override CollectionLocationInformation GetAttributeValue(CollectionLocationAttribute attribute)
    {
        return attribute.CollectionInformation;
    }

    public override CollectionLocationInformation? GetLocateOrNull(Type identifier)
    {
        if (!LocatesDictionary.ContainsKey(identifier))
        {
            return null;
        }

        LocatesDictionary.TryGetValue(identifier, out var value);
        return value;
    }

    public IDictionary<Type, CollectionLocationInformation> GetLocatesOrEmpty()
    {
        return LocatesDictionary;
    }
}