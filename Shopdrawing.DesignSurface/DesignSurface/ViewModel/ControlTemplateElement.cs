// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.ControlTemplateElement
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class ControlTemplateElement : FrameworkTemplateElement
  {
    public static readonly IPropertyId TargetTypeProperty = (IPropertyId) PlatformTypes.ControlTemplate.GetMember(MemberType.LocalProperty, "TargetType", MemberAccessTypes.Public);
    public static readonly IPropertyId ControlTemplateTriggersProperty = (IPropertyId) PlatformTypes.ControlTemplate.GetMember(MemberType.LocalProperty, "Triggers", MemberAccessTypes.Public);
    public static readonly ControlTemplateElement.ConcreteControlTemplateElementFactory Factory = new ControlTemplateElement.ConcreteControlTemplateElementFactory();

    public ITypeId ControlTemplateTargetTypeId
    {
      get
      {
        return (ITypeId) DocumentNodeUtilities.GetStyleOrTemplateTargetType(this.DocumentNode);
      }
      set
      {
        DocumentNode valueNode = (DocumentNode) null;
        if (value != null)
          valueNode = (DocumentNode) new DocumentPrimitiveNode(this.DocumentContext, PlatformTypes.Type, (IDocumentNodeValue) new DocumentNodeMemberValue((IMember) this.Platform.Metadata.ResolveType(value)));
        this.SetLocalValue(((ITargetTypeMetadata) this.Metadata).TargetTypeProperty, valueNode);
      }
    }

    protected override IProperty TriggersProperty
    {
      get
      {
        return this.ProjectContext.ResolveProperty(ControlTemplateElement.ControlTemplateTriggersProperty);
      }
    }

    public override Type TargetElementType
    {
      get
      {
        ITypeId templateTargetTypeId = this.ControlTemplateTargetTypeId;
        IType type = templateTargetTypeId == null ? (IType) null : this.ProjectContext.ResolveType(templateTargetTypeId);
        if (type == null)
          return (Type) null;
        return type.RuntimeType;
      }
    }

    public class ConcreteControlTemplateElementFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new ControlTemplateElement();
      }
    }
  }
}
