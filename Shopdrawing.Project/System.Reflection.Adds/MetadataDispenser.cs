using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.Reflection.Adds
{
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode=true)]
	internal class MetadataDispenser
	{
		public MetadataDispenser()
		{
		}

		private static MetadataDispenser.IMetaDataDispenserEx GetDispenserShim()
		{
			return (MetadataDispenser.IMetaDataDispenserEx)RuntimeEnvironment.GetRuntimeInterfaceAsObject(typeof(MetadataDispenser.CorMetaDataDispenserExClass).GUID, typeof(MetadataDispenser.IMetaDataDispenserEx).GUID);
		}

		public MetadataFile OpenFile(string fileName)
		{
			MetadataFile metadataFile;
			MetadataDispenser.IMetaDataDispenserEx dispenserShim = MetadataDispenser.GetDispenserShim();
			Guid gUID = typeof(MetadataDispenser.IMetadataImportDummy).GUID;
			IntPtr zero = IntPtr.Zero;
			try
			{
				int num = dispenserShim.OpenScope(fileName, MetadataDispenser.CorOpenFlags.ReadOnly, ref gUID, out zero);
				Marshal.ThrowExceptionForHR(num);
				GC.KeepAlive(dispenserShim);
				Marshal.FinalReleaseComObject(dispenserShim);
				dispenserShim = null;
				metadataFile = new MetadataFile(zero);
			}
			finally
			{
				if (zero != IntPtr.Zero)
				{
					Marshal.Release(zero);
				}
			}
			return metadataFile;
		}

		public MetadataFile OpenFileAsFileMapping(string fileName)
		{
			MetadataFile metadataFileAndRvaResolver;
			FileMapping fileMapping = new FileMapping(fileName);
			MetadataDispenser.IMetaDataDispenserEx dispenserShim = MetadataDispenser.GetDispenserShim();
			Guid gUID = typeof(MetadataDispenser.IMetadataImportDummy).GUID;
			IntPtr zero = IntPtr.Zero;
			try
			{
				IntPtr baseAddress = fileMapping.BaseAddress;
				uint length = (uint)fileMapping.Length;
				int num = dispenserShim.OpenScopeOnMemory(baseAddress, length, MetadataDispenser.CorOpenFlags.ReadOnly, ref gUID, out zero);
				Marshal.ThrowExceptionForHR(num);
				GC.KeepAlive(dispenserShim);
				Marshal.FinalReleaseComObject(dispenserShim);
				dispenserShim = null;
				metadataFileAndRvaResolver = new MetadataFileAndRvaResolver(zero, fileMapping);
			}
			finally
			{
				if (zero != IntPtr.Zero)
				{
					Marshal.Release(zero);
				}
			}
			return metadataFileAndRvaResolver;
		}

		public MetadataFile OpenFromByteArray(byte[] data)
		{
			MetadataFile metadataFileOnByteArray;
			data = (byte[])data.Clone();
			MetadataDispenser.IMetaDataDispenserEx dispenserShim = MetadataDispenser.GetDispenserShim();
			Guid gUID = typeof(MetadataDispenser.IMetadataImportDummy).GUID;
			IntPtr zero = IntPtr.Zero;
			GCHandle gCHandle = new GCHandle();
			try
			{
				gCHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
				IntPtr intPtr = Marshal.UnsafeAddrOfPinnedArrayElement(data, 0);
				uint length = (uint)data.Length;
				int num = dispenserShim.OpenScopeOnMemory(intPtr, length, MetadataDispenser.CorOpenFlags.ReadOnly, ref gUID, out zero);
				Marshal.ThrowExceptionForHR(num);
				GC.KeepAlive(dispenserShim);
				Marshal.FinalReleaseComObject(dispenserShim);
				dispenserShim = null;
				metadataFileOnByteArray = new MetadataDispenser.MetadataFileOnByteArray(ref gCHandle, zero);
			}
			finally
			{
				if (gCHandle.IsAllocated)
				{
					gCHandle.Free();
				}
				if (zero != IntPtr.Zero)
				{
					Marshal.Release(zero);
				}
			}
			return metadataFileOnByteArray;
		}

		[Guid("E5CB7A31-7512-11D2-89CE-0080C792E5D8")]
		private class CorMetaDataDispenserExClass
		{
			public extern CorMetaDataDispenserExClass();
		}

		private enum CorOpenFlags : uint
		{
			Read = 0,
			ReadWriteMask = 1,
			Write = 1,
			CopyMemory = 2,
			CacheImage = 4,
			ManifestMetadata = 8,
			ReadOnly = 16,
			TakeOwnership = 32,
			NoTypeLib = 128
		}

		[Guid("31BCFCE2-DAFB-11D2-9F81-00C04F79A0A3")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		private interface IMetaDataDispenserEx
		{
			int _OpenScopeOnITypeInfo();

			int DefineScope(ref Guid rclsid, uint dwCreateFlags, ref Guid riid, out object ppIUnk);

			int FindAssembly(string szAppBase, string szPrivateBin, string szGlobalBin, string szAssemblyName, char[] szName, uint cchName, out uint pcName);

			int FindAssemblyModule(string szAppBase, string szPrivateBin, string szGlobalBin, string szAssemblyName, string szModuleName, char[] szName, uint cchName, out uint pcName);

			int GetCORSystemDirectory(char[] szBuffer, uint cchBuffer, out uint pchBuffer);

			int GetOption(ref Guid optionid, out object pvalue);

			int OpenScope(string szScope, MetadataDispenser.CorOpenFlags dwOpenFlags, ref Guid riid, out IntPtr ppIUnk);

			int OpenScopeOnMemory(IntPtr pData, uint cbData, MetadataDispenser.CorOpenFlags dwOpenFlags, ref Guid riid, out IntPtr ppIUnk);

			int SetOption(ref Guid optionid, object value);
		}

		[Guid("7DAC8207-D3AE-4c75-9B67-92801A497D44")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		private interface IMetadataImportDummy
		{

		}

		[CoClass(typeof(MetadataDispenser.CorMetaDataDispenserExClass))]
		[Guid("31BCFCE2-DAFB-11D2-9F81-00C04F79A0A3")]
		private interface MetaDataDispenserEx : MetadataDispenser.IMetaDataDispenserEx
		{

		}

		private class MetadataFileOnByteArray : MetadataFile
		{
			private GCHandle m_handle;

			public MetadataFileOnByteArray(ref GCHandle h, IntPtr pUnk) : base(pUnk)
			{
				this.m_handle = h;
				h = new GCHandle();
			}

			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
				this.m_handle.Free();
			}
		}
	}
}