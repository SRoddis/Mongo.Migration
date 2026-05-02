using Mongo.Migration.Documents;
using NUnit.Framework;

namespace Mongo.Migration.Tests.Documents;

[TestFixture]
public class DocumentVersionWhenCreating
{
    [Test]
    public void If_Empty_Then_version_is_empty_value()
    {
        DocumentVersion version = DocumentVersion.Empty;
        Assert.That(version.ToString(), Is.EqualTo("-1.0.0"));
    }

    [Test]
    public void If_Default_Then_version_is_default_value()
    {
        DocumentVersion version = DocumentVersion.Default;

        Assert.That(version.ToString(), Is.EqualTo("0.0.0"));
    }

    [Test]
    public void If_first_part_contains_char_Then_exception_is_thrown()
    {
        Assert.Throws<FormatException>(() => DocumentVersion.Parse("a.0.0"));
    }

    [Test]
    public void If_new_version_with_int_Then_version_string_should_be_same()
    {
        var version = new DocumentVersion(1, 0, 2);
        Assert.That(version.ToString(), Is.EqualTo("1.0.2"));
    }

    [Test]
    public void If_new_version_with_string_Then_version_string_should_be_same()
    {
        var version = DocumentVersion.Parse("1.0.2");
        Assert.That(version.ToString(), Is.EqualTo("1.0.2"));
    }

    [Test]
    public void If_second_part_contains_char_Then_exception_is_thrown()
    {
        Assert.Throws<FormatException>(() => DocumentVersion.Parse("0.a.0"));
    }

    [Test]
    public void If_third_part_contains_char_Then_exception_is_thrown()
    {
        Assert.Throws<FormatException>(() => DocumentVersion.Parse("0.0.a"));
    }

    [Test]
    public void If_version_string_is_too_long_Then_exception_is_thrown()
    {
        Assert.Throws<IndexOutOfRangeException>(() => DocumentVersion.Parse("0.0.0.0"));
    }

    [Test]
    public void If_version_string_is_too_short1_then_default()
    {
        var version = DocumentVersion.Parse("33");
        Assert.That(version.ToString(), Is.EqualTo("33.0.0"));
    }

    [Test]
    public void If_version_string_is_too_short2_then_default()
    {
        var version = DocumentVersion.Parse("42.27");
        Assert.That(version.ToString(), Is.EqualTo("42.27.0"));
    }
}