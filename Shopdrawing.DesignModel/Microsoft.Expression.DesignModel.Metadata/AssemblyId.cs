using System;
using System.Globalization;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal class AssemblyId : IAssemblyId
	{
		private string fullName;

		private string name;

		private System.Version version;

		private byte[] publicKeyToken;

		public string FullName
		{
			get
			{
				return this.fullName;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public System.Version Version
		{
			get
			{
				return this.version;
			}
		}

		public AssemblyId(string fullName)
		{
			this.fullName = fullName;
			string[] strArrays = fullName.Split(new char[] { ',' });
			if ((int)strArrays.Length > 0)
			{
				this.name = strArrays[0].Trim();
			}
			if ((int)strArrays.Length > 1)
			{
				for (int i = 1; i < (int)strArrays.Length; i++)
				{
					string[] strArrays1 = strArrays[i].Trim().Split(new char[] { '=' });
					if ((int)strArrays1.Length == 2)
					{
						string str = strArrays1[0].Trim();
						string str1 = strArrays1[1].Trim();
						string str2 = str;
						string str3 = str2;
						if (str2 != null)
						{
							if (str3 == "Version")
							{
								this.version = new System.Version(str1);
							}
							else if (str3 == "PublicKeyToken")
							{
								this.publicKeyToken = AssemblyId.ParseByteArray(str1);
							}
						}
					}
				}
			}
		}

		public byte[] GetPublicKeyToken()
		{
			return this.publicKeyToken;
		}

		private static byte[] ParseByteArray(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return new byte[0];
			}
			byte[] numArray = new byte[value.Length >> 1];
			for (int i = 0; i < (int)numArray.Length; i++)
			{
				numArray[i] = byte.Parse(value.Substring(i << 1, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
			}
			return numArray;
		}

		public override string ToString()
		{
			return this.fullName;
		}
	}
}