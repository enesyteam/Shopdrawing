// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.TriggerSetterNodeReference
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class TriggerSetterNodeReference : DocumentPropertyNodeReferenceBase
  {
    private BaseTriggerNode trigger;
    private DocumentCompositeNode parent;
    private IPropertyId propertyKey;
    private bool visualTriggerOnly;

    public BaseTriggerNode Trigger
    {
      get
      {
        return this.trigger;
      }
    }

    public override DocumentCompositeNode Parent
    {
      get
      {
        return this.parent;
      }
    }

    public override IPropertyId PropertyKey
    {
      get
      {
        return this.propertyKey;
      }
    }

    public override DocumentNode Node
    {
      get
      {
        return this.trigger.GetDocumentNodeValue(this.parent, this.propertyKey, this.visualTriggerOnly);
      }
      set
      {
        this.trigger.SetDocumentNodeValue(this.parent, this.propertyKey, value);
      }
    }

    public TriggerSetterNodeReference(BaseTriggerNode trigger, DocumentCompositeNode parent, IPropertyId propertyKey, bool visualTriggerOnly)
    {
      this.trigger = trigger;
      this.parent = parent;
      this.propertyKey = propertyKey;
      this.visualTriggerOnly = visualTriggerOnly;
    }

    public override bool Equals(object obj)
    {
      TriggerSetterNodeReference setterNodeReference = obj as TriggerSetterNodeReference;
      if (setterNodeReference != null && this.trigger == setterNodeReference.trigger && this.parent.Equals((DocumentNode) setterNodeReference.parent))
        return this.propertyKey.Equals((object) setterNodeReference.propertyKey);
      return false;
    }

    public override int GetHashCode()
    {
      return this.parent.GetHashCode() ^ this.propertyKey.GetHashCode();
    }

    public override string ToString()
    {
      return this.parent.ToString() + "." + this.propertyKey.Name;
    }
  }
}
