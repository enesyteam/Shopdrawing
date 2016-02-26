// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.PlatformUI.Shell.Controls.IDockPreviewWindow
// Assembly: Microsoft.Expression.Framework, Version=4.0.1000.1000, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1CFB9CAE-EE8F-44DB-B6AB-EAABBC8A4B40
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Framework.dll

namespace Microsoft.VisualStudio.PlatformUI.Shell.Controls
{
  public interface IDockPreviewWindow
  {
    double Top { get; set; }

    double Left { get; set; }

    double Width { get; set; }

    double Height { get; set; }

    void Show();

    void Hide();

    void Close();
  }
}
