// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Selection.DesignTimeTextSelection
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.ViewObjects;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.Selection
{
  public class DesignTimeTextSelection
  {
    private SceneElement textElement;
    private int start;
    private int end;
    private int length;

    public bool CanDelete
    {
      get
      {
        if (this.start == this.end)
          return this.end < this.length;
        return true;
      }
    }

    public bool CanCopy
    {
      get
      {
        return this.start != this.end;
      }
    }

    public DesignTimeTextSelection(SceneElement textElement, IViewTextBox editingTextBox)
    {
      this.textElement = textElement;
      this.start = editingTextBox.SelectionStart;
      this.end = editingTextBox.SelectionStart + editingTextBox.SelectionLength;
      this.length = editingTextBox.Text.Length;
    }

    public DesignTimeTextSelection(SceneElement textElement, IViewFlowDocumentEditor editingTextBox)
    {
      this.textElement = textElement;
      this.start = editingTextBox.Document.ContentStart.GetOffsetToPosition(editingTextBox.Selection.Start);
      this.end = editingTextBox.Document.ContentStart.GetOffsetToPosition(editingTextBox.Selection.End);
      this.length = editingTextBox.Document.ContentStart.GetOffsetToPosition(editingTextBox.Document.ContentEnd);
    }

    public override string ToString()
    {
      return this.textElement.ToString() + (object) " [" + (string) (object) this.start + ", " + (string) (object) this.end + "]";
    }
  }
}
