// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.BaseTriggerNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public abstract class BaseTriggerNode : TriggerBaseNode
  {
    private const string StyleRootID = "~Self";

    public abstract ISceneNodeCollection<SceneNode> Setters { get; }

    public abstract override string PresentationName { get; }

    public DocumentNode GetDocumentNodeValue(DocumentCompositeNode compositeNode, IPropertyId propertyKey, bool visualTriggerOnly)
    {
      DocumentNode documentNode = (DocumentNode) null;
      string id = this.GetID(compositeNode);
      if (id != null)
      {
        ISceneNodeCollection<SceneNode> setters = this.Setters;
        for (int index = setters.Count - 1; index >= 0; --index)
        {
          DocumentCompositeNode setterNode = (DocumentCompositeNode) setters[index].DocumentNode;
          if (this.IsSetter(setterNode, id, propertyKey))
          {
            documentNode = setterNode.Properties[SetterSceneNode.ValueProperty];
            break;
          }
        }
        if (visualTriggerOnly)
          return documentNode;
      }
      if (documentNode == null)
        documentNode = compositeNode.Properties[propertyKey];
      return documentNode;
    }

    public void SetDocumentNodeValue(DocumentCompositeNode compositeNode, IPropertyId propertyKey, DocumentNode valueNode)
    {
      string id = this.GetID(compositeNode);
      if (id != null)
      {
        ISceneNodeCollection<SceneNode> setters = this.Setters;
        int index = 0;
        while (index < setters.Count)
        {
          if (this.IsSetter((DocumentCompositeNode) setters[index].DocumentNode, id, propertyKey))
            setters.RemoveAt(index);
          else
            ++index;
        }
        if (valueNode == null)
          return;
        SetterSceneNode setterSceneNode = (SetterSceneNode) this.ViewModel.CreateSceneNode(typeof (Setter));
        if (!this.IsRootNode((DocumentNode) compositeNode))
          setterSceneNode.Target = id;
        setterSceneNode.Property = (DependencyPropertyReferenceStep) propertyKey;
        ((DocumentCompositeNode) setterSceneNode.DocumentNode).Properties[SetterSceneNode.ValueProperty] = valueNode;
        setters.Add((SceneNode) setterSceneNode);
      }
      else
        compositeNode.Properties[propertyKey] = valueNode;
    }

    public bool ShouldRecordPropertyChange(SceneNode node, IPropertyId propertyKey)
    {
      if (this.ViewModel.IsForcingBaseValue || !this.ViewModel.AnimationEditor.IsRecording && !this.IsSettingValue(node, propertyKey))
        return false;
      return propertyKey is DependencyPropertyReferenceStep;
    }

    private bool IsRootNode(DocumentNode containerNode)
    {
      if (this.StoryboardContainer == null)
        return false;
      if (containerNode == ((SceneNode) this.StoryboardContainer).DocumentNode)
        return true;
      if (this.StoryboardContainer.TargetElement != null)
        return containerNode == this.StoryboardContainer.TargetElement.DocumentNode;
      return false;
    }

    private string GetID(DocumentCompositeNode compositeNode)
    {
      string str;
      if (this.IsRootNode((DocumentNode) compositeNode))
      {
        str = "~Self";
      }
      else
      {
        IPropertyId index = (IPropertyId) compositeNode.Type.Metadata.NameProperty;
        str = DocumentPrimitiveNode.GetValueAsString(compositeNode.Properties[index]);
      }
      return str;
    }

    private string GetTargetName(DocumentCompositeNode setterNode)
    {
      return DocumentPrimitiveNode.GetValueAsString(setterNode.Properties[SetterSceneNode.TargetNameProperty]) ?? "~Self";
    }

    private bool IsSetter(DocumentCompositeNode setterNode, string id, IPropertyId propertyKey)
    {
      bool flag = false;
      if (propertyKey is DependencyPropertyReferenceStep)
      {
        string targetName = this.GetTargetName(setterNode);
        if (id == targetName)
        {
          DocumentPrimitiveNode documentPrimitiveNode = setterNode.Properties[SetterSceneNode.PropertyProperty] as DocumentPrimitiveNode;
          if (documentPrimitiveNode == null)
            throw new InvalidDataException(ExceptionStringTable.VisualTriggerSetterDoesNotContainPropertyPath);
          IMemberId memberId = (IMemberId) DocumentPrimitiveNode.GetValueAsMember((DocumentNode) documentPrimitiveNode);
          if (memberId != null && memberId == propertyKey)
            flag = true;
        }
      }
      return flag;
    }

    private bool IsSettingValue(SceneNode node, IPropertyId propertyKey)
    {
      foreach (SetterSceneNode setterSceneNode in (IEnumerable<SceneNode>) this.Setters)
      {
        if (setterSceneNode.Property == propertyKey && setterSceneNode.TargetNode == node)
          return true;
      }
      return false;
    }
  }
}
