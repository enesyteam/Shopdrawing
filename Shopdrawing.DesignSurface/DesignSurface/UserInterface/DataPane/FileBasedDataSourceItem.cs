// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.FileBasedDataSourceItem
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.Framework.Data;
using Microsoft.Expression.Project;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public class FileBasedDataSourceItem : DataSourceItem
  {
    public IProjectItem DesignDataFile { get; private set; }

    public override string UniqueId
    {
      get
      {
        return this.DesignDataFile.DocumentReference.Path;
      }
    }

    public override string DisplayName
    {
      get
      {
        return Path.GetFileNameWithoutExtension(this.DesignDataFile.DocumentReference.Path);
      }
      set
      {
      }
    }

    public override bool HasErrors
    {
      get
      {
        return !string.IsNullOrEmpty(this.ErrorMessage);
      }
    }

    public override string ErrorMessage
    {
      get
      {
        string format;
        if (this.SchemaItem.Schema is EmptySchema)
        {
          format = StringTable.DataSourceErrorBecauseOfFile;
        }
        else
        {
          SceneDocument sceneDocument = this.DesignDataFile.Document as SceneDocument;
          if (sceneDocument == null || sceneDocument.XamlDocument.ParseErrorsCount == 0)
            return (string) null;
          format = StringTable.XamlErrorsInSampleOrDesignDataFile;
        }
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, new object[1]
        {
          (object) this.DesignDataFile.ProjectRelativeDocumentReference.TrimStart('/', '\\')
        });
      }
    }

    public override bool CanEditData
    {
      get
      {
        return true;
      }
    }

    public ICommand EditData
    {
      get
      {
        return (ICommand) new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.EditDesignDataImpl));
      }
    }

    public override string ToolTip
    {
      get
      {
        return StringTable.DesignDataSourceTooltip;
      }
    }

    public FileBasedDataSourceItem(ISchema schema, DataPanelModel model, IProjectItem designDataFile)
      : base(schema, model)
    {
      this.DesignDataFile = designDataFile;
    }

    private void EditDesignDataImpl()
    {
      SceneView sceneView = this.DesignDataFile.OpenView(true) as SceneView;
      if (sceneView == null)
        return;
      sceneView.ViewMode = ViewMode.Code;
    }
  }
}
