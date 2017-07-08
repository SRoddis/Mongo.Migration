using FluentAssertions;
using Mongo.Migration.Documents;
using NUnit.Framework;

namespace Mongo.Migration.Test.Documents
{
    [TestFixture]
    public class DocumentVersion_When_casting
    {
        [Test]
        public void If_implicit_string_to_version_Then_cast_should_work()
        {
            DocumentVersion version = "1.0.2";

            version.ToString().Should().Be("1.0.2");
        }

        [Test]
        public void If_implicit_version_to_string_Then_cast_should_work()
        {
            var version = new DocumentVersion("1.0.2");

            string versionString = version;

            versionString.Should().Be("1.0.2");
        }
    }
}