using Mongo.Migration.Documents;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Documents;

[TestFixture]
public class DocumentVersionWhenCompare
{
    private readonly DocumentVersion _equalLowerVersion = new(0,0,1);

    private readonly DocumentVersion _higherVersion = new(0, 0, 2);

    private readonly DocumentVersion _lowerVersion = new(0, 0, 1);

    [Test]
    public void If_higherVersion_lte_equalLowerVersion_Then_false()
    {
        bool result = _higherVersion <= _lowerVersion;

        Assert.That(result, Is.False);
    }

    [Test]
    public void If_lowerVersion_gt_higherVersion_Then_false()
    {
        bool result = _lowerVersion > _higherVersion;

        Assert.That(result, Is.False);
    }

    [Test]
    public void If_lowerVersion_gte_equalLowerVersion_Then_true()
    {
        bool result = _lowerVersion >= _equalLowerVersion;

        Assert.That(result, Is.True);
    }

    [Test]
    public void If_lowerVersion_gte_higherVersion_Then_false()
    {
        bool result = _lowerVersion >= _higherVersion;

        Assert.That(result, Is.False);
    }

    [Test]
    public void If_lowerVersion_lt_higherVersion_Then_true()
    {
        bool result = _lowerVersion < _higherVersion;

        Assert.That(result, Is.True);
    }

    [Test]
    public void If_lowerVersion_lte_equalLowerVersion_Then_true()
    {
        bool result = _lowerVersion <= _equalLowerVersion;

        Assert.That(result, Is.True);
    }
}