// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.IFileDialogEvents
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Expression.Framework
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("973510DB-7D7F-452B-8975-74A85828D354")]
  [ComImport]
  internal interface IFileDialogEvents
  {
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    NativeMethods.HRESULT OnFileOk([MarshalAs(UnmanagedType.Interface), In] IFileDialog pfd);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    NativeMethods.HRESULT OnFolderChanging([MarshalAs(UnmanagedType.Interface), In] IFileDialog pfd, [MarshalAs(UnmanagedType.Interface), In] IShellItem psiFolder);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnFolderChange([MarshalAs(UnmanagedType.Interface), In] IFileDialog pfd);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnSelectionChange([MarshalAs(UnmanagedType.Interface), In] IFileDialog pfd);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnShareViolation([MarshalAs(UnmanagedType.Interface), In] IFileDialog pfd, [MarshalAs(UnmanagedType.Interface), In] IShellItem psi, out NativeMethods.FDE_SHAREVIOLATION_RESPONSE pResponse);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnTypeChange([MarshalAs(UnmanagedType.Interface), In] IFileDialog pfd);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnOverwrite([MarshalAs(UnmanagedType.Interface), In] IFileDialog pfd, [MarshalAs(UnmanagedType.Interface), In] IShellItem psi, out NativeMethods.FDE_OVERWRITE_RESPONSE pResponse);
  }
}
