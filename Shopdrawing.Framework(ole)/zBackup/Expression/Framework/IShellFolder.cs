// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.IShellFolder
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.Expression.Framework
{
  [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  [Guid("000214E6-0000-0000-C000-000000000046")]
  [ComConversionLoss]
  [ComImport]
  internal interface IShellFolder
  {
    [MethodImpl(MethodImplOptions.PreserveSig)]
    NativeMethods.HRESULT ParseDisplayName([In] IntPtr hwnd, [MarshalAs(UnmanagedType.Interface), In] IBindCtx pbc, [MarshalAs(UnmanagedType.LPWStr), In] string pszDisplayName, [In, Out] ref uint pchEaten, [Out] IntPtr ppidl, [In, Out] ref uint pdwAttributes);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    NativeMethods.HRESULT EnumObjects([In] IntPtr hwnd, [In] NativeMethods.SHCONTF grfFlags, [MarshalAs(UnmanagedType.Interface)] out IEnumIDList ppenumIDList);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    NativeMethods.HRESULT BindToObject([In] IntPtr pidl, [MarshalAs(UnmanagedType.Interface), In] IBindCtx pbc, [In] ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out IShellFolder ppv);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    NativeMethods.HRESULT BindToStorage([In] IntPtr pidl, [MarshalAs(UnmanagedType.Interface), In] IBindCtx pbc, [In] ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out IShellFolder ppv);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    NativeMethods.HRESULT CompareIDs([In] IntPtr lParam, [In] IntPtr pidl1, [In] IntPtr pidl2);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    NativeMethods.HRESULT CreateViewObject([In] IntPtr hwndOwner, [In] ref Guid riid, out IntPtr ppv);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    NativeMethods.HRESULT GetAttributesOf([In] uint cidl, [In] IntPtr apidl, [In, Out] ref uint rgfInOut);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    NativeMethods.HRESULT GetUIObjectOf([In] IntPtr hwndOwner, [In] uint cidl, [In] IntPtr apidl, [In] ref Guid riid, [In, Out] ref uint rgfReserved, out IntPtr ppv);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    NativeMethods.HRESULT GetDisplayNameOf([In] IntPtr pidl, [In] NativeMethods.SHGDNF uFlags, out IntPtr pName);

    [MethodImpl(MethodImplOptions.PreserveSig)]
    NativeMethods.HRESULT SetNameOf([In] IntPtr hwnd, [In] IntPtr pidl, [MarshalAs(UnmanagedType.LPWStr), In] string pszName, [In] NativeMethods.SHGDNF uFlags, out IntPtr ppidlOut);
  }
}
