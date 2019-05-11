using System;
using FluentAssertions;
using Mongo.Migration.Documents;
using Mongo.Migration.Exceptions;
using NUnit.Framework;

namespace Mongo.Migration.Test.Documents
{
    [TestFixture]
    public class DocumentVersion_When_creating
    {
        [Test]
        public void If_Default_Then_version_is_default_value()
        {
            DocumentVersion version = DocumentVersion.Default();

            version.ToString().Should().Be("0.0.0");
        }

        [Test]
        public void If_first_part_contains_char_Then_exception_is_thrown()
        {
            Action act = () => new DocumentVersion("a.0.0");

            act.Should().Throw<InvalidVersionValueException>();
        }

        [Test]
        public void If_new_version_with_int_Then_version_string_should_be_same()
        {
            var version = new DocumentVersion(1, 0, 2);

            version.ToString().Should().Be("1.0.2");
        }

        [Test]
        public void If_new_version_with_string_Then_version_string_should_be_same()
        {
            var version = new DocumentVersion("1.0.2");

            version.ToString().Should().Be("1.0.2");
        }

        [Test]
        public void If_second_part_contains_char_Then_exception_is_thrown()
        {
            Action act = () => new DocumentVersion("0.a.0");

            act.Should().Throw<InvalidVersionValueException>();
        }

        [Test]
        public void If_third_part_contains_char_Then_exception_is_thrown()
        {
            Action act = () => new DocumentVersion("0.0.a");

            act.Should().Throw<InvalidVersionValueException>();
        }

        [Test]
        public void If_version_string_is_to_long_Then_exception_is_thrown()
        {
            Action act = () => new DocumentVersion("0.0.0.0");

            act.Should().Throw<VersionStringToLongException>();
        }
    }
}