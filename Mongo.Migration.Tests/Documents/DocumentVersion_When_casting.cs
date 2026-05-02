using Mongo.Migration.Documents;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Documents;

[TestFixture]
public class DocumentVersionWhenCasting
{
    [Test]
    public void If_implicit_string_to_version_Then_cast_should_work()
    {
        DocumentVersion version = new(1,0,2);
        Assert.That(version.ToString(), Is.EqualTo("1.0.2"));
    }

    [Test]
    public void If_implicit_version_to_string_Then_cast_should_work()
    {
        var version = new DocumentVersion(1,0,2);

        string versionString = version.ToString();
        Assert.That(versionString, Is.EqualTo("1.0.2"));
    }
}