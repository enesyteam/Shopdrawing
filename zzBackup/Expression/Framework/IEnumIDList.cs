// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.IEnumIDList
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Expression.Framework
{
  [Guid("000214F2-0000-0000-C000-000000000046")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  internal interface IEnumIDList
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    NativeMethods.HRESULT Next([In] uint celt, out IntPtr rgelt, out uint pceltFetched);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    NativeMethods.HRESULT Skip([In] uint celt);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    NativeMethods.HRESULT Reset();

    [MethodImpl(MethodImplOptions.PreserveSig)]
    NativeMethods.HRESULT Clone([MarshalAs(UnmanagedType.Interface)] out IEnumIDList ppenum);
  }
}
