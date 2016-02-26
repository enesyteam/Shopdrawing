// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.DictionaryEntryNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Properties;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public sealed class DictionaryEntryNode : SceneNode
  {
    public static readonly IPropertyId KeyProperty = (IPropertyId) PlatformTypes.DictionaryEntry.GetMember(MemberType.LocalProperty, "Key", MemberAccessTypes.Public);
    public static readonly IPropertyId ValueProperty = (IPropertyId) PlatformTypes.DictionaryEntry.GetMember(MemberType.LocalProperty, "Value", MemberAccessTypes.Public);
    public static readonly DictionaryEntryNode.ConcreteDictionaryEntryNodeFactory Factory = new DictionaryEntryNode.ConcreteDictionaryEntryNodeFactory();

    public object Key
    {
      get
      {
        object obj = this.GetLocalOrDefaultValue(DictionaryEntryNode.KeyProperty);
        if (obj == null && this.ProjectContext.IsCapabilitySet(PlatformCapability.NameSupportedAsKey))
        {
          SceneNode sceneNode = this.Value;
          if (sceneNode != null && sceneNode.NameProperty != null && sceneNode.DocumentNode is DocumentCompositeNode)
            obj = (object) sceneNode.Name;
        }
        return obj;
      }
      set
      {
        IProjectContext projectContext = this.ProjectContext;
        string str = value as string;
        if (projectContext.IsCapabilitySet(PlatformCapability.NameSupportedAsKey) && (value == null || str != null) && this.IsSet(DictionaryEntryNode.KeyProperty) == PropertyState.Unset)
        {
          SceneNode sceneNode = this.Value;
          if (sceneNode != null && sceneNode.NameProperty != null && sceneNode.IsSet(sceneNode.NameProperty) == PropertyState.Set)
          {
            sceneNode.Name = str;
            return;
          }
        }
        this.SetLocalValue(DictionaryEntryNode.KeyProperty, value);
      }
    }

    public SceneNode KeyNode
    {
      get
      {
        SceneNode valueAsSceneNode = this.GetLocalValueAsSceneNode(DictionaryEntryNode.KeyProperty);
        if (valueAsSceneNode == null && this.ProjectContext.IsCapabilitySet(PlatformCapability.NameSupportedAsKey))
        {
          SceneNode sceneNode = this.Value;
          if (sceneNode != null && sceneNode.NameProperty != null && sceneNode.DocumentNode is DocumentCompositeNode)
            valueAsSceneNode = sceneNode.GetLocalValueAsSceneNode(sceneNode.NameProperty);
        }
        return valueAsSceneNode;
      }
      set
      {
        if (this.ProjectContext.IsCapabilitySet(PlatformCapability.NameSupportedAsKey) && (value == null || PlatformTypes.String.IsAssignableFrom((ITypeId) value.Type)) && this.IsSet(DictionaryEntryNode.KeyProperty) == PropertyState.Unset)
        {
          SceneNode sceneNode = this.Value;
          if (sceneNode != null)
          {
            IPropertyId nameProperty = sceneNode.NameProperty;
            if (nameProperty != null && sceneNode.IsSet(sceneNode.NameProperty) == PropertyState.Set)
            {
              sceneNode.SetValueAsSceneNode(nameProperty, value);
              return;
            }
          }
        }
        this.SetValueAsSceneNode(DictionaryEntryNode.KeyProperty, value);
      }
    }

    public SceneNode Value
    {
      get
      {
        return this.GetLocalValueAsSceneNode(DictionaryEntryNode.ValueProperty);
      }
      set
      {
        this.SetValueAsSceneNode(DictionaryEntryNode.ValueProperty, value);
      }
    }

    public class ConcreteDictionaryEntryNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new DictionaryEntryNode();
      }

      public DictionaryEntryNode Instantiate(object key, SceneNode value)
      {
        DictionaryEntryNode dictionaryEntryNode = (DictionaryEntryNode) this.Instantiate(value.ViewModel, PlatformTypes.DictionaryEntry);
        dictionaryEntryNode.Key = key;
        dictionaryEntryNode.Value = value;
        return dictionaryEntryNode;
      }
    }
  }
}
