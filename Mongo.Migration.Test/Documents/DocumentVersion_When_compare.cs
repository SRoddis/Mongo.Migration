using FluentAssertions;

using Mongo.Migration.Documents;

using NUnit.Framework;

namespace Mongo.Migration.Test.Documents
{
    [TestFixture]
    public class DocumentVersion_When_compare
    {
        private readonly DocumentVersion equalLowerVersion = new DocumentVersion("0.0.1");

        private readonly DocumentVersion higherVersion = new DocumentVersion("0.0.2");

        private readonly DocumentVersion lowerVersion = new DocumentVersion("0.0.1");

        [Test]
        public void If_higherVersion_lte_equalLowerVersion_Then_false()
        {
            bool result = this.higherVersion <= this.lowerVersion;

            result.Should().BeFalse();
        }

        [Test]
        public void If_lowerVersion_gt_higherVersion_Then_false()
        {
            bool result = this.lowerVersion > this.higherVersion;

            result.Should().BeFalse();
        }

        [Test]
        public void If_lowerVersion_gte_equalLowerVersion_Then_true()
        {
            bool result = this.lowerVersion >= this.equalLowerVersion;

            result.Should().BeTrue();
        }

        [Test]
        public void If_lowerVersion_gte_higherVersion_Then_false()
        {
            bool result = this.lowerVersion >= this.higherVersion;

            result.Should().BeFalse();
        }

        [Test]
        public void If_lowerVersion_lt_higherVersion_Then_true()
        {
            bool result = this.lowerVersion < this.higherVersion;

            result.Should().BeTrue();
        }

        [Test]
        public void If_lowerVersion_lte_equalLowerVersion_Then_true()
        {
            bool result = this.lowerVersion <= this.equalLowerVersion;

            result.Should().BeTrue();
        }
    }
}