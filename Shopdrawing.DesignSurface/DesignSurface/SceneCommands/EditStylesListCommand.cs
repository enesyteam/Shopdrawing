// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.EditStylesListCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class EditStylesListCommand : ControlStylingPropertyListCommandBase
  {
    private ISceneViewHost viewHost;

    public EditStylesListCommand(ISceneViewHost viewHost, SceneViewModel viewModel)
      : base(viewModel)
    {
      this.viewHost = viewHost;
    }

    protected override bool IsCommandProperty(ReferenceStep referenceStep)
    {
      if (base.IsCommandProperty(referenceStep))
        return EditStylesListCommand.IsCommandProperty(this.TargetElement, referenceStep);
      return false;
    }

    internal static bool IsCommandProperty(SceneElement element, ReferenceStep referenceStep)
    {
      if (PlatformTypes.Style.IsAssignableFrom((ITypeId) referenceStep.PropertyType) && !referenceStep.Equals((object) BaseFrameworkElement.StyleProperty))
        return TypeHelper.IsPropertyWritable((ITypeResolver) element.ViewModel.ProjectContext, (IProperty) referenceStep, element.IsSubclassDefinition);
      return false;
    }

    protected override IList<Control> GeneratePropertyItems(SceneElement targetElement, ReferenceStep targetProperty)
    {
      List<Control> list = new List<Control>();
      list.Add((Control) this.CreateMenuItem(StringTable.EditExistingStyleCommandName, "EditExistingStyleCommand", (ICommand) new EditExistingStyleTemplateCommand(this.viewHost, this.ViewModel, (IPropertyId) targetProperty)));
      list.Add((Control) this.CreateMenuItem(StringTable.EditClonedStyleCommandName, "EditClonedStyleCommand", (ICommand) new EditCopyOfStyleCommand(this.viewHost, this.ViewModel, (IPropertyId) targetProperty)));
      list.Add((Control) this.CreateMenuItem(StringTable.EditEmptyStyleCommandName, "EditEmptyStyleCommand", (ICommand) new EditNewStyleCommand(this.viewHost, this.ViewModel, (IPropertyId) targetProperty)));
      Separator separator = new Separator();
      list.Add((Control) separator);
      ResourceListCommand resourceListCommand = new ResourceListCommand(this.ViewModel, (IPropertyId) targetProperty);
      list.Add((Control) resourceListCommand.CreateSingleInstance());
      return (IList<Control>) list;
    }
  }
}
