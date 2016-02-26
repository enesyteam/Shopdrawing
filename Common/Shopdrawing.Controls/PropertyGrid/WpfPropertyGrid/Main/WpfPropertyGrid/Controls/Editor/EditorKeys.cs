using System.Windows;

namespace System.Windows.Controls.WpfPropertyGrid.Controls
{
  public static class EditorKeys
  {
    private static ComponentResourceKey _FilePathPickerEditorKey =
      new ComponentResourceKey(typeof(EditorKeys), "FilePathPickerEditor");

    public static ComponentResourceKey FilePathPickerEditorKey
    {
      get { return _FilePathPickerEditorKey; }
    }
  }
}
