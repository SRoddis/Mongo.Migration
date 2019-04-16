using System;
using System.Collections;
using System.Collections.Generic;
using Mongo.Migration.Documents.Attributes;

namespace Mongo.Migration.Documents.Locators
{
    public interface IDatabaseLocator : ILocator<DatabaseLocationInformation, Type>
    {
        IDictionary<Type, DatabaseLocationInformation> GetLocatesOrEmpty();
    }
}