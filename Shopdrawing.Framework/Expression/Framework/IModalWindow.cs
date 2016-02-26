// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.IModalWindow
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Expression.Framework
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("b4db1657-70d7-485e-8e3e-6fcb5a5c1802")]
  [ComImport]
  internal interface IModalWindow
  {
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    int Show([In] IntPtr parent);
  }
}
