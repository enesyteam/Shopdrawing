using System;
using System.Reflection.Adds;
using System.Runtime.InteropServices;

namespace Microsoft.MetadataReader
{
	[Guid("D8F579AB-402D-4b8e-82D9-5D63B1065C68")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IMetadataTables
	{
		void GetBlob_();

		void GetBlobHeapSize(out uint countBytesBlobs);

		void GetCodedTokenInfo_();

		void GetColumn_();

		void GetColumnInfo_();

		void GetGuid_();

		void GetGuidHeapSize(out uint countBytesGuids);

		void GetNextBlob_();

		void GetNextGuid_();

		void GetNextString_();

		void GetNextUserString_();

		void GetNumTables(out uint countTables);

		void GetRow_();

		void GetString_();

		void GetStringHeapSize(out uint countBytesStrings);

		void GetTableIndex(uint token, out uint tableIndex);

		void GetTableInfo(MetadataTable tableIndex, out int countByteRows, out int countRows, out int countColumns, out int columnPrimaryKey, out UnusedIntPtr name);

		void GetUserString_();

		void GetUserStringHeapSize(out uint countByteBlobs);
	}
}