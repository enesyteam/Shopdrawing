// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.EditExistingStyleTemplateCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using System;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal class EditExistingStyleTemplateCommand : ControlStylingCommandBase
  {
    private bool editTemplate;

    private DocumentNodePath StyleOrTemplateNodePath
    {
      get
      {
        if (!this.editTemplate)
          return ControlStylingOperations.ProvideStyleOrTemplateNodePath(this.TargetElement, this.TargetPropertyReference);
        return ControlStylingOperations.ResolveNodePathForTemplateWithinExistingStyle(this.TargetElement, this.TargetPropertyReference);
      }
    }

    public override bool IsEnabled
    {
      get
      {
        if (base.IsEnabled && this.HasValidTarget)
          return this.StyleOrTemplateNodePath != null;
        return false;
      }
    }

    public EditExistingStyleTemplateCommand(ISceneViewHost viewHost, SceneViewModel viewModel, IPropertyId targetProperty)
      : this(viewHost, viewModel, targetProperty, false)
    {
    }

    public EditExistingStyleTemplateCommand(ISceneViewHost viewHost, SceneViewModel viewModel, IPropertyId targetProperty, bool editTemplate)
      : base(viewHost, viewModel, targetProperty)
    {
      this.editTemplate = editTemplate;
    }

    public EditExistingStyleTemplateCommand(ISceneViewHost viewHost, SceneViewModel viewModel)
      : base(viewHost, viewModel, (IPropertyId) null, true)
    {
    }

    public override void Execute()
    {
      using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.UndoUnitEditStyleTemplate, new object[1]
      {
        (object) this.TargetPropertyReference.Path
      }), 1 != 0))
      {
        this.SceneView.CandidateEditingContainer = this.TargetElement.DocumentNodePath;
        editTransaction.Update();
        DocumentNodePath templateNodePath = this.StyleOrTemplateNodePath;
        this.SceneView.CandidateEditingContainer = (DocumentNodePath) null;
        if (templateNodePath != null)
        {
          bool preferInPlaceEdit = ControlStylingOperations.CanEditInPlace(this.TargetElement, this.TargetProperty, templateNodePath);
          ControlStylingOperations.SetActiveEditingContainer(this.TargetElement, this.TargetProperty, templateNodePath.Node, templateNodePath, preferInPlaceEdit, editTransaction);
        }
        editTransaction.Commit();
      }
    }
  }
}
