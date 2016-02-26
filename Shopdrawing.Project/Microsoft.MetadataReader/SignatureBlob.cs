using System;
using System.Reflection.Adds;

namespace Microsoft.MetadataReader
{
	internal class SignatureBlob
	{
		private readonly byte[] m_signature;

		private SignatureBlob(byte[] data)
		{
			this.m_signature = data;
		}

		public byte[] GetSignatureAsByteArray()
		{
			return this.m_signature;
		}

		public static SignatureBlob ReadSignature(MetadataFile storage, EmbeddedBlobPointer pointer, int countBytes)
		{
			return new SignatureBlob(storage.ReadEmbeddedBlob(pointer, countBytes));
		}
	}
}