// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.View.XamlDesignChooserCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.Commands;

namespace Microsoft.Expression.DesignSurface.View
{
  internal sealed class XamlDesignChooserCommand : Command
  {
    private SceneView sceneView;
    private Command designCommand;
    private Command xamlCommand;

    public override bool IsEnabled
    {
      get
      {
        if (!this.IsEnabled)
          return false;
        if (this.sceneView.FocusedEditor == FocusedEditor.Code)
          return this.xamlCommand.IsEnabled;
        return this.designCommand.IsEnabled;
      }
    }

    public override bool IsAvailable
    {
      get
      {
        if (this.sceneView.FocusedEditor == FocusedEditor.Code)
          return this.xamlCommand.IsAvailable;
        return this.designCommand.IsAvailable;
      }
    }

    public XamlDesignChooserCommand(SceneView sceneView, Command designCommand, Command xamlCommand)
    {
      this.sceneView = sceneView;
      this.designCommand = designCommand;
      this.xamlCommand = xamlCommand;
    }

    public override void Execute()
    {
      if (this.sceneView.FocusedEditor == FocusedEditor.Code)
        this.xamlCommand.Execute();
      else
        this.designCommand.Execute();
    }

    public override object GetProperty(string propertyName)
    {
      if (this.sceneView.FocusedEditor == FocusedEditor.Code)
        return this.xamlCommand.GetProperty(propertyName);
      return this.designCommand.GetProperty(propertyName);
    }

    public override void SetProperty(string propertyName, object propertyValue)
    {
      if (this.sceneView.FocusedEditor == FocusedEditor.Code)
        this.xamlCommand.SetProperty(propertyName, propertyValue);
      else
        this.designCommand.SetProperty(propertyName, propertyValue);
    }
  }
}
