using System;
using System.Configuration.Assemblies;
using System.Reflection;
using System.Reflection.Adds;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.MetadataReader
{
	internal static class AssemblyNameHelper
	{
		private static ProcessorArchitecture CalculateProcArchIndex(PortableExecutableKinds pek, ImageFileMachine ifm)
		{
			if ((pek & PortableExecutableKinds.PE32Plus) == PortableExecutableKinds.PE32Plus)
			{
				ImageFileMachine imageFileMachine = ifm;
				if (imageFileMachine != ImageFileMachine.I386)
				{
					if (imageFileMachine == ImageFileMachine.IA64)
					{
						return ProcessorArchitecture.IA64;
					}
					if (imageFileMachine == ImageFileMachine.AMD64)
					{
						return ProcessorArchitecture.Amd64;
					}
				}
				else if ((pek & PortableExecutableKinds.ILOnly) == PortableExecutableKinds.ILOnly)
				{
					return ProcessorArchitecture.MSIL;
				}
			}
			else if (ifm == ImageFileMachine.I386)
			{
				if ((pek & PortableExecutableKinds.Required32Bit) != PortableExecutableKinds.Required32Bit && (pek & PortableExecutableKinds.ILOnly) == PortableExecutableKinds.ILOnly)
				{
					return ProcessorArchitecture.MSIL;
				}
				return ProcessorArchitecture.X86;
			}
			return ProcessorArchitecture.None;
		}

		public static AssemblyName GetAssemblyName(MetadataOnlyModule module)
		{
			PortableExecutableKinds portableExecutableKind;
			ImageFileMachine imageFileMachine;
			Token assemblyToken = MetadataOnlyAssembly.GetAssemblyToken(module);
			IMetadataAssemblyImport rawImport = (IMetadataAssemblyImport)module.RawImport;
			AssemblyNameHelper.AssemblyNameFromDefitionBuilder assemblyNameFromDefitionBuilder = new AssemblyNameHelper.AssemblyNameFromDefitionBuilder(assemblyToken, module.RawMetadata, rawImport);
			AssemblyName codeBaseFromManifestModule = assemblyNameFromDefitionBuilder.CalculateName();
			codeBaseFromManifestModule.CodeBase = MetadataOnlyAssembly.GetCodeBaseFromManifestModule(module);
			module.GetPEKind(out portableExecutableKind, out imageFileMachine);
			codeBaseFromManifestModule.ProcessorArchitecture = AssemblyNameHelper.CalculateProcArchIndex(portableExecutableKind, imageFileMachine);
			return codeBaseFromManifestModule;
		}

		public static AssemblyName GetAssemblyNameFromRef(Token assemblyRefToken, MetadataOnlyModule module, IMetadataAssemblyImport assemblyImport)
		{
			return (new AssemblyNameHelper.AssemblyNameFromRefBuilder(assemblyRefToken, module.RawMetadata, assemblyImport)).CalculateName();
		}

		private abstract class AssemblyNameBuilder : IDisposable
		{
			private readonly MetadataFile m_storage;

			protected readonly IMetadataAssemblyImport m_assemblyImport;

			protected EmbeddedBlobPointer m_publicKey;

			protected int m_cbPublicKey;

			protected int m_hashAlgId;

			protected StringBuilder m_szName;

			protected int m_chName;

			protected AssemblyNameFlags m_flags;

			protected AssemblyMetaData m_metadata;

			protected AssemblyNameBuilder(MetadataFile storage, IMetadataAssemblyImport assemblyImport)
			{
				this.m_storage = storage;
				this.m_assemblyImport = assemblyImport;
			}

			public AssemblyName CalculateName()
			{
				AssemblyName assemblyName = new AssemblyName();
				this.m_metadata = new AssemblyMetaData();
				this.m_metadata.Init();
				this.m_szName = null;
				this.m_chName = 0;
				this.Fetch();
				this.m_szName = new StringBuilder()
				{
					Capacity = this.m_chName
				};
				int mMetadata = (int)(this.m_metadata.cbLocale * 2);
				this.m_metadata.szLocale = new UnmanagedStringMemoryHandle(mMetadata);
				this.m_metadata.ulProcessor = 0;
				this.m_metadata.ulOS = 0;
				this.Fetch();
				assemblyName.CultureInfo = this.m_metadata.Locale;
				byte[] numArray = this.m_storage.ReadEmbeddedBlob(this.m_publicKey, this.m_cbPublicKey);
				assemblyName.HashAlgorithm = (AssemblyHashAlgorithm)this.m_hashAlgId;
				assemblyName.Name = this.m_szName.ToString();
				assemblyName.Version = this.m_metadata.Version;
				assemblyName.Flags = this.m_flags;
				if ((this.m_flags & AssemblyNameFlags.PublicKey) == AssemblyNameFlags.None)
				{
					assemblyName.SetPublicKeyToken(numArray);
				}
				else
				{
					assemblyName.SetPublicKey(numArray);
				}
				return assemblyName;
			}

			public void Dispose()
			{
				this.Dispose(true);
				GC.SuppressFinalize(this);
			}

			protected virtual void Dispose(bool disposing)
			{
				if (disposing)
				{
					this.m_metadata.szLocale.Dispose();
				}
			}

			protected abstract void Fetch();
		}

		private class AssemblyNameFromDefitionBuilder : AssemblyNameHelper.AssemblyNameBuilder
		{
			private Token assemblyToken;

			public AssemblyNameFromDefitionBuilder(Token assemblyToken, MetadataFile storage, IMetadataAssemblyImport assemblyImport) : base(storage, assemblyImport)
			{
				this.assemblyToken = assemblyToken;
			}

			protected override void Fetch()
			{
				this.m_assemblyImport.GetAssemblyProps(this.assemblyToken, out this.m_publicKey, out this.m_cbPublicKey, out this.m_hashAlgId, this.m_szName, this.m_chName, out this.m_chName, ref this.m_metadata, out this.m_flags);
			}
		}

		private class AssemblyNameFromRefBuilder : AssemblyNameHelper.AssemblyNameBuilder
		{
			private Token assemblyRefToken;

			public AssemblyNameFromRefBuilder(Token assemblyRefToken, MetadataFile storage, IMetadataAssemblyImport assemblyImport) : base(storage, assemblyImport)
			{
				if (assemblyRefToken.TokenType != System.Reflection.Adds.TokenType.AssemblyRef)
				{
					throw new ArgumentException(MetadataStringTable.AssemblyRefTokenExpected);
				}
				this.assemblyRefToken = assemblyRefToken;
			}

			protected override void Fetch()
			{
				UnusedIntPtr unusedIntPtr;
				uint num;
				this.m_assemblyImport.GetAssemblyRefProps(this.assemblyRefToken, out this.m_publicKey, out this.m_cbPublicKey, this.m_szName, this.m_chName, out this.m_chName, ref this.m_metadata, out unusedIntPtr, out num, out this.m_flags);
			}
		}
	}
}