// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.IFileDialogEvents
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Expression.Utility
{
  [Guid("973510DB-7D7F-452B-8975-74A85828D354")]
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [ComImport]
  internal interface IFileDialogEvents
  {
    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Microsoft.Expression.Utility.Interop.NativeMethods.HRESULT OnFileOk([MarshalAs(UnmanagedType.Interface), In] IFileDialog pfd);

    [MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    Microsoft.Expression.Utility.Interop.NativeMethods.HRESULT OnFolderChanging([MarshalAs(UnmanagedType.Interface), In] IFileDialog pfd, [MarshalAs(UnmanagedType.Interface), In] IShellItem psiFolder);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnFolderChange([MarshalAs(UnmanagedType.Interface), In] IFileDialog pfd);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnSelectionChange([MarshalAs(UnmanagedType.Interface), In] IFileDialog pfd);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnShareViolation([MarshalAs(UnmanagedType.Interface), In] IFileDialog pfd, [MarshalAs(UnmanagedType.Interface), In] IShellItem psi, out Microsoft.Expression.Utility.Interop.NativeMethods.FDE_SHAREVIOLATION_RESPONSE pResponse);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnTypeChange([MarshalAs(UnmanagedType.Interface), In] IFileDialog pfd);

    [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
    void OnOverwrite([MarshalAs(UnmanagedType.Interface), In] IFileDialog pfd, [MarshalAs(UnmanagedType.Interface), In] IShellItem psi, out Microsoft.Expression.Utility.Interop.NativeMethods.FDE_OVERWRITE_RESPONSE pResponse);
  }
}
