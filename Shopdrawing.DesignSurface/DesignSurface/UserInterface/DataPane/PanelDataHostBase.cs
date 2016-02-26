// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.DataPane.PanelDataHostBase
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.UserInterface.DataPane
{
  public abstract class PanelDataHostBase : IDataHost
  {
    private ITypeId panelType;

    public string ElementName
    {
      get
      {
        return this.panelType.Name;
      }
    }

    public string PropertyName
    {
      get
      {
        return "Children";
      }
    }

    public ITypeId PanelType
    {
      get
      {
        return this.panelType;
      }
    }

    protected PanelDataHostBase(ITypeId panelType)
    {
      this.panelType = panelType;
    }

    public virtual BaseFrameworkElement BuildPanel(SceneViewModel viewModel, ICollection<BaseFrameworkElement> children)
    {
      BaseFrameworkElement frameworkElement1 = (BaseFrameworkElement) viewModel.CreateSceneNode(this.PanelType);
      ISceneNodeCollection<SceneNode> collectionForProperty = frameworkElement1.GetCollectionForProperty(PanelElement.ChildrenProperty);
      foreach (BaseFrameworkElement frameworkElement2 in (IEnumerable<BaseFrameworkElement>) children)
        collectionForProperty.Add((SceneNode) frameworkElement2);
      return frameworkElement1;
    }
  }
}
