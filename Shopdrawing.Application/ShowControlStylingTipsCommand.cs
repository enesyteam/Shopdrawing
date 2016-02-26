// Decompiled with JetBrains decompiler
// Type: Shopdrawing.App.ShowControlStylingTipsCommand
// Assembly: Shopdrawing.App, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: DDD2F1CF-BB6D-4068-A4D9-DDBDD16D6739
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Shopdrawing.App.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface.SkinEditing;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shopdrawing.App
{
  internal class ShowControlStylingTipsCommand : Command
  {
    private IServices services;

    public ShowControlStylingTipsCommand(IServices services)
    {
      this.services = services;
    }

    public override void Execute()
    {
      ControlTemplateElement controlTemplateElement = ((SceneView) this.services.GetService<IViewService>().ActiveView).ViewModel.ActiveEditingContainer as ControlTemplateElement;
      if (controlTemplateElement == null)
        return;
      ITypeId index = Enumerable.FirstOrDefault<ITypeId>((IEnumerable<ITypeId>) PartsModel.ControlEditingTipsGuids.Keys, (Func<ITypeId, bool>) (type => type.Equals((object) controlTemplateElement.ControlTemplateTargetTypeId)));
      if (index == null)
        return;
      BlendHelp.Instance.ShowHelpTopic("/html/" + PartsModel.ControlEditingTipsGuids[index] + ".htm");
    }
  }
}
