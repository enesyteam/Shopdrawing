// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.MiniBindingDialog
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.UserInterface;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  internal sealed class MiniBindingDialog : Dialog
  {
    private MiniBindingDialogModel model;

    private MiniBindingDialog(MiniBindingDialogModel model)
    {
      FrameworkElement element = FileTable.GetElement("Resources\\DataPane\\MiniBindingDialog.xaml");
      this.DialogContent = (UIElement) element;
      this.SizeToContent = SizeToContent.WidthAndHeight;
      this.Title = StringTable.CreateDatabindingDialogTitle;
      this.model = model;
      element.DataContext = (object) this.model;
    }

    public static BindingSceneNode CreateElementNameBinding(DataSchemaNodePath bindingPath, SceneNode target, IType targetPropertyType)
    {
      MiniSourceBindingDialogModel bindingDialogModel = new MiniSourceBindingDialogModel(bindingPath, target, targetPropertyType);
      bool? nullable = new MiniBindingDialog((MiniBindingDialogModel) bindingDialogModel).ShowDialog();
      if ((!nullable.GetValueOrDefault() ? 1 : (!nullable.HasValue ? true : false)) != 0)
        return (BindingSceneNode) null;
      return bindingDialogModel.CreateElementNameBinding(target);
    }

    public static SceneNode CreateAndSetBindingOrData(DataSchemaNodePath bindingPath, SceneNode target, bool useDesignDataContext, ref ReferenceStep targetProperty)
    {
      MiniTargetBindingDialogModel bindingDialogModel = new MiniTargetBindingDialogModel(bindingPath, target, targetProperty, useDesignDataContext);
      MiniBindingDialog miniBindingDialog = new MiniBindingDialog((MiniBindingDialogModel) bindingDialogModel);
      if (targetProperty == null)
      {
        using (TemporaryCursor.SetCursor((Cursor) null))
        {
          bool? nullable = miniBindingDialog.ShowDialog();
          if ((!nullable.GetValueOrDefault() ? 1 : (!nullable.HasValue ? true : false)) != 0)
            return (SceneNode) null;
        }
      }
      return bindingDialogModel.CreateAndSetBindingOrData(ref targetProperty);
    }
  }
}
