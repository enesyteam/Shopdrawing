// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.TargetedReferenceStep
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  [DebuggerDisplay("{ReferenceStep}")]
  public class TargetedReferenceStep
  {
    private ReferenceStep referenceStep;
    private AttributeCollection attributes;
    private IType targetType;

    public ReferenceStep ReferenceStep
    {
      get
      {
        return this.referenceStep;
      }
    }

    public AttributeCollection Attributes
    {
      get
      {
        if (this.attributes == null)
        {
          AttributeCollection attributeCollection = this.referenceStep.Attributes ?? new AttributeCollection(new Attribute[0]);
          List<Attribute> list = new List<Attribute>(attributeCollection.Count);
          for (; this.targetType != null && this.targetType.RuntimeType != (Type) null && !this.targetType.Equals((object) this.referenceStep.DeclaringTypeId); this.targetType = this.targetType.BaseType)
            list.AddRange(MetadataStore.GetAttributes(this.targetType.RuntimeType, this.referenceStep.Name));
          foreach (Attribute attribute in attributeCollection)
            list.Add(attribute);
          this.attributes = new AttributeCollection(TypeUtilities.CullDuplicateAttributes((IList<Attribute>) list));
        }
        return this.attributes;
      }
    }

    public TargetedReferenceStep(ReferenceStep referenceStep, IType targetType)
    {
      this.referenceStep = referenceStep;
      this.targetType = targetType;
    }
  }
}
