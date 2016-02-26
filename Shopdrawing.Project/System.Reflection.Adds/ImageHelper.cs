using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace System.Reflection.Adds
{
	internal class ImageHelper
	{
		private readonly IntPtr m_baseAddress;

		private readonly long m_lengthBytes;

		private readonly uint m_idx;

		private readonly uint m_idxSectionStart;

		private readonly uint m_numSections;

		private readonly uint m_clrHeaderRva;

		public System.Reflection.Adds.ImageType ImageType
		{
			get;
			private set;
		}

		public ImageHelper(IntPtr baseAddress, long lengthBytes)
		{
			this.m_baseAddress = baseAddress;
			this.m_lengthBytes = lengthBytes;
			ImageHelper.IMAGE_DOS_HEADER mAGEDOSHEADER = this.MarshalAt<ImageHelper.IMAGE_DOS_HEADER>(0);
			if (!mAGEDOSHEADER.IsValid)
			{
				throw new ArgumentException(MetadataStringTable.InvalidFileFormat);
			}
			this.m_idx = mAGEDOSHEADER.e_lfanew;
			ImageHelper.IMAGE_NT_HEADERS_HELPER mAGENTHEADERSHELPER = this.MarshalAt<ImageHelper.IMAGE_NT_HEADERS_HELPER>(this.m_idx);
			if (!mAGENTHEADERSHELPER.IsValid)
			{
				throw new ArgumentException(MetadataStringTable.InvalidFileFormat);
			}
			if (mAGENTHEADERSHELPER.Magic == 267)
			{
				this.ImageType = System.Reflection.Adds.ImageType.Pe32bit;
				ImageHelper.IMAGE_NT_HEADERS_32 mAGENTHEADERS32 = this.MarshalAt<ImageHelper.IMAGE_NT_HEADERS_32>(this.m_idx);
				this.m_idxSectionStart = this.m_idx + Marshal.SizeOf(typeof(ImageHelper.IMAGE_NT_HEADERS_32));
				this.m_numSections = (uint)mAGENTHEADERS32.FileHeader.NumberOfSections;
				this.m_clrHeaderRva = mAGENTHEADERS32.OptionalHeader.ClrHeaderTable.VirtualAddress;
				return;
			}
			if (mAGENTHEADERSHELPER.Magic != 523)
			{
				throw new ArgumentException(MetadataStringTable.UnsupportedImageType);
			}
			this.ImageType = System.Reflection.Adds.ImageType.Pe64bit;
			ImageHelper.IMAGE_NT_HEADERS_64 mAGENTHEADERS64 = this.MarshalAt<ImageHelper.IMAGE_NT_HEADERS_64>(this.m_idx);
			this.m_idxSectionStart = this.m_idx + Marshal.SizeOf(typeof(ImageHelper.IMAGE_NT_HEADERS_64));
			this.m_numSections = (uint)mAGENTHEADERS64.FileHeader.NumberOfSections;
			this.m_clrHeaderRva = mAGENTHEADERS64.OptionalHeader.ClrHeaderTable.VirtualAddress;
		}

		internal ImageHelper.IMAGE_COR20_HEADER GetCor20Header()
		{
			IntPtr intPtr = this.ResolveRva((long)this.m_clrHeaderRva);
			return (ImageHelper.IMAGE_COR20_HEADER)Marshal.PtrToStructure(intPtr, typeof(ImageHelper.IMAGE_COR20_HEADER));
		}

		public Token GetEntryPointToken()
		{
			ImageHelper.IMAGE_COR20_HEADER cor20Header = this.GetCor20Header();
			if ((int)(cor20Header.Flags & ImageHelper.CorHdrNumericDefines.COMIMAGE_FLAGS_NATIVE_ENTRYPOINT) != 0)
			{
				return Token.Nil;
			}
			return new Token(cor20Header.EntryPoint);
		}

		public IntPtr GetResourcesSectionStart()
		{
			return this.ResolveRva((long)this.GetCor20Header().Resources.VirtualAddress);
		}

		internal T MarshalAt<T>(uint offset)
		{
			long num = this.m_baseAddress.ToInt64();
			long mLengthBytes = num + this.m_lengthBytes;
			int num1 = Marshal.SizeOf(typeof(T));
			if ((ulong)offset + (long)num1 > mLengthBytes)
			{
				throw new InvalidOperationException(MetadataStringTable.CorruptImage);
			}
			IntPtr intPtr = new IntPtr((long)(num + (ulong)offset));
			return (T)Marshal.PtrToStructure(intPtr, typeof(T));
		}

		public IntPtr ResolveRva(long rva)
		{
			uint mIdxSectionStart = this.m_idxSectionStart;
			for (int i = 0; (long)i < (ulong)this.m_numSections; i++)
			{
				ImageHelper.IMAGE_SECTION_HEADER mAGESECTIONHEADER = this.MarshalAt<ImageHelper.IMAGE_SECTION_HEADER>(mIdxSectionStart);
				if (rva >= (ulong)mAGESECTIONHEADER.VirtualAddress && rva < (ulong)(mAGESECTIONHEADER.VirtualAddress + mAGESECTIONHEADER.SizeOfRawData))
				{
					IntPtr mBaseAddress = this.m_baseAddress;
					long num = (long)(mBaseAddress.ToInt64() + rva - (ulong)mAGESECTIONHEADER.VirtualAddress + (ulong)mAGESECTIONHEADER.PointerToRawData);
					return new IntPtr(num);
				}
				mIdxSectionStart = mIdxSectionStart + Marshal.SizeOf(typeof(ImageHelper.IMAGE_SECTION_HEADER));
			}
			return IntPtr.Zero;
		}

		internal enum CorHdrNumericDefines : uint
		{
			COMIMAGE_FLAGS_ILONLY = 1,
			COMIMAGE_FLAGS_32BITREQUIRED = 2,
			COMIMAGE_FLAGS_IL_LIBRARY = 4,
			COMIMAGE_FLAGS_STRONGNAMESIGNED = 8,
			COMIMAGE_FLAGS_NATIVE_ENTRYPOINT = 16,
			COMIMAGE_FLAGS_TRACKDEBUGDATA = 65536,
			COMIMAGE_FLAGS_ISIBCOPTIMIZED = 131072
		}

		internal class IMAGE_COR20_HEADER
		{
			public uint cb;

			public ushort MajorRuntimeVersion;

			public ushort MinorRuntimeVersion;

			public ImageHelper.IMAGE_DATA_DIRECTORY MetaData;

			public ImageHelper.CorHdrNumericDefines Flags;

			public uint EntryPoint;

			public ImageHelper.IMAGE_DATA_DIRECTORY Resources;

			public ImageHelper.IMAGE_DATA_DIRECTORY StrongNameSignature;

			public ImageHelper.IMAGE_DATA_DIRECTORY CodeManagerTable;

			public ImageHelper.IMAGE_DATA_DIRECTORY VTableFixups;

			public ImageHelper.IMAGE_DATA_DIRECTORY ExportAddressTableJumps;

			public ImageHelper.IMAGE_DATA_DIRECTORY ManagedNativeHeader;

			public IMAGE_COR20_HEADER()
			{
			}
		}

		internal class IMAGE_DATA_DIRECTORY
		{
			public uint VirtualAddress;

			public uint Size;

			public IMAGE_DATA_DIRECTORY()
			{
			}
		}

		[StructLayout(LayoutKind.Explicit)]
		public class IMAGE_DOS_HEADER
		{
			[FieldOffset(0)]
			public short e_magic;

			[FieldOffset(60)]
			public uint e_lfanew;

			public bool IsValid
			{
				get
				{
					return this.e_magic == 23117;
				}
			}

			public IMAGE_DOS_HEADER()
			{
			}
		}

		internal class IMAGE_FILE_HEADER
		{
			public short Machine;

			public short NumberOfSections;

			public uint TimeDateStamp;

			public uint PointerToSymbolTable;

			public uint NumberOfSymbols;

			public short SizeOfOptionalHeader;

			public short Characteristics;

			public IMAGE_FILE_HEADER()
			{
			}
		}

		internal class IMAGE_NT_HEADERS_32
		{
			public uint Signature;

			public ImageHelper.IMAGE_FILE_HEADER FileHeader;

			public ImageHelper.IMAGE_OPTIONAL_HEADER_32 OptionalHeader;

			public IMAGE_NT_HEADERS_32()
			{
			}
		}

		internal class IMAGE_NT_HEADERS_64
		{
			public uint Signature;

			public ImageHelper.IMAGE_FILE_HEADER FileHeader;

			public ImageHelper.IMAGE_OPTIONAL_HEADER_64 OptionalHeader;

			public IMAGE_NT_HEADERS_64()
			{
			}
		}

		internal class IMAGE_NT_HEADERS_HELPER
		{
			public uint Signature;

			public ImageHelper.IMAGE_FILE_HEADER FileHeader;

			public ushort Magic;

			public bool IsValid
			{
				get
				{
					return this.Signature == 17744;
				}
			}

			public IMAGE_NT_HEADERS_HELPER()
			{
			}
		}

		internal class IMAGE_OPTIONAL_HEADER_32
		{
			public ushort Magic;

			public byte MajorLinkerVersion;

			public byte MinorLinkerVersion;

			public uint SizeOfCode;

			public uint SizeOfInitializedData;

			public uint SizeOfUninitializedData;

			public uint AddressOfEntryPoint;

			public uint BaseOfCode;

			public uint BaseOfData;

			public uint ImageBase;

			public uint SectionAlignment;

			public uint FileAlignment;

			public ushort MajorOperatingSystemVersion;

			public ushort MinorOperatingSystemVersion;

			public ushort MajorImageVersion;

			public ushort MinorImageVersion;

			public ushort MajorSubsystemVersion;

			public ushort MinorSubsystemVersion;

			public uint Win32VersionValue;

			public uint SizeOfImage;

			public uint SizeOfHeaders;

			public uint CheckSum;

			public ushort Subsystem;

			public ushort DllCharacteristics;

			public uint SizeOfStackReserve;

			public uint SizeOfStackCommit;

			public uint SizeOfHeapReserve;

			public uint SizeOfHeapCommit;

			public uint LoaderFlags;

			public uint NumberOfRvaAndSizes;

			public ImageHelper.IMAGE_DATA_DIRECTORY ExportTable;

			public ImageHelper.IMAGE_DATA_DIRECTORY ImportTable;

			public ImageHelper.IMAGE_DATA_DIRECTORY ResourceTable;

			public ImageHelper.IMAGE_DATA_DIRECTORY ExceptionTable;

			public ImageHelper.IMAGE_DATA_DIRECTORY CertificateTable;

			public ImageHelper.IMAGE_DATA_DIRECTORY BaseRelocationTable;

			public ImageHelper.IMAGE_DATA_DIRECTORY DebugData;

			public ImageHelper.IMAGE_DATA_DIRECTORY ArchitectureData;

			public ImageHelper.IMAGE_DATA_DIRECTORY GlobalPointer;

			public ImageHelper.IMAGE_DATA_DIRECTORY TlsTable;

			public ImageHelper.IMAGE_DATA_DIRECTORY LoadConfigTable;

			public ImageHelper.IMAGE_DATA_DIRECTORY BoundImportTable;

			public ImageHelper.IMAGE_DATA_DIRECTORY ImportAddressTable;

			public ImageHelper.IMAGE_DATA_DIRECTORY DelayImportTable;

			public ImageHelper.IMAGE_DATA_DIRECTORY ClrHeaderTable;

			public ImageHelper.IMAGE_DATA_DIRECTORY Reserved;

			public IMAGE_OPTIONAL_HEADER_32()
			{
			}
		}

		internal class IMAGE_OPTIONAL_HEADER_64
		{
			public ushort Magic;

			public byte MajorLinkerVersion;

			public byte MinorLinkerVersion;

			public uint SizeOfCode;

			public uint SizeOfInitializedData;

			public uint SizeOfUninitializedData;

			public uint AddressOfEntryPoint;

			public uint BaseOfCode;

			public ulong ImageBase;

			public uint SectionAlignment;

			public uint FileAlignment;

			public ushort MajorOperatingSystemVersion;

			public ushort MinorOperatingSystemVersion;

			public ushort MajorImageVersion;

			public ushort MinorImageVersion;

			public ushort MajorSubsystemVersion;

			public ushort MinorSubsystemVersion;

			public uint Win32VersionValue;

			public uint SizeOfImage;

			public uint SizeOfHeaders;

			public uint CheckSum;

			public ushort Subsystem;

			public ushort DllCharacteristics;

			public ulong SizeOfStackReserve;

			public ulong SizeOfStackCommit;

			public ulong SizeOfHeapReserve;

			public ulong SizeOfHeapCommit;

			public uint LoaderFlags;

			public uint NumberOfRvaAndSizes;

			public ImageHelper.IMAGE_DATA_DIRECTORY ExportTable;

			public ImageHelper.IMAGE_DATA_DIRECTORY ImportTable;

			public ImageHelper.IMAGE_DATA_DIRECTORY ResourceTable;

			public ImageHelper.IMAGE_DATA_DIRECTORY ExceptionTable;

			public ImageHelper.IMAGE_DATA_DIRECTORY CertificateTable;

			public ImageHelper.IMAGE_DATA_DIRECTORY BaseRelocationTable;

			public ImageHelper.IMAGE_DATA_DIRECTORY DebugData;

			public ImageHelper.IMAGE_DATA_DIRECTORY ArchitectureData;

			public ImageHelper.IMAGE_DATA_DIRECTORY GlobalPointer;

			public ImageHelper.IMAGE_DATA_DIRECTORY TlsTable;

			public ImageHelper.IMAGE_DATA_DIRECTORY LoadConfigTable;

			public ImageHelper.IMAGE_DATA_DIRECTORY BoundImportTable;

			public ImageHelper.IMAGE_DATA_DIRECTORY ImportAddressTable;

			public ImageHelper.IMAGE_DATA_DIRECTORY DelayImportTable;

			public ImageHelper.IMAGE_DATA_DIRECTORY ClrHeaderTable;

			public ImageHelper.IMAGE_DATA_DIRECTORY Reserved;

			public IMAGE_OPTIONAL_HEADER_64()
			{
			}
		}

		private class IMAGE_SECTION_HEADER
		{
			public string name;

			public uint union;

			public uint VirtualAddress;

			public uint SizeOfRawData;

			public uint PointerToRawData;

			public uint PointerToRelocations;

			public uint PointerToLinenumbers;

			public ushort NumberOfRelocations;

			public ushort NumberOfLinenumbers;

			public uint Characteristics;

			public IMAGE_SECTION_HEADER()
			{
			}
		}
	}
}