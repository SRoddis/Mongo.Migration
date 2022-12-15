using System;

using Mongo.Migration.Exceptions;

namespace Mongo.Migration.Documents
{
    public struct DocumentVersion : IComparable<DocumentVersion>
    {
        private const char VERSION_SPLIT_CHAR = '.';

        private const int MAX_LENGTH = 3;

        public int Major { get; init; }

        public int Minor { get; init; }

        public int Revision { get; init; }

        public DocumentVersion(string version)
        {
            string[] versionParts = version.Split(VERSION_SPLIT_CHAR);

            if (versionParts.Length != MAX_LENGTH)
            {
                throw new VersionStringToLongException(version);
            }

            this.Major = ParseVersionPart(versionParts[0]);

            this.Minor = ParseVersionPart(versionParts[1]);

            this.Revision = ParseVersionPart(versionParts[2]);
        }

        public DocumentVersion(int major, int minor, int revision)
        {
            this.Major = major;
            this.Minor = minor;
            this.Revision = revision;
        }

        public static DocumentVersion Default()
        {
            return default(DocumentVersion);
        }

        public static DocumentVersion Empty()
        {
            return new DocumentVersion(-1, 0, 0);
        }

        public static implicit operator DocumentVersion(string version)
        {
            return new DocumentVersion(version);
        }

        public static implicit operator string(DocumentVersion documentVersion)
        {
            return documentVersion.ToString();
        }

        public override string ToString()
        {
            return $"{this.Major}.{this.Minor}.{this.Revision}";
        }

        public int CompareTo(DocumentVersion other)
        {
            if (this.Equals(other))
            {
                return 0;
            }

            return this > other ? 1 : -1;
        }

        public static bool operator ==(DocumentVersion a, DocumentVersion b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(DocumentVersion a, DocumentVersion b)
        {
            return !(a == b);
        }

        public static bool operator >(DocumentVersion a, DocumentVersion b)
        {
            return a.Major > b.Major
                   || (a.Major == b.Major && a.Minor > b.Minor)
                   || (a.Major == b.Major && a.Minor == b.Minor && a.Revision > b.Revision);
        }

        public static bool operator <(DocumentVersion a, DocumentVersion b)
        {
            return a != b && !(a > b);
        }

        public static bool operator <=(DocumentVersion a, DocumentVersion b)
        {
            return a == b || a < b;
        }

        public static bool operator >=(DocumentVersion a, DocumentVersion b)
        {
            return a == b || a > b;
        }

        public bool Equals(DocumentVersion other)
        {
            return other.Major == this.Major && other.Minor == this.Minor && other.Revision == this.Revision;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (obj.GetType() != typeof(DocumentVersion))
            {
                return false;
            }

            return this.Equals((DocumentVersion)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = this.Major;
                result = (result * 397) ^ this.Minor;
                result = (result * 397) ^ this.Revision;
                return result;
            }
        }

        private static int ParseVersionPart(string value)
        {
            string revisionString = value;
            if (!int.TryParse(revisionString, out var target))
            {
                throw new InvalidVersionValueException(revisionString);
            }

            return target;
        }
    }
}