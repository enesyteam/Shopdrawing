using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Licensing
{
	public class LicenseState
	{
		public int DaysLeft
		{
			get;
			private set;
		}

		public bool FullyLicensed
		{
			get
			{
				if (!this.IsLicensed)
				{
					return false;
				}
				if (!this.IsActivatable)
				{
					return true;
				}
				return this.IsActivated;
			}
		}

		public bool IsActivatable
		{
			get;
			private set;
		}

		public bool IsActivated
		{
			get;
			private set;
		}

		public bool IsExpired
		{
			get;
			private set;
		}

		public bool IsLicensed
		{
			get;
			private set;
		}

		public bool IsTrial
		{
			get;
			private set;
		}

		public bool LicenseExpires
		{
			get;
			private set;
		}

		private LicenseState()
		{
		}

		public static LicenseState Expired(bool isActivatable)
		{
			LicenseState licenseState = new LicenseState()
			{
				IsLicensed = false,
				IsTrial = false,
				IsExpired = true,
				IsActivatable = isActivatable
			};
			return licenseState;
		}

		public static LicenseState FullLicense()
		{
			LicenseState licenseState = new LicenseState()
			{
				IsLicensed = true,
				IsTrial = false,
				IsExpired = false,
				LicenseExpires = false,
				IsActivated = true,
				IsActivatable = false
			};
			return licenseState;
		}

		public static LicenseState Licensed(bool activatable)
		{
			LicenseState licenseState = new LicenseState()
			{
				IsLicensed = true,
				IsTrial = false,
				IsExpired = false,
				LicenseExpires = false,
				IsActivated = false,
				IsActivatable = activatable
			};
			return licenseState;
		}

		public static LicenseState TemporaryLicense(bool activatable, int daysLeft)
		{
			LicenseState licenseState = new LicenseState()
			{
				IsLicensed = true,
				DaysLeft = daysLeft,
				LicenseExpires = true,
				IsActivatable = activatable
			};
			return licenseState;
		}

		public static LicenseState Trial(bool isActivatable, int daysLeft)
		{
			LicenseState licenseState = new LicenseState()
			{
				IsLicensed = false,
				IsTrial = true,
				IsExpired = false,
				DaysLeft = daysLeft,
				IsActivatable = isActivatable
			};
			return licenseState;
		}
	}
}