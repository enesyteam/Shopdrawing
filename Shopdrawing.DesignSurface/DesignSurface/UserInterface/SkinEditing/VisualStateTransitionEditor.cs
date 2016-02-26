// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.SkinEditing.VisualStateTransitionEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.PropertyInspector;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.SkinEditing
{
  public class VisualStateTransitionEditor : IPropertyInspector
  {
    private PropertyEditingHelper helper;
    private VisualStateTransitionSceneNode sceneNode;
    private VisualTransitionObjectSet transitionSet;

    public VisualTransitionObjectSet VisualTransitionObjectSet
    {
      get
      {
        return this.transitionSet;
      }
    }

    public VisualStateTransitionEditor(VisualStateTransitionSceneNode sceneNode)
    {
      this.sceneNode = sceneNode;
      this.helper = new PropertyEditingHelper(this.sceneNode.DesignerContext.ActiveDocument, (UIElement) null);
      this.transitionSet = new VisualTransitionObjectSet(sceneNode, this);
    }

    public bool IsCategoryExpanded(string categoryName)
    {
      return true;
    }

    public void UpdateTransaction()
    {
      this.helper.UpdateTransaction();
    }
  }
}
