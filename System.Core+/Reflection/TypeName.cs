using System;
using System.Text.RegularExpressions;

namespace System.Reflection
{
	public class TypeName
	{
		private const string TypeNamePattern = @"[A-Za-z<>][A-Za-z0-9\+`\[\]_<>]*";
		private static readonly Regex pattern = new Regex(@"^(?<type>" + TypeNamePattern + @"(\." + TypeNamePattern + @")*),\s*(?<assembly>[A-Za-z][A-Za-z0-9]*(\.[A-Za-z][A-Za-z0-9]*)*),\s*Version=(?<version>\d+\.\d+\.\d+\.\d+),\s*Culture=(?<culture>\w{2}|neutral),\s*PublicKeyToken=(?<pkt>null|[a-h0-9]{16})$", RegexOptions.Compiled);

		private string assembly;
        private string ns;
		private string type;
		private string version;
		private string culture;
		private string publicKeyToken;

		public string Assembly
		{
			get { return assembly; }
		}

        public string Namespace
        {
            get { return this.ns; }
        }

		public string Type
		{
			get { return this.type; }
		}

        public string FullType
        {
            get { return "{0}.{1}".FormatString(Namespace, Type); }
        }

		public string Version
		{
			get { return this.version; }
		}

		public string Culture
		{
			get { return this.culture; }
		}

		public string PublicKeyToken
		{
			get { return this.publicKeyToken; }
		}

		public TypeName(Type type)
			: this(type.AssemblyQualifiedName) { }

		public TypeName(string assemblyQualifiedTypeName)
		{
			if (assemblyQualifiedTypeName == null)
				throw new ArgumentNullException("assemblyQualifiedTypeName");

			Match match = pattern.Match(assemblyQualifiedTypeName);

			if (!match.Success)
				throw new FormatException("assemblyQualifiedTypeName does not match the format <TypeName>,<AssemblyName>,Version=<Major.Minor.Revision.Build>,Culture=<neutral|CultureInfo>,PublicKeyToken=<null|PublicKeyToken>");

			this.assembly = match.Groups["assembly"].Value;
			this.version = match.Groups["version"].Value;
			this.culture = match.Groups["culture"].Value;
			this.publicKeyToken = match.Groups["pkt"].Value;

            var fullType = match.Groups["type"].Value;
            var dot = fullType.LastIndexOf('.');

            this.ns = fullType.Substring(0, dot);
            this.type = fullType.Substring(dot + 1, fullType.Length - dot - 1);
		}

		public string GetAssemblyNameString()
		{
			return string.Format("{0}, Version={1}, Culture={2}, PublicKeyToken={3}", this.assembly, this.version, this.culture, this.publicKeyToken);
		}

		public string GetTypeNameString()
		{
			return string.Format("{0}, {1}", this.FullType, this.GetAssemblyNameString());
		}

		public string GetAssemblyQualifiedNameWithoutVersion()
		{
			return String.Format("{0}, {1}", this.FullType, this.assembly);
		}

		public override string ToString()
		{
			return this.GetTypeNameString();
		}

		public bool Equals(TypeName typeName, bool ignoreVersion)
		{
			if (!ignoreVersion)
				return this.Equals(typeName);
			else
				return typeName.Assembly.Equals(this.Assembly) && typeName.FullType.Equals(this.FullType) && typeName.Culture.Equals(this.Culture) && typeName.PublicKeyToken.Equals(this.PublicKeyToken);
		}

		public override bool Equals(object obj)
		{
			return obj is TypeName && obj.GetHashCode() == this.GetHashCode();
		}

		public override int GetHashCode()
		{
			return Assembly.GetHashCode() ^ FullType.GetHashCode() ^ Version.GetHashCode() ^ Culture.GetHashCode() ^ PublicKeyToken.GetHashCode();
		}
	}
}
