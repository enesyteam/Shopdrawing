// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.CreatePropertyBindingPickWhipStrategy
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.DesignSurface.ViewModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class CreatePropertyBindingPickWhipStrategy : IElementSelectionStrategy
  {
    private ReferenceStep targetProperty;
    private SceneElement lastElement;
    private bool lastCanSelectResult;

    public CreatePropertyBindingPickWhipStrategy(ReferenceStep targetProperty)
    {
      this.targetProperty = targetProperty;
    }

    public bool CanSelectElement(SceneElement element)
    {
      if (element == null)
        return false;
      if (this.lastElement != element)
      {
        this.lastElement = element;
        this.lastCanSelectResult = element.CanNameElement && MiniSourceBindingDialogModel.AnyBindableProperties((SceneNode) element, this.targetProperty.PropertyType);
      }
      return this.lastCanSelectResult;
    }

    public void SelectElement(SceneElement element, SceneNodeProperty editingProperty)
    {
      ISchema schemaForDataSource = SchemaManager.GetSchemaForDataSource((SceneNode) element);
      using (SceneEditTransaction editTransaction = element.ViewModel.CreateEditTransaction(StringTable.CreateElementPropertyBindingUndo))
      {
        BindingSceneNode elementNameBinding = MiniBindingDialog.CreateElementNameBinding(new DataSchemaNodePath(schemaForDataSource, schemaForDataSource.Root), (SceneNode) element, editingProperty.PropertyTypeId);
        if (elementNameBinding != null)
        {
          editingProperty.SetValue((object) elementNameBinding.DocumentNode);
          editTransaction.Commit();
        }
        else
          editTransaction.Cancel();
      }
    }
  }
}
