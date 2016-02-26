// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.IEnumIDList
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Expression.Utility
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("000214F2-0000-0000-C000-000000000046")]
  [ComImport]
  internal interface IEnumIDList
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    Microsoft.Expression.Utility.Interop.NativeMethods.HRESULT Next([In] uint celt, out IntPtr rgelt, out uint pceltFetched);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    Microsoft.Expression.Utility.Interop.NativeMethods.HRESULT Skip([In] uint celt);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    Microsoft.Expression.Utility.Interop.NativeMethods.HRESULT Reset();

    [MethodImpl(MethodImplOptions.PreserveSig)]
    Microsoft.Expression.Utility.Interop.NativeMethods.HRESULT Clone([MarshalAs(UnmanagedType.Interface)] out IEnumIDList ppenum);
  }
}
