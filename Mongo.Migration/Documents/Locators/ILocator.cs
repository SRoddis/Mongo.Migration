namespace Mongo.Migration.Documents.Locators;

public interface ILocator<TReturnType, TTypeIdentifier>
    where TReturnType : struct
    where TTypeIdentifier : class
{
    TReturnType? GetLocateOrNull(TTypeIdentifier identifier);

    void Locate();
}