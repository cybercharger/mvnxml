using System;
using System.Collections.Generic;

namespace mvndepxml
{
    public class MavenDepInfo
    {
        public const string PropGroupId = "groupId";
        public const string PropArtifactId = "artifactId";
        public const string PropPackage = "package";
        public const string PropVersion = "version";
        public const string PropScope = "scope";
        public const string PropComments = "comments";
        public static readonly string[] PropNames = { PropGroupId, PropArtifactId, PropPackage, PropVersion, PropScope, PropComments };


        private const char SectionDelimiter = ':';
        private const char CommentsDelimeter = '-';

        private const char ReferenceStart = '(';
        private const char ReferenceEnd = ')';

        public bool IsReference { get; private set; }
        public string GroupId { get { return  GetProperty(PropGroupId); } }

        public string AritifactId { get { return GetProperty(PropArtifactId); } }

        public string Package { get { return GetProperty(PropPackage); } }

        public string Version { get { return GetProperty(PropVersion); } }

        public string Scope { get { return GetProperty(PropScope); } }

        public string Comments { get { return GetProperty(PropComments); } }

        private readonly IDictionary<string, string> _properties = new Dictionary<string, string>();

        public MavenDepInfo(string content)
        {
            if (string.IsNullOrEmpty(content)) throw new ArgumentNullException("content");
            if (content.StartsWith(ReferenceStart.ToString()))
            {
                IsReference = true;
                content = content.TrimStart(ReferenceStart).TrimEnd(ReferenceEnd);
            }
            var sections = content.Split(SectionDelimiter);
            for (var i = 0; i < sections.Length; ++i)
            {
                _properties[PropNames[i]] = sections[i];
            }

            if (Scope == null || !Scope.Contains(CommentsDelimeter.ToString())) return;
            var sc = Scope.Split(CommentsDelimeter);
            _properties[PropScope] = sc[0].Trim();
            _properties[PropComments] = sc[1].Trim();
        }

        public string GetProperty(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;
            return _properties.ContainsKey(key) ? _properties[key] : null;
        }
    }
}