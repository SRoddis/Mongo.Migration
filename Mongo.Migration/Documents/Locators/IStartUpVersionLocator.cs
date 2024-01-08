using System;

namespace Mongo.Migration.Documents.Locators;

internal interface IStartUpVersionLocator : ILocator<DocumentVersion, Type>
{
}