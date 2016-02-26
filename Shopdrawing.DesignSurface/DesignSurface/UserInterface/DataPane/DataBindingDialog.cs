// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.DataBindingDialog
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using System;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  internal sealed class DataBindingDialog : Dialog, IDisposable
  {
    private DataBindingDialogModel model;
    private DataPanelModel dataPanelModel;

    private DataBindingDialog(SceneNode targetElement, ReferenceStep targetProperty)
    {
      FrameworkElement element = Microsoft.Expression.DesignSurface.FileTable.GetElement("Resources\\DataPane\\DataBindingDialog.xaml");
      this.DialogContent = (UIElement) element;
      this.ResizeMode = ResizeMode.NoResize;
      this.SizeToContent = SizeToContent.WidthAndHeight;
      this.Title = StringTable.CreateDatabindingDialogTitle;
      this.dataPanelModel = new DataPanelModel(targetElement.ViewModel, true);
      this.model = new DataBindingDialogModel(this.dataPanelModel, targetElement, targetProperty);
      element.DataContext = (object) this.model;
    }

    public void Dispose()
    {
      this.model.Teardown();
      this.dataPanelModel.Unload();
    }

    public static SceneNode CreateAndSetBindingOrData(DesignerContext designerContext, SceneNode target, ReferenceStep targetProperty)
    {
      SceneNode sceneNode = (SceneNode) null;
      using (DataBindingDialog dataBindingDialog = new DataBindingDialog(target, targetProperty))
      {
        bool? nullable = dataBindingDialog.ShowDialog();
        if ((!nullable.GetValueOrDefault() ? 1 : (!nullable.HasValue ? true : false)) != 0)
          return (SceneNode) null;
        using (target.ViewModel.AnimationEditor.DeferKeyFraming())
        {
          using (SceneEditTransaction editTransaction = target.ViewModel.CreateEditTransaction(StringTable.UndoUnitSetBinding))
          {
            sceneNode = dataBindingDialog.model.CreateBindingOrData();
            bool flag = sceneNode != null;
            BindingSceneNode binding = sceneNode as BindingSceneNode;
            if (binding != null)
            {
              flag = false;
              if (binding.ElementBindingTarget != null)
              {
                binding.ElementBindingTarget.EnsureNamed();
                binding.ElementName = binding.ElementBindingTarget.Name;
              }
              string errorMessage;
              if (!binding.IsBindingLegal(target, targetProperty, out errorMessage))
              {
                MessageBoxArgs args = new MessageBoxArgs()
                {
                  Message = errorMessage,
                  Button = MessageBoxButton.OK,
                  Image = MessageBoxImage.Hand
                };
                int num = (int) designerContext.MessageDisplayService.ShowMessage(args);
              }
              else
              {
                IPropertyId propertyKey = (IPropertyId) targetProperty;
                if (binding.Source != null)
                {
                  ISchema schema = dataBindingDialog.model.CurrentBindingSource.Schema;
                  if (schema != null && schema.DataSource != null && schema.DataSource.DocumentNode != null)
                    propertyKey = BindingEditor.RefineDataContextProperty(target, (IPropertyId) targetProperty, schema.DataSource.DocumentNode);
                }
                target.SetBinding(propertyKey, binding);
                flag = true;
              }
            }
            if (flag)
              editTransaction.Commit();
            else
              editTransaction.Cancel();
          }
        }
        return sceneNode;
      }
    }
  }
}
