// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Framework.Controls.ExpressionOpenFileDialog
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

using Microsoft.Expression.Framework;
using Microsoft.Win32;
using System.Collections.Generic;

namespace Microsoft.Expression.Framework.Controls
{
  public sealed class ExpressionOpenFileDialog : ExpressionFileDialog
  {
    private OpenFileDialog win32Dialog;
    private NativeFileOpenDialog dialog;
    private bool multiselect;
    private List<string> fileNames;

    public bool Multiselect
    {
      get
      {
        return this.multiselect;
      }
      set
      {
        this.multiselect = value;
      }
    }

    public string FileName
    {
      get
      {
        this.EnsureFileNames();
        int count = this.fileNames.Count;
        return this.fileNames[0];
      }
    }

    public string[] FileNames
    {
      get
      {
        this.EnsureFileNames();
        return this.fileNames.ToArray();
      }
    }

    internal override IFileDialog NativeFileDialog
    {
      get
      {
        if (this.dialog == null)
          this.dialog = (NativeFileOpenDialog) new FileOpenDialogRCW();
        return (IFileDialog) this.dialog;
      }
    }

    protected override FileDialog Win32FileDialog
    {
      get
      {
        if (this.win32Dialog == null)
          this.win32Dialog = new OpenFileDialog();
        return (FileDialog) this.win32Dialog;
      }
    }

    public ExpressionOpenFileDialog()
      : this(true)
    {
    }

    public ExpressionOpenFileDialog(bool preferVistaDialog)
      : base(preferVistaDialog)
    {
    }

    private void EnsureFileNames()
    {
      if (this.dialog != null && this.fileNames == null)
      {
        this.fileNames = new List<string>();
        IShellItemArray ppenum;
        this.dialog.GetResults(out ppenum);
        uint pdwNumItems;
        ppenum.GetCount(out pdwNumItems);
        for (uint dwIndex = 0U; dwIndex < pdwNumItems; ++dwIndex)
        {
          IShellItem ppsi;
          ppenum.GetItemAt(dwIndex, out ppsi);
          string ppszName;
          ppsi.GetDisplayName(NativeMethods.SIGDN.SIGDN_FILESYSPATH, out ppszName);
          this.fileNames.Add(ppszName);
        }
      }
      else
      {
        if (this.win32Dialog == null || this.fileNames != null)
          return;
        this.fileNames = new List<string>((IEnumerable<string>) this.win32Dialog.FileNames);
      }
    }

    protected override void InitializeWin32FileDialog(FileDialog dialog)
    {
      ((OpenFileDialog) dialog).Multiselect = this.multiselect;
      base.InitializeWin32FileDialog(dialog);
    }

    internal override NativeMethods.FOS GenerateNativeDialogOptions()
    {
      NativeMethods.FOS fos = (NativeMethods.FOS) 0;
      if (this.Multiselect)
        fos |= NativeMethods.FOS.FOS_ALLOWMULTISELECT;
      return fos | base.GenerateNativeDialogOptions();
    }
  }
}
