// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.EditControlCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Globalization;
using System.IO;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class EditControlCommand : SingleTargetCommandBase
  {
    public override bool IsAvailable
    {
      get
      {
        return JoltHelper.TypeSupported((ITypeResolver) this.SceneViewModel.ProjectContext, PlatformTypes.Control);
      }
    }

    public override bool IsEnabled
    {
      get
      {
        bool flag = base.IsEnabled;
        if (flag)
        {
          flag = false;
          SceneElement targetElement = this.TargetElement;
          if (targetElement != null && targetElement.DocumentNode != null && targetElement != this.SceneViewModel.RootNode)
            flag = targetElement.DocumentNode.Type.XamlSourcePath != null;
        }
        return flag;
      }
    }

    internal EditControlCommand(SceneViewModel model)
      : base(model)
    {
    }

    public override void Execute()
    {
      EditControlCommand.EditControl(this.TargetElement);
    }

    internal static void EditControl(SceneElement sceneElement)
    {
      if (sceneElement == null || sceneElement.DocumentNode == null)
        return;
      IType type = sceneElement.DocumentNode.Type;
      if (type.XamlSourcePath == null)
        return;
      try
      {
        IDocumentRoot documentRoot = sceneElement.ProjectContext.GetDocumentRoot(type.XamlSourcePath);
        ISceneViewHost sceneViewHost = sceneElement.ProjectContext as ISceneViewHost;
        if (sceneViewHost == null || documentRoot == null)
          return;
        sceneViewHost.OpenView(documentRoot, true);
      }
      catch (Exception ex)
      {
        if (ex is ArgumentException || ex is IOException || (ex is InvalidOperationException || ex is UnauthorizedAccessException))
        {
          string message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.FileOpenFailedDialogMessage, new object[2]
          {
            (object) Path.GetFileName(type.XamlSourcePath),
            (object) ex.Message
          });
          sceneElement.DesignerContext.MessageDisplayService.ShowError(message);
        }
        else
          throw;
      }
    }
  }
}
