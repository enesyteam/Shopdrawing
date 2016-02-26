using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace Microsoft.Expression.Project
{
	public class FrameworkNameEqualityComparer : IEqualityComparer<FrameworkName>
	{
		private static FrameworkNameEqualityComparer instance;

		public static FrameworkNameEqualityComparer Instance
		{
			get
			{
				if (FrameworkNameEqualityComparer.instance == null)
				{
					FrameworkNameEqualityComparer.instance = new FrameworkNameEqualityComparer();
				}
				return FrameworkNameEqualityComparer.instance;
			}
		}

		private FrameworkNameEqualityComparer()
		{
		}

		public static bool AreEquivalent(FrameworkName frameworkName1, FrameworkName frameworkName2)
		{
			return FrameworkNameEqualityComparer.Instance.Equals(frameworkName1, frameworkName2);
		}

		private static bool CompareIdentifiers(string identifier1, string identifier2)
		{
			return string.Equals(identifier1, identifier2, StringComparison.Ordinal);
		}

		private static bool CompareProfiles(string profile1, string profile2)
		{
			if (FrameworkNameEqualityComparer.IsDefaultOrClientProfile(profile1))
			{
				return FrameworkNameEqualityComparer.IsDefaultOrClientProfile(profile2);
			}
			return string.Equals(profile1, profile2, StringComparison.Ordinal);
		}

		private static bool CompareVersions(Version version1, Version version2)
		{
			return version1.Equals(version2);
		}

		public bool Equals(FrameworkName x, FrameworkName y)
		{
			return this.Equals(x, y, true, true, true);
		}

		public bool Equals(FrameworkName frameworkName1, FrameworkName frameworkName2, bool checkIdentifiers, bool checkVersion, bool checkProfile)
		{
			if (object.ReferenceEquals(frameworkName1, null))
			{
				return object.ReferenceEquals(frameworkName2, null);
			}
			if (object.ReferenceEquals(frameworkName2, null))
			{
				return false;
			}
			if (checkIdentifiers && !FrameworkNameEqualityComparer.CompareIdentifiers(frameworkName1.Identifier, frameworkName2.Identifier) || checkVersion && !FrameworkNameEqualityComparer.CompareVersions(frameworkName1.Version, frameworkName2.Version))
			{
				return false;
			}
			if (!checkProfile)
			{
				return true;
			}
			return FrameworkNameEqualityComparer.CompareProfiles(frameworkName1.Profile, frameworkName2.Profile);
		}

		public int GetHashCode(FrameworkName obj)
		{
			int hashCode = obj.Identifier.GetHashCode() ^ obj.Version.GetHashCode();
			if (!FrameworkNameEqualityComparer.IsDefaultOrClientProfile(obj.Profile))
			{
				hashCode = hashCode ^ obj.Profile.GetHashCode();
			}
			return hashCode;
		}

		private static bool IsDefaultOrClientProfile(string profile)
		{
			if (string.IsNullOrEmpty(profile))
			{
				return true;
			}
			return string.Equals(profile, "Client", StringComparison.Ordinal);
		}
	}
}