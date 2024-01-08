using System;
using System.Collections.Generic;

using Mongo.Migration.Documents.Attributes;

namespace Mongo.Migration.Documents.Locators;

public interface ICollectionLocator : ILocator<CollectionLocationInformation, Type>
{
    IDictionary<Type, CollectionLocationInformation> GetLocatesOrEmpty();
}