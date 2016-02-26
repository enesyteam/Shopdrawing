using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Adds;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.MetadataReader
{
	internal class MetadataOnlyAssembly : Assembly, IAssembly2, IDisposable
	{
		private readonly Module[] m_modules;

		private readonly MetadataOnlyModule m_manifestModule;

		private readonly string m_manifestFile;

		private readonly AssemblyName m_name;

		public override string CodeBase
		{
			get
			{
				return MetadataOnlyAssembly.GetCodeBaseFromManifestModule(this.m_manifestModule);
			}
		}

		public override MethodInfo EntryPoint
		{
			get
			{
				Token token = this.m_manifestModule.RawMetadata.ReadEntryPointToken();
				if (token.IsNil)
				{
					return null;
				}
				System.Reflection.Adds.TokenType tokenType = token.TokenType;
				if (tokenType == System.Reflection.Adds.TokenType.FieldDef)
				{
					throw new NotImplementedException();
				}
				if (tokenType != System.Reflection.Adds.TokenType.MethodDef)
				{
					throw new InvalidOperationException(MetadataStringTable.InvalidMetadata);
				}
				return (MethodInfo)this.ManifestModule.ResolveMethod(token.Value);
			}
		}

		public override string FullName
		{
			get
			{
				return this.m_name.FullName;
			}
		}

		public override string Location
		{
			get
			{
				return this.m_manifestFile;
			}
		}

		public override Module ManifestModule
		{
			get
			{
				return this.m_modules[0];
			}
		}

		internal MetadataOnlyModule ManifestModuleInternal
		{
			get
			{
				return this.m_manifestModule;
			}
		}

		public override bool ReflectionOnly
		{
			get
			{
				return false;
			}
		}

		public ITypeUniverse TypeUniverse
		{
			get
			{
				return this.m_manifestModule.AssemblyResolver;
			}
		}

		internal MetadataOnlyAssembly(MetadataOnlyModule manifestModule, string manifestFile) : this(new MetadataOnlyModule[] { manifestModule }, manifestFile)
		{
		}

		internal MetadataOnlyAssembly(MetadataOnlyModule[] modules, string manifestFile)
		{
			MetadataOnlyAssembly.VerifyModules(modules);
			this.m_manifestModule = modules[0];
			this.m_name = AssemblyNameHelper.GetAssemblyName(this.m_manifestModule);
			this.m_manifestFile = manifestFile;
			MetadataOnlyModule[] metadataOnlyModuleArray = modules;
			for (int num = 0; num < (int)metadataOnlyModuleArray.Length; num++)
			{
				metadataOnlyModuleArray[num].SetContainingAssembly(this);
			}
			List<Module> modules1 = new List<Module>(modules);
			foreach (string fileNamesFromFilesTable in MetadataOnlyAssembly.GetFileNamesFromFilesTable(this.m_manifestModule, false))
			{
				if (modules1.Find((Module i) => i.Name.Equals(fileNamesFromFilesTable, StringComparison.OrdinalIgnoreCase)) != null)
				{
					continue;
				}
				Module module = this.m_manifestModule.AssemblyResolver.ResolveModule(this, fileNamesFromFilesTable);
				if (module == null)
				{
					throw new InvalidOperationException(MetadataStringTable.ResolverMustResolveToValidModule);
				}
				if (module.Assembly != this)
				{
					throw new InvalidOperationException(MetadataStringTable.ResolverMustSetAssemblyProperty);
				}
				modules1.Add(module);
			}
			this.m_modules = modules1.ToArray();
		}

		private static FileStream[] ConvertFileNamesToStreams(string[] filenames)
		{
			return Array.ConvertAll<string, FileStream>(filenames, (string n) => new FileStream(n, FileMode.Open, FileAccess.Read, FileShare.Read));
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && this.m_modules != null)
			{
				Module[] mModules = this.m_modules;
				for (int i = 0; i < (int)mModules.Length; i++)
				{
					IDisposable disposable = mModules[i] as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}
		}

		public override bool Equals(object obj)
		{
			Assembly assembly = obj as Assembly;
			if (assembly == null)
			{
				return false;
			}
			return this.ManifestModule.Equals(assembly.ManifestModule);
		}

		internal static Token GetAssemblyToken(MetadataOnlyModule module)
		{
			int num;
			if (((IMetadataAssemblyImport)module.RawImport).GetAssemblyFromScope(out num) != 0)
			{
				return Token.Nil;
			}
			return new Token(num);
		}

		internal static string GetCodeBaseFromManifestModule(MetadataOnlyModule manifestModule)
		{
			string str;
			string fullyQualifiedName = manifestModule.FullyQualifiedName;
			if (!Utility.IsValidPath(fullyQualifiedName))
			{
				return string.Empty;
			}
			try
			{
				str = (new Uri(fullyQualifiedName)).ToString();
			}
			catch (Exception exception)
			{
				throw;
			}
			return str;
		}

		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return this.m_manifestModule.GetCustomAttributeData(MetadataOnlyAssembly.GetAssemblyToken(this.m_manifestModule));
		}

		public override Type[] GetExportedTypes()
		{
			Type[] types = this.GetTypes();
			List<Type> types1 = new List<Type>();
			Type[] typeArray = types;
			for (int i = 0; i < (int)typeArray.Length; i++)
			{
				Type type = typeArray[i];
				if (type.IsVisible)
				{
					types1.Add(type);
				}
			}
			return types1.ToArray();
		}

		public override FileStream GetFile(string name)
		{
			Module module = this.GetModule(name);
			if (module == null)
			{
				return null;
			}
			return new FileStream(module.FullyQualifiedName, FileMode.Open, FileAccess.Read, FileShare.Read);
		}

		private static List<string> GetFileNamesFromFilesTable(MetadataOnlyModule manifestModule, bool getResources)
		{
			int num;
			int num1;
			int num2;
			UnusedIntPtr unusedIntPtr;
			uint num3;
			CorFileFlags corFileFlag;
			HCORENUM hCORENUM = new HCORENUM();
			StringBuilder stringBuilder = new StringBuilder();
			List<string> strs = new List<string>();
			IMetadataAssemblyImport rawImport = (IMetadataAssemblyImport)manifestModule.RawImport;
			try
			{
				while (true)
				{
					rawImport.EnumFiles(ref hCORENUM, out num, 1, out num1);
					if (num1 == 0)
					{
						break;
					}
					rawImport.GetFileProps(num, null, 0, out num2, out unusedIntPtr, out num3, out corFileFlag);
					if (getResources || corFileFlag != CorFileFlags.ContainsNoMetaData)
					{
						if (num2 > stringBuilder.Capacity)
						{
							stringBuilder.Capacity = num2;
						}
						rawImport.GetFileProps(num, stringBuilder, num2, out num2, out unusedIntPtr, out num3, out corFileFlag);
						strs.Add(stringBuilder.ToString());
					}
				}
			}
			finally
			{
				hCORENUM.Close(rawImport);
			}
			return strs;
		}

		public override FileStream[] GetFiles(bool getResourceModules)
		{
			List<string> strs = new List<string>();
			Module[] mModules = this.m_modules;
			for (int i = 0; i < (int)mModules.Length; i++)
			{
				strs.Add(mModules[i].FullyQualifiedName);
			}
			if (getResourceModules)
			{
				string directoryName = Path.GetDirectoryName(this.m_manifestFile);
				foreach (string fileNamesFromFilesTable in MetadataOnlyAssembly.GetFileNamesFromFilesTable(this.m_manifestModule, true))
				{
					strs.Add(Path.Combine(directoryName, fileNamesFromFilesTable));
				}
			}
			return MetadataOnlyAssembly.ConvertFileNamesToStreams(strs.ToArray());
		}

		public override int GetHashCode()
		{
			return this.m_modules[0].GetHashCode();
		}

		public override Module[] GetLoadedModules(bool getResourceModules)
		{
			return this.m_modules;
		}

		public override string[] GetManifestResourceNames()
		{
			int num;
			int num1;
			int num2;
			int num3;
			uint num4;
			CorManifestResourceFlags corManifestResourceFlag;
			HCORENUM hCORENUM = new HCORENUM();
			StringBuilder stringBuilder = new StringBuilder();
			List<string> strs = new List<string>();
			IMetadataAssemblyImport rawImport = (IMetadataAssemblyImport)this.m_manifestModule.RawImport;
			try
			{
				while (true)
				{
					rawImport.EnumManifestResources(ref hCORENUM, out num, 1, out num1);
					if (num1 == 0)
					{
						break;
					}
					rawImport.GetManifestResourceProps(num, null, 0, out num2, out num3, out num4, out corManifestResourceFlag);
					if (num2 > stringBuilder.Capacity)
					{
						stringBuilder.Capacity = num2;
					}
					rawImport.GetManifestResourceProps(num, stringBuilder, num2, out num2, out num3, out num4, out corManifestResourceFlag);
					strs.Add(stringBuilder.ToString());
				}
			}
			finally
			{
				hCORENUM.Close(rawImport);
			}
			return strs.ToArray();
		}

		public override Stream GetManifestResourceStream(Type type, string name)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (type != null)
			{
				string @namespace = type.Namespace;
				if (@namespace != null)
				{
					stringBuilder.Append(@namespace);
					if (name != null)
					{
						stringBuilder.Append(Type.Delimiter);
					}
				}
			}
			else if (name == null)
			{
				throw new ArgumentNullException("type");
			}
			if (name != null)
			{
				stringBuilder.Append(name);
			}
			return this.GetManifestResourceStream(stringBuilder.ToString());
		}

		public override Stream GetManifestResourceStream(string name)
		{
			int num;
			int num1;
			int num2;
			uint num3;
			CorManifestResourceFlags corManifestResourceFlag;
			int num4;
			UnusedIntPtr unusedIntPtr;
			uint num5;
			CorFileFlags corFileFlag;
			IMetadataAssemblyImport rawImport = (IMetadataAssemblyImport)this.m_manifestModule.RawImport;
			rawImport.FindManifestResourceByName(name, out num);
			if ((new Token(num)).IsNil)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder(name.Length + 1);
			rawImport.GetManifestResourceProps(num, stringBuilder, stringBuilder.Capacity, out num1, out num2, out num3, out corManifestResourceFlag);
			Token token = new Token(num2);
			if (token.TokenType != System.Reflection.Adds.TokenType.File)
			{
				if (token.TokenType != System.Reflection.Adds.TokenType.AssemblyRef)
				{
					throw new ArgumentException(MetadataStringTable.InvalidMetadata);
				}
				throw new NotImplementedException();
			}
			if (token.IsNil)
			{
				byte[] numArray = this.m_manifestModule.RawMetadata.ReadResource((long)num3);
				return new MemoryStream(numArray);
			}
			rawImport.GetFileProps(token.Value, null, 0, out num4, out unusedIntPtr, out num5, out corFileFlag);
			StringBuilder stringBuilder1 = new StringBuilder(num4);
			rawImport.GetFileProps(token.Value, stringBuilder1, num4, out num4, out unusedIntPtr, out num5, out corFileFlag);
			string directoryName = Path.GetDirectoryName(this.Location);
			string str = Path.Combine(directoryName, stringBuilder1.ToString());
			return new FileStream(str, FileMode.Open);
		}

		public override Module GetModule(string name)
		{
			Module[] mModules = this.m_modules;
			for (int i = 0; i < (int)mModules.Length; i++)
			{
				Module module = mModules[i];
				if (module.ScopeName.Equals(name, StringComparison.OrdinalIgnoreCase))
				{
					return module;
				}
			}
			return null;
		}

		public override Module[] GetModules(bool getResourceModules)
		{
			return this.m_modules;
		}

		public override AssemblyName GetName()
		{
			return this.m_name;
		}

		public override AssemblyName GetName(bool copiedName)
		{
			throw new NotImplementedException();
		}

		public override AssemblyName[] GetReferencedAssemblies()
		{
			Token token;
			int num;
			IMetadataAssemblyImport rawImport = (IMetadataAssemblyImport)this.m_manifestModule.RawImport;
			List<AssemblyName> assemblyNames = new List<AssemblyName>();
			HCORENUM hCORENUM = new HCORENUM();
			try
			{
				while (true)
				{
					int num1 = rawImport.EnumAssemblyRefs(ref hCORENUM, out token, 1, out num);
					Marshal.ThrowExceptionForHR(num1);
					if (num == 0)
					{
						break;
					}
					assemblyNames.Add(AssemblyNameHelper.GetAssemblyNameFromRef(token, this.m_manifestModule, rawImport));
				}
			}
			finally
			{
				hCORENUM.Close(rawImport);
			}
			return assemblyNames.ToArray();
		}

		public override Type GetType(string name)
		{
			return this.GetType(name, false, false);
		}

		public override Type GetType(string name, bool throwOnError)
		{
			return this.GetType(name, throwOnError, false);
		}

		public override Type GetType(string name, bool throwOnError, bool ignoreCase)
		{
			Type type = null;
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			for (int i = 0; i < (int)this.m_modules.Length; i++)
			{
				type = this.m_modules[i].GetType(name, false, ignoreCase);
				if (type != null)
				{
					return type;
				}
			}
			Type type1 = this.m_manifestModule.Policy.TryTypeForwardResolution(this, name, ignoreCase);
			if (type1 != null)
			{
				return type1;
			}
			if (throwOnError)
			{
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				string cannotFindTypeInModule = MetadataStringTable.CannotFindTypeInModule;
				object[] objArray = new object[] { name, this.m_modules[0].ScopeName };
				throw new TypeLoadException(string.Format(invariantCulture, cannotFindTypeInModule, objArray));
			}
			return null;
		}

		public override Type[] GetTypes()
		{
			List<Type> types = new List<Type>();
			Module[] mModules = this.m_modules;
			for (int i = 0; i < (int)mModules.Length; i++)
			{
				types.AddRange(mModules[i].GetTypes());
			}
			return types.ToArray();
		}

		private static void VerifyModules(MetadataOnlyModule[] modules)
		{
			if (modules == null || (int)modules.Length < 1)
			{
				throw new ArgumentException(MetadataStringTable.ManifestModuleMustBeProvided);
			}
			if (MetadataOnlyAssembly.GetAssemblyToken(modules[0]) == Token.Nil)
			{
				throw new ArgumentException(MetadataStringTable.NoAssemblyManifest);
			}
			for (int i = 1; i < (int)modules.Length; i++)
			{
				if (MetadataOnlyAssembly.GetAssemblyToken(modules[i]) != Token.Nil)
				{
					throw new ArgumentException(MetadataStringTable.ExtraAssemblyManifest);
				}
			}
		}
	}
}