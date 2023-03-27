using FluentAssertions;
using Mongo.Migration.Documents;
using NUnit.Framework;

namespace Mongo.Migration.Test.Documents
{
    [TestFixture]
    public class DocumentWhenCreatingTest
    {
        [Test]
        public void Then_document_can_be_created()
        {
            IDocument document = new Document();

            document.Should().BeOfType<Document>();
        }

        [Test]
        public void Then_document_has_a_version()
        {
            IDocument document = new Document();

            var version = document.Version;

            version.Should().Be("0.0.0");
        }
    }
}