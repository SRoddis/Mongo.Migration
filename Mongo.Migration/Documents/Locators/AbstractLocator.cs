using System.Collections.Frozen;
using System.Reflection;

namespace Mongo.Migration.Documents.Locators;

public abstract class AbstractLocator<TReturnType, TAttributeType> : ILocator<TReturnType, Type>
    where TReturnType : struct
    where TAttributeType : Attribute
{
    private readonly Lazy<IDictionary<Type, TReturnType>> _lazyLocateDictionary;

    protected AbstractLocator()
    {
        _lazyLocateDictionary = new Lazy<IDictionary<Type, TReturnType>>(
            LoadLocateDictionary,
            LazyThreadSafetyMode.PublicationOnly);
    }

    protected abstract TReturnType GetAttributeValue(TAttributeType attribute);

    protected IDictionary<Type, TReturnType> LocatesDictionary => _lazyLocateDictionary.Value;

    public virtual TReturnType? GetLocateOrNull(Type identifier)
    {
        if (LocatesDictionary.TryGetValue(identifier, out TReturnType returnType))
        {
            return returnType;
        }

        return null;
    }

    private FrozenDictionary<Type, TReturnType> LoadLocateDictionary()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetExportedTypes())
            .Select(t => (t, t.GetCustomAttribute<TAttributeType>()))
            .Where(t => t.Item2 is not null)
            .ToFrozenDictionary(
                pair => pair.t,
                pair => GetAttributeValue(pair.Item2!));
    }
}