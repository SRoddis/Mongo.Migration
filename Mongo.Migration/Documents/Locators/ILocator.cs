namespace Mongo.Migration.Documents.Locators;

public interface ILocator<TReturnType, in TTypeIdentifier>
    where TReturnType : struct
    where TTypeIdentifier : class
{
    TReturnType? GetLocateOrNull(TTypeIdentifier identifier);
}