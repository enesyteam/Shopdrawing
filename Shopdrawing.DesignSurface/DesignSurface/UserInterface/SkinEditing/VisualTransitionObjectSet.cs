// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.SkinEditing.VisualTransitionObjectSet
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.UserInterface.ResourcePane;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.PropertyInspector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Expression.DesignSurface.UserInterface.SkinEditing
{
  public class VisualTransitionObjectSet : SceneNodeObjectSetBase
  {
    private VisualStateTransitionSceneNode sceneNode;

    public override SceneViewModel ViewModel
    {
      get
      {
        if (this.sceneNode == null)
          return (SceneViewModel) null;
        return this.sceneNode.ViewModel;
      }
    }

    public override ObservableCollection<LocalResourceModel> LocalResources
    {
      get
      {
        return (ObservableCollection<LocalResourceModel>) this.ProvideLocalResources(new List<ResourceContainer>(this.DesignerContext.ResourceManager.ActiveResourceContainers));
      }
    }

    public override bool ShouldWalkParentsForGetValue
    {
      get
      {
        return false;
      }
    }

    public override bool ShouldAllowAnimation
    {
      get
      {
        return false;
      }
    }

    public override bool IsValidForUpdate
    {
      get
      {
        return this.sceneNode.DesignerContext != null;
      }
    }

    public override SceneNode[] Objects
    {
      get
      {
        return new SceneNode[1]
        {
          (SceneNode) this.sceneNode
        };
      }
    }

    public override bool IsHomogenous
    {
      get
      {
        throw new NotImplementedException(ExceptionStringTable.MethodOrOperationIsNotImplemented);
      }
    }

    public override bool CanSetBindingExpression
    {
      get
      {
        if (!this.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsBindingInVisualStateManager))
          return false;
        return base.CanSetBindingExpression;
      }
    }

    public override bool CanSetDynamicExpression
    {
      get
      {
        if (!this.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsDynamicResourceInVisualStateManager))
          return false;
        return base.CanSetDynamicExpression;
      }
    }

    public VisualTransitionObjectSet(VisualStateTransitionSceneNode sceneNode, VisualStateTransitionEditor transactionContext)
      : base(sceneNode.DesignerContext, (IPropertyInspector) transactionContext)
    {
      this.sceneNode = sceneNode;
    }

    public override void RegisterPropertyChangedHandler(PropertyReference propertyReference, Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler handler)
    {
      if (this.DesignerContext.PropertyManager == null)
        return;
      this.DesignerContext.PropertyManager.RegisterPropertyReferenceChangedHandler(propertyReference, handler, true);
    }

    public override void UnregisterPropertyChangedHandler(PropertyReference propertyReference, Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler handler)
    {
      if (this.DesignerContext.PropertyManager == null)
        return;
      this.DesignerContext.PropertyManager.UnregisterPropertyReferenceChangedHandler(propertyReference, handler);
    }

    public override SceneEditTransaction CreateEditTransaction(string description)
    {
      return this.Document.CreateEditTransaction(description);
    }

    public override SceneEditTransaction CreateEditTransaction(string description, bool hidden)
    {
      return this.Document.CreateEditTransaction(description, hidden);
    }
  }
}
