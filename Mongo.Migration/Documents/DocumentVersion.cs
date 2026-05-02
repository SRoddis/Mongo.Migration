namespace Mongo.Migration.Documents;

public readonly record struct DocumentVersion : IComparable<DocumentVersion>
{
    private const char VersionSplitChar = '.';

    public int Major { get; }

    public int Minor { get; }

    public int Revision { get; }

    public DocumentVersion(int major, int minor, int revision)
    {
        Major = major;
        Minor = minor;
        Revision = revision;
    }

    public static readonly DocumentVersion Default = new(0,0,0);

    public static readonly DocumentVersion Empty = new(-1, 0, 0);

    public static implicit operator DocumentVersion(string version) => Parse(version.AsSpan());

    public static implicit operator DocumentVersion(ReadOnlySpan<char> versionSpan) => Parse(versionSpan);

    public static implicit operator string(DocumentVersion documentVersion) => documentVersion.ToString();

    public override string ToString() => $"{Major}.{Minor}.{Revision}";

    public int CompareTo(DocumentVersion other)
    {
        int compare = Major.CompareTo(other.Major);
        if (compare != 0)
        {
            return compare;
        }

        compare = Minor.CompareTo(other.Minor);
        if (compare != 0)
        {
            return compare;
        }

        return Revision.CompareTo(other.Revision);
    }

    public static bool operator >(DocumentVersion a, DocumentVersion b) => a.CompareTo(b) > 0;

    public static bool operator <(DocumentVersion a, DocumentVersion b) => a.CompareTo(b) < 0;

    public static bool operator >=(DocumentVersion a, DocumentVersion b) => a.CompareTo(b) >= 0;

    public static bool operator <=(DocumentVersion a, DocumentVersion b) => a.CompareTo(b) <= 0;

    public static DocumentVersion Parse(ReadOnlySpan<char> versionSpan)
    {
        int[] versionParts = new int[3];
        int versionPartCount = 0;
        int startIndex = 0;

        for (int i = 0; i < versionSpan.Length; i++)
        {
            if (versionSpan[i] == VersionSplitChar)
            {
                versionParts[versionPartCount++] = int.Parse(versionSpan[startIndex..i]);
                startIndex = i + 1;
            }
        }

        versionParts[versionPartCount] = int.Parse(versionSpan[startIndex..]);

        return new DocumentVersion(versionParts[0], versionParts[1], versionParts[2]);
    }
}