// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.Code.UserInterface.CodeOptionsModel
// Assembly: Microsoft.Expression.Code, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3445693A-E9B1-4B68-8C1A-000C20F2A3F8
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.Code.dll

using Microsoft.Expression.Framework.Configuration;
using Microsoft.VisualStudio.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Microsoft.Expression.Code.UserInterface
{
  public class CodeOptionsModel : INotifyPropertyChanged
  {
    private EditorSpecificOptionsModel xamlOptions = new EditorSpecificOptionsModel(StringTable.XamlEditorOption, "");
    private EditorSpecificOptionsModel codeOptions = new EditorSpecificOptionsModel(StringTable.OtherEditorOption, "Code_");
    private int lastSelectedEditor;
    private static int DefaultLastSelectedEditor;
    private ObservableCollection<EditorSpecificOptionsModel> editorSpecificOptions;
    private Dictionary<EditorType, EditorSpecificOptionsModel> editorOptionsLookup;

    public int LastSelectedEditor
    {
      get
      {
        return this.lastSelectedEditor;
      }
      set
      {
        this.lastSelectedEditor = value;
      }
    }

    public ObservableCollection<EditorSpecificOptionsModel> EditorSpecificOptions
    {
      get
      {
        return this.editorSpecificOptions;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged
    {
      add
      {
      }
      remove
      {
      }
    }

    public CodeOptionsModel()
    {
      this.editorSpecificOptions = new ObservableCollection<EditorSpecificOptionsModel>();
      this.editorSpecificOptions.Add(this.xamlOptions);
      this.editorSpecificOptions.Add(this.codeOptions);
      this.editorOptionsLookup = new Dictionary<EditorType, EditorSpecificOptionsModel>();
      this.editorOptionsLookup[EditorType.XamlEditor] = this.xamlOptions;
      this.editorOptionsLookup[EditorType.CodeEditor] = this.codeOptions;
    }

    public EditorType GetEditorType(ITextBuffer textBuffer)
    {
      return !(textBuffer.ContentType.TypeName == "text.xml") && !(textBuffer.ContentType.TypeName == "text.xaml") ? EditorType.CodeEditor : EditorType.XamlEditor;
    }

    public EditorSpecificOptionsModel GetEditorModel(EditorType editorType)
    {
      return this.editorOptionsLookup[editorType];
    }

    public void Load(IConfigurationObject value)
    {
      this.LastSelectedEditor = (int) value.GetProperty("LastSelectedIndex", (object) CodeOptionsModel.DefaultLastSelectedEditor);
      this.LoadCore(value);
    }

    public void LoadMaintainingSelection(IConfigurationObject value)
    {
      value.SetProperty("LastSelectedIndex", (object) this.LastSelectedEditor);
      this.LoadCore(value);
    }

    private void LoadCore(IConfigurationObject value)
    {
      this.xamlOptions.Load(value);
      this.codeOptions.Load(value);
    }

    public void Save(IConfigurationObject value)
    {
      value.SetProperty("LastSelectedIndex", (object) this.LastSelectedEditor);
      this.xamlOptions.Save(value);
      this.codeOptions.Save(value);
    }
  }
}
