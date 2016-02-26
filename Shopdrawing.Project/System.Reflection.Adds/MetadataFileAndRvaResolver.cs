// Decompiled with JetBrains decompiler
// Type: System.Reflection.Adds.MetadataFileAndRvaResolver
// Assembly: Microsoft.Expression.Project, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 80357D9B-A7D7-4011-8FBC-3E1052652ADC
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Project.dll

using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Adds
{
    internal class MetadataFileAndRvaResolver : MetadataFile
    {
        private FileMapping m_file;

        public override string FilePath
        {
            get
            {
                return this.m_file.Path;
            }
        }

        public MetadataFileAndRvaResolver(IntPtr importer, FileMapping file)
            : base(importer)
        {
            this.m_file = file;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.m_file.Dispose();
        }

        private IntPtr ResolveRva(long rva)
        {
            IntPtr num = new ImageHelper(this.m_file.BaseAddress, this.m_file.Length).ResolveRva(rva);
            if (num == IntPtr.Zero)
                throw new InvalidOperationException(MetadataStringTable.CannotResolveRVA);
            return num;
        }

        public override byte[] ReadRva(long rva, int countBytes)
        {
            IntPtr source = this.ResolveRva(rva);
            byte[] destination = new byte[countBytes];
            Marshal.Copy(source, destination, 0, destination.Length);
            return destination;
        }

        protected override void ValidateRange(IntPtr ptr, int countBytes)
        {
            long num1 = this.m_file.BaseAddress.ToInt64();
            long num2 = num1 + this.m_file.Length;
            long num3 = ptr.ToInt64();
            if (num3 < num1 || num3 + (long)countBytes >= num2)
                throw new InvalidOperationException();
        }

        public override byte[] ReadResource(long offset)
        {
            unsafe
            {
                ImageHelper imageHelper = new ImageHelper(this.m_file.BaseAddress, this.m_file.Length);
                IntPtr resourcesSectionStart = imageHelper.GetResourcesSectionStart();
                IntPtr intPtr = new IntPtr(resourcesSectionStart.ToInt64() + offset);
                uint num = (uint)Marshal.ReadInt32(intPtr);
                intPtr = new IntPtr(intPtr.ToInt64() + (long)Marshal.SizeOf(num));
                byte[] numArray = new byte[num];
                Marshal.Copy(intPtr, numArray, 0, (int)numArray.Length);
                return numArray;
            }
        }

        public override Token ReadEntryPointToken()
        {
            return new ImageHelper(this.m_file.BaseAddress, this.m_file.Length).GetEntryPointToken();
        }

        public override T ReadRvaStruct<T>(long rva)
        {
            return (T)Marshal.PtrToStructure(this.ResolveRva(rva), typeof(T));
        }
    }
}
