// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.SceneCommands.ExpandControlCommand
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Globalization;

namespace Microsoft.Expression.DesignSurface.SceneCommands
{
  internal sealed class ExpandControlCommand : SingleTargetCommandBase
  {
    private string Text
    {
      get
      {
        string str = string.Empty;
        if (this.Type != null)
          str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.UndoUnitShowPopup, new object[1]
          {
            (object) this.Type.Name
          });
        return str;
      }
    }

    private string UndoString
    {
      get
      {
        string str = string.Empty;
        if (this.Type != null)
          str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, this.IsExpanded ? StringTable.UndoUnitHidePopup : StringTable.UndoUnitShowPopup, new object[1]
          {
            (object) this.Type.Name
          });
        return str;
      }
    }

    private bool IsExpanded
    {
      get
      {
        IExpandable expandable = this.TargetElement as IExpandable;
        if (expandable != null && this.TargetElement.IsViewObjectValid)
          return (bool) this.TargetElement.GetComputedValue(expandable.ExpansionProperty) || (bool) this.TargetElement.GetComputedValue(expandable.DesignTimeExpansionProperty);
        return false;
      }
      set
      {
        IExpandable expandable = this.TargetElement as IExpandable;
        if (expandable == null)
          return;
        using (this.SceneViewModel.ForceBaseValue())
        {
          if (!value)
            this.TargetElement.ClearValue(expandable.DesignTimeExpansionProperty);
          this.TargetElement.SetValue(expandable.ExpansionProperty, (object) (bool) (value ? true : false));
        }
      }
    }

    public ExpandControlCommand(SceneViewModel sceneViewModel)
      : base(sceneViewModel)
    {
    }

    public override void Execute()
    {
      throw new InvalidOperationException();
    }

    public override object GetProperty(string propertyName)
    {
      if (propertyName == "IsChecked")
        return (object) (bool) (this.IsExpanded ? true : false);
      if (propertyName == "Text")
        return (object) this.Text;
      return base.GetProperty(propertyName);
    }

    public override void SetProperty(string propertyName, object propertyValue)
    {
      if (propertyName == "IsChecked")
      {
        PerformanceUtility.StartPerformanceSequence(PerformanceEvent.ExpandPopup);
        using (SceneEditTransaction editTransaction = this.SceneViewModel.CreateEditTransaction(this.UndoString, false))
        {
          this.IsExpanded = (bool) propertyValue;
          editTransaction.Commit();
        }
        PerformanceUtility.EndPerformanceSequence(PerformanceEvent.ExpandPopup);
      }
      base.SetProperty(propertyName, propertyValue);
    }
  }
}
