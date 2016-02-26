// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.EditTemplatesListCommand
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
  internal class EditTemplatesListCommand : ControlStylingPropertyListCommandBase
  {
    private ISceneViewHost viewHost;

    public EditTemplatesListCommand(ISceneViewHost viewHost, SceneViewModel viewModel)
      : base(viewModel)
    {
      this.viewHost = viewHost;
    }

    protected override bool IsCommandProperty(ReferenceStep referenceStep)
    {
      if (base.IsCommandProperty(referenceStep) && EditTemplatesListCommand.IsCommandProperty(this.TargetElement, referenceStep))
        return true;
      if (EditStylesListCommand.IsCommandProperty(this.TargetElement, referenceStep))
        return ControlStylingOperations.DoesStyleTargetControl(this.ViewModel.ProjectContext, this.TargetElement.Type, (IPropertyId) referenceStep);
      return false;
    }

    internal static bool IsCommandProperty(SceneElement element, ReferenceStep referenceStep)
    {
      if (TypeHelper.IsPropertyWritable((ITypeResolver) element.ViewModel.ProjectContext, (IProperty) referenceStep, element.IsSubclassDefinition) && PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) referenceStep.PropertyType) && !ControlElement.TemplateProperty.Equals((object) referenceStep))
        return !PageElement.TemplateProperty.Equals((object) referenceStep);
      return false;
    }

    protected override IList<Control> GeneratePropertyItems(SceneElement targetElement, ReferenceStep targetProperty)
    {
      List<Control> list = new List<Control>();
      if (PlatformTypes.FrameworkTemplate.IsAssignableFrom((ITypeId) targetProperty.PropertyType))
      {
        list.Add((Control) this.CreateMenuItem(StringTable.EditExistingTemplateCommandName, "EditExistingTemplateCommand", (ICommand) new EditExistingStyleTemplateCommand(this.viewHost, this.ViewModel, (IPropertyId) targetProperty)));
        list.Add((Control) this.CreateMenuItem(StringTable.EditClonedStyleCommandName, "EditClonedTemplateCommand", (ICommand) new EditCopyOfTemplateCommand(this.viewHost, this.ViewModel, (IPropertyId) targetProperty)));
        list.Add((Control) this.CreateMenuItem(StringTable.EditEmptyStyleCommandName, "EditEmptyTemplateCommand", (ICommand) new EditNewTemplateCommand(this.viewHost, this.ViewModel, (IPropertyId) targetProperty)));
      }
      else if (PlatformTypes.Style.IsAssignableFrom((ITypeId) targetProperty.PropertyType))
      {
        list.Add((Control) this.CreateMenuItem(StringTable.EditExistingTemplateCommandName, "EditExistingTemplateCommand", (ICommand) new EditExistingStyleTemplateCommand(this.viewHost, this.ViewModel, (IPropertyId) targetProperty, true)));
        list.Add((Control) this.CreateMenuItem(StringTable.EditClonedStyleCommandName, "EditClonedTemplateCommand", (ICommand) new EditCopyOfStyleWithTemplateCommand(this.viewHost, this.ViewModel, (IPropertyId) targetProperty)));
        list.Add((Control) this.CreateMenuItem(StringTable.EditEmptyStyleCommandName, "EditEmptyTemplateCommand", (ICommand) new EditNewStyleWithTemplateCommand(this.viewHost, this.ViewModel, (IPropertyId) targetProperty)));
      }
      Separator separator = new Separator();
      list.Add((Control) separator);
      ResourceListCommand resourceListCommand = new ResourceListCommand(this.ViewModel, (IPropertyId) targetProperty);
      list.Add((Control) resourceListCommand.CreateSingleInstance());
      return (IList<Control>) list;
    }
  }
}
