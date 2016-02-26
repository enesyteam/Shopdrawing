// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Utility.IShellFolder
// Assembly: Microsoft.Expression.Utility, Version=5.0.30709.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: B77F0E80-A3D7-4861-BF76-6A6A586443F3
// Assembly location: C:\Users\M4600\Documents\Project\4.5\Microsoft.Expression.ProjectReferences\Microsoft.Expression.Utility.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.Expression.Utility
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("000214E6-0000-0000-C000-000000000046")]
  [ComConversionLoss]
  [ComImport]
  internal interface IShellFolder
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    Microsoft.Expression.Utility.Interop.NativeMethods.HRESULT ParseDisplayName([In] IntPtr hwnd, [MarshalAs(UnmanagedType.Interface), In] IBindCtx pbc, [MarshalAs(UnmanagedType.LPWStr), In] string pszDisplayName, [In, Out] ref uint pchEaten, [Out] IntPtr ppidl, [In, Out] ref uint pdwAttributes);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    Microsoft.Expression.Utility.Interop.NativeMethods.HRESULT EnumObjects([In] IntPtr hwnd, [In] Microsoft.Expression.Utility.Interop.NativeMethods.SHCONTF grfFlags, [MarshalAs(UnmanagedType.Interface)] out IEnumIDList ppenumIDList);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    Microsoft.Expression.Utility.Interop.NativeMethods.HRESULT BindToObject([In] IntPtr pidl, [MarshalAs(UnmanagedType.Interface), In] IBindCtx pbc, [In] ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out IShellFolder ppv);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    Microsoft.Expression.Utility.Interop.NativeMethods.HRESULT BindToStorage([In] IntPtr pidl, [MarshalAs(UnmanagedType.Interface), In] IBindCtx pbc, [In] ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out IShellFolder ppv);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    Microsoft.Expression.Utility.Interop.NativeMethods.HRESULT CompareIDs([In] IntPtr lParam, [In] IntPtr pidl1, [In] IntPtr pidl2);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    Microsoft.Expression.Utility.Interop.NativeMethods.HRESULT CreateViewObject([In] IntPtr hwndOwner, [In] ref Guid riid, out IntPtr ppv);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    Microsoft.Expression.Utility.Interop.NativeMethods.HRESULT GetAttributesOf([In] uint cidl, [In] IntPtr apidl, [In, Out] ref uint rgfInOut);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    Microsoft.Expression.Utility.Interop.NativeMethods.HRESULT GetUIObjectOf([In] IntPtr hwndOwner, [In] uint cidl, [In] IntPtr apidl, [In] ref Guid riid, [In, Out] ref uint rgfReserved, out IntPtr ppv);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    Microsoft.Expression.Utility.Interop.NativeMethods.HRESULT GetDisplayNameOf([In] IntPtr pidl, [In] Microsoft.Expression.Utility.Interop.NativeMethods.SHGDNF uFlags, out IntPtr pName);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    Microsoft.Expression.Utility.Interop.NativeMethods.HRESULT SetNameOf([In] IntPtr hwnd, [In] IntPtr pidl, [MarshalAs(UnmanagedType.LPWStr), In] string pszName, [In] Microsoft.Expression.Utility.Interop.NativeMethods.SHGDNF uFlags, out IntPtr ppidlOut);
  }
}
