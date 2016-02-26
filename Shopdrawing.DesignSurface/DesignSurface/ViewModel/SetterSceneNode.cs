// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.SetterSceneNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Selection;
using System;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class SetterSceneNode : SceneNode, ISceneElementSubpart, IComparable
  {
    public static readonly IPropertyId PropertyProperty = (IPropertyId) PlatformTypes.Setter.GetMember(MemberType.LocalProperty, "Property", MemberAccessTypes.Public);
    public static readonly IPropertyId ValueProperty = (IPropertyId) PlatformTypes.Setter.GetMember(MemberType.LocalProperty, "Value", MemberAccessTypes.Public);
    public static readonly IPropertyId TargetNameProperty = (IPropertyId) PlatformTypes.Setter.GetMember(MemberType.LocalProperty, "TargetName", MemberAccessTypes.Public);
    public static readonly SetterSceneNode.ConcreteSetterSceneNodeFactory Factory = new SetterSceneNode.ConcreteSetterSceneNodeFactory();

    public DependencyPropertyReferenceStep Property
    {
      get
      {
        return DocumentPrimitiveNode.GetValueAsMember(((DocumentCompositeNode) this.DocumentNode).Properties[SetterSceneNode.PropertyProperty]) as DependencyPropertyReferenceStep;
      }
      set
      {
        DocumentNode valueNode = (DocumentNode) null;
        if (value != null)
          valueNode = (DocumentNode) this.DocumentContext.CreateNode(PlatformTypes.DependencyProperty, (IDocumentNodeValue) new DocumentNodeMemberValue((IMember) value));
        this.SetLocalValue(SetterSceneNode.PropertyProperty, valueNode);
      }
    }

    public object Value
    {
      get
      {
        return this.GetLocalValue(SetterSceneNode.ValueProperty);
      }
      set
      {
        this.SetLocalValue(SetterSceneNode.ValueProperty, value);
      }
    }

    public string Target
    {
      get
      {
        IProperty property = this.ProjectContext.ResolveProperty(SetterSceneNode.TargetNameProperty);
        if (property != null)
          return this.GetLocalValue((IPropertyId) property) as string;
        return string.Empty;
      }
      set
      {
        this.SetLocalValue(SetterSceneNode.TargetNameProperty, (object) value);
      }
    }

    public SceneNode TargetNode
    {
      get
      {
        return this.StoryboardContainer.ResolveTargetName(this.Target);
      }
    }

    SceneElement ISceneElementSubpart.SceneElement
    {
      get
      {
        if (string.IsNullOrEmpty(this.Target))
          return (SceneElement) this.StoryboardContainer;
        return this.TargetNode as SceneElement;
      }
    }

    public int CompareTo(object obj)
    {
      SceneNode y = obj as SceneNode;
      if (y != null)
        return SceneNode.MarkerCompare((SceneNode) this, y);
      return 1;
    }

    public class ConcreteSetterSceneNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new SetterSceneNode();
      }
    }
  }
}
