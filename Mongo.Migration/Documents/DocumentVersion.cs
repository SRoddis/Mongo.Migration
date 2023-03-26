using System;
using Mongo.Migration.Documents.Serializers;
using Mongo.Migration.Exceptions;
using MongoDB.Bson.Serialization;

namespace Mongo.Migration.Documents
{
    public struct DocumentVersion : IComparable<DocumentVersion>
    {
        private const char VERSION_SPLIT_CHAR = '.';
        private const int MAX_LENGTH = 3;
        public int Major { get; init; }
        public int Minor { get; init; }
        public int Revision { get; init; }

        static DocumentVersion()
        {
            try
            {
                BsonSerializer.RegisterSerializer(typeof(DocumentVersion), new DocumentVersionSerializer());
            }
            catch (Exception)
            {
            }
        }

        public DocumentVersion(string version)
        {
            var versionParts = version.Split(VERSION_SPLIT_CHAR);

            if (versionParts.Length != MAX_LENGTH)
            {
                throw new VersionStringToLongException(version);
            }

            Major = ParseVersionPart(versionParts[0]);

            Minor = ParseVersionPart(versionParts[1]);

            Revision = ParseVersionPart(versionParts[2]);
        }

        public DocumentVersion(int major, int minor, int revision)
        {
            Major = major;
            Minor = minor;
            Revision = revision;
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
            return $"{Major}.{Minor}.{Revision}";
        }

        public int CompareTo(DocumentVersion other)
        {
            if (Equals(other))
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

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is DocumentVersion version && Equals(version);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = Major;
                result = (result * 397) ^ Minor;
                result = (result * 397) ^ Revision;
                return result;
            }
        }

        private bool Equals(DocumentVersion other)
        {
            return other.Major == Major && other.Minor == Minor && other.Revision == Revision;
        }

        private static int ParseVersionPart(string value)
        {
            if (!int.TryParse(value, out var target))
            {
                throw new InvalidVersionValueException(value);
            }

            return target;
        }
    }
}