// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.ExpressionFileDialog
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Windows;
using System.Windows.Interop;

namespace Microsoft.Expression.Framework.Controls
{
  public abstract class ExpressionFileDialog
  {
    private static readonly int MaxFileTypeDescriptionLength = 220;
    private static readonly Version VistaOSVersion = new Version(6, 0);
    private bool trimFileTypeDescriptions = true;
    private Window parentWindow;
    private bool preferVistaDialog;
    private bool restoreDirectory;
    private bool pickFolders;
    private bool webFolders;
    private string initialDirectory;
    private string title;
    private string filter;

    public bool PreferVistaDialog
    {
      get
      {
        return this.preferVistaDialog;
      }
    }

    public bool PickFolders
    {
      get
      {
        return this.pickFolders;
      }
      set
      {
        this.pickFolders = value;
      }
    }

    public bool WebFolders
    {
      get
      {
        return this.webFolders;
      }
      set
      {
        this.webFolders = value;
      }
    }

    public bool RestoreDirectory
    {
      get
      {
        return this.restoreDirectory;
      }
      set
      {
        this.restoreDirectory = value;
      }
    }

    public string InitialDirectory
    {
      get
      {
        return this.initialDirectory;
      }
      set
      {
        this.initialDirectory = value;
      }
    }

    public string Title
    {
      get
      {
        return this.title;
      }
      set
      {
        this.title = value;
      }
    }

    public string Filter
    {
      get
      {
        return this.filter;
      }
      set
      {
        this.filter = value;
      }
    }

    public bool TrimFileDescriptions
    {
      get
      {
        return this.trimFileTypeDescriptions;
      }
      set
      {
        this.trimFileTypeDescriptions = value;
      }
    }

    public static bool CanPickFolders
    {
      get
      {
        return ExpressionFileDialog.IsVista;
      }
    }

    protected static bool IsVista
    {
      get
      {
        OperatingSystem osVersion = Environment.OSVersion;
        if (osVersion != null && osVersion.Platform == PlatformID.Win32NT)
          return osVersion.Version.CompareTo(ExpressionFileDialog.VistaOSVersion) >= 0;
        return false;
      }
    }

    protected bool ShouldUseVistaDialog
    {
      get
      {
        if (ExpressionFileDialog.IsVista)
          return this.preferVistaDialog;
        return false;
      }
    }

    protected bool ShouldUseWebFolders
    {
      get
      {
        if (ExpressionFileDialog.IsVista)
          return this.webFolders;
        return false;
      }
    }

    internal abstract IFileDialog NativeFileDialog { get; }

    protected abstract FileDialog Win32FileDialog { get; }

    protected ExpressionFileDialog()
      : this(true)
    {
    }

    protected ExpressionFileDialog(bool preferVistaDialog)
      : this(preferVistaDialog, (Window) null)
    {
    }

    protected ExpressionFileDialog(bool preferVistaDialog, Window parentWindow)
    {
      this.preferVistaDialog = preferVistaDialog;
      if (parentWindow == null)
      {
        parentWindow = Application.Current.MainWindow;
        foreach (Window window in Application.Current.Windows)
        {
          if (window.IsActive)
          {
            parentWindow = window;
            break;
          }
        }
      }
      this.parentWindow = parentWindow;
    }

    public bool? ShowDialog()
    {
      using (new ModalDialogHelper())
      {
        if (this.ShouldUseVistaDialog)
        {
          IFileDialog nativeFileDialog = this.NativeFileDialog;
          nativeFileDialog.SetTitle(this.Title);
          this.SetFileTypes(nativeFileDialog);
          try
          {
            IShellItem psi = !this.ShouldUseWebFolders ? ExpressionFileDialog.GetShellItemForPath(this.initialDirectory) : ExpressionFileDialog.GetShellItemForWebPath(this.initialDirectory);
            nativeFileDialog.SetDefaultFolder(psi);
            nativeFileDialog.SetFolder(psi);
          }
          catch (FileNotFoundException ex)
          {
          }
          NativeMethods.FOS pfos;
          nativeFileDialog.GetOptions(out pfos);
          nativeFileDialog.SetOptions(this.GenerateNativeDialogOptions() | pfos);
          return new bool?(nativeFileDialog.Show(ExpressionFileDialog.GetPointerFromWindow(this.parentWindow)) == 0);
        }
        FileDialog win32FileDialog = this.Win32FileDialog;
        this.InitializeWin32FileDialog(win32FileDialog);
        return win32FileDialog.ShowDialog();
      }
    }

    protected virtual void InitializeWin32FileDialog(FileDialog dialog)
    {
      dialog.Title = this.title;
      dialog.InitialDirectory = this.initialDirectory;
      dialog.RestoreDirectory = this.restoreDirectory;
      dialog.Filter = this.filter;
    }

    private static IntPtr GetPointerFromWindow(Window window)
    {
      if (window == null)
        return NativeMethods.NO_PARENT;
      return new WindowInteropHelper(window).Handle;
    }

    private List<NativeMethods.COMDLG_FILTERSPEC> ParseFilterString()
    {
      List<NativeMethods.COMDLG_FILTERSPEC> list = new List<NativeMethods.COMDLG_FILTERSPEC>();
      if (this.filter != null)
      {
        string[] strArray = this.filter.Split('|');
        int index = 0;
        while (index < strArray.Length - 1)
        {
          string str1 = strArray[index];
          string str2 = strArray[index + 1];
          string str3 = str1.Trim();
          string str4 = str2.Trim();
          if (this.TrimFileDescriptions)
          {
            int length = str3.IndexOf("(*.", StringComparison.OrdinalIgnoreCase);
            if (length >= 0)
              str3 = str3.Substring(0, length).Trim();
            string str5 = str4.Replace(";", ", ");
            str3 = str3 + " (" + str5 + ")";
            if (str3.Length > ExpressionFileDialog.MaxFileTypeDescriptionLength)
              str3 = str3.Substring(0, ExpressionFileDialog.MaxFileTypeDescriptionLength);
          }
          list.Add(new NativeMethods.COMDLG_FILTERSPEC()
          {
            pszName = str3,
            pszSpec = str4
          });
          index += 2;
        }
      }
      return list;
    }

    private void SetFileTypes(IFileDialog dialog)
    {
      List<NativeMethods.COMDLG_FILTERSPEC> list = this.ParseFilterString();
      NativeMethods.COMDLG_FILTERSPEC[] rgFilterSpec = list.ToArray();
      dialog.SetFileTypes((uint) list.Count, rgFilterSpec);
    }

    internal virtual NativeMethods.FOS GenerateNativeDialogOptions()
    {
      NativeMethods.FOS fos = NativeMethods.FOS.FOS_FORCEFILESYSTEM | NativeMethods.FOS.FOS_NOTESTFILECREATE;
      if (this.RestoreDirectory)
        fos |= NativeMethods.FOS.FOS_NOCHANGEDIR;
      if (this.PickFolders)
        fos |= NativeMethods.FOS.FOS_PICKFOLDERS;
      return fos;
    }

    private static IShellItem GetShellItemForPath(string path)
    {
      IShellItem ppsi = (IShellItem) null;
      IntPtr ppIdl = IntPtr.Zero;
      uint rgflnOut = 0U;
      if (0 > Microsoft.Expression.Framework.UnsafeNativeMethods.Shell32.SHILCreateFromPath(path, out ppIdl, ref rgflnOut) || 0 > Microsoft.Expression.Framework.UnsafeNativeMethods.Shell32.SHCreateShellItem(IntPtr.Zero, IntPtr.Zero, ppIdl, out ppsi))
        throw new FileNotFoundException();
      return ppsi;
    }

    private static IShellItem GetShellItemForWebPath(string path)
    {
      IShellItem ppsi = (IShellItem) null;
      IBindCtx ppbc;
      if (Microsoft.Expression.Framework.UnsafeNativeMethods.Ole32.CreateBindCtx(0U, out ppbc) >= 0 && ppbc != null)
      {
        IShellFolder ppshf = (IShellFolder) null;
        if (Microsoft.Expression.Framework.UnsafeNativeMethods.Shell32.SHGetDesktopFolder(out ppshf) < 0 || ppshf == null)
          throw new FileNotFoundException();
        ppbc.RegisterObjectParam(Microsoft.Expression.Framework.UnsafeNativeMethods.Ole32.BindContextStringKeys.STR_PARSE_PREFER_FOLDER_BROWSING, (object) ppshf);
        IntPtr pidl = IntPtr.Zero;
        uint psfgaoOut;
        if (Microsoft.Expression.Framework.UnsafeNativeMethods.Shell32.SHParseDisplayName(path, ppbc, out pidl, 0U, out psfgaoOut) < 0)
          throw new FileNotFoundException();
        if (Microsoft.Expression.Framework.UnsafeNativeMethods.Shell32.SHCreateShellItem(IntPtr.Zero, IntPtr.Zero, pidl, out ppsi) < 0 || ppsi == null)
          throw new FileNotFoundException();
      }
      return ppsi;
    }
  }
}
