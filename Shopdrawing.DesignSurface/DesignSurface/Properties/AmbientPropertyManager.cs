// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Properties.AmbientPropertyManager
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.Extensions.Enumerable;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.Properties
{
  internal class AmbientPropertyManager : IAmbientPropertyManager, IDisposable
  {
    private static readonly Brush DefaultFillBrush = (Brush) new SolidColorBrush(Color.FromArgb(byte.MaxValue, (byte) 244, (byte) 244, (byte) 245));
    private static List<ITypeId> SupportedTypes = new List<ITypeId>()
    {
      PlatformTypes.Shape,
      ProjectNeutralTypes.PrimitiveShape,
      ProjectNeutralTypes.CompositeShape,
      ProjectNeutralTypes.CompositeContentShape
    };
    private Dictionary<string, AmbientPropertyManager.AmbientPropertyValue> ambientPropertyValues = new Dictionary<string, AmbientPropertyManager.AmbientPropertyValue>()
    {
      {
        "Stroke",
        new AmbientPropertyManager.AmbientPropertyValue((object) Brushes.Black, new IPropertyId[3]
        {
          ShapeElement.StrokeProperty,
          CompositeShapeElement.StrokeProperty,
          CompositeContentShapeElement.StrokeProperty
        })
      },
      {
        "Fill",
        new AmbientPropertyManager.AmbientPropertyValue((object) AmbientPropertyManager.DefaultFillBrush, new IPropertyId[3]
        {
          ShapeElement.FillProperty,
          CompositeShapeElement.FillProperty,
          CompositeContentShapeElement.FillProperty
        })
      },
      {
        "RadiusX",
        new AmbientPropertyManager.AmbientPropertyValue(DependencyProperty.UnsetValue, new IPropertyId[1]
        {
          RectangleElement.RadiusXProperty
        })
      },
      {
        "RadiusY",
        new AmbientPropertyManager.AmbientPropertyValue(DependencyProperty.UnsetValue, new IPropertyId[1]
        {
          RectangleElement.RadiusYProperty
        })
      },
      {
        "StrokeThickness",
        new AmbientPropertyManager.AmbientPropertyValue(DependencyProperty.UnsetValue, new IPropertyId[3]
        {
          ShapeElement.StrokeThicknessProperty,
          CompositeShapeElement.StrokeThicknessProperty,
          CompositeContentShapeElement.StrokeThicknessProperty
        })
      },
      {
        "StrokeMiterLimit",
        new AmbientPropertyManager.AmbientPropertyValue(DependencyProperty.UnsetValue, new IPropertyId[3]
        {
          ShapeElement.StrokeMiterLimitProperty,
          CompositeShapeElement.StrokeMiterLimitProperty,
          CompositeContentShapeElement.StrokeMiterLimitProperty
        })
      },
      {
        "StrokeEndLineCap",
        new AmbientPropertyManager.AmbientPropertyValue(DependencyProperty.UnsetValue, new IPropertyId[3]
        {
          ShapeElement.StrokeEndLineCapProperty,
          CompositeShapeElement.StrokeEndLineCapProperty,
          CompositeContentShapeElement.StrokeEndLineCapProperty
        })
      },
      {
        "StrokeStartLineCap",
        new AmbientPropertyManager.AmbientPropertyValue(DependencyProperty.UnsetValue, new IPropertyId[3]
        {
          ShapeElement.StrokeStartLineCapProperty,
          CompositeShapeElement.StrokeStartLineCapProperty,
          CompositeContentShapeElement.StrokeStartLineCapProperty
        })
      },
      {
        "StrokeLineJoin",
        new AmbientPropertyManager.AmbientPropertyValue(DependencyProperty.UnsetValue, new IPropertyId[3]
        {
          ShapeElement.StrokeLineJoinProperty,
          CompositeShapeElement.StrokeLineJoinProperty,
          CompositeContentShapeElement.StrokeLineJoinProperty
        })
      }
    };
    private DesignerContext designerContext;
    private IProjectContext activeProjectContext;
    private int suppressApplyCount;

    private IProjectContext ActiveProjectContext
    {
      get
      {
        return this.activeProjectContext;
      }
      set
      {
        if (this.activeProjectContext == value)
          return;
        if (this.activeProjectContext != null)
        {
          INotifyCollectionChanged collectionChanged = this.activeProjectContext.AssemblyReferences as INotifyCollectionChanged;
          if (collectionChanged != null)
            collectionChanged.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.AssemblyReferences_CollectionChanged);
          EnumerableExtensions.ForEach<AmbientPropertyManager.AmbientPropertyValue>((IEnumerable<AmbientPropertyManager.AmbientPropertyValue>) this.ambientPropertyValues.Values, (Action<AmbientPropertyManager.AmbientPropertyValue>) (propertyValue => propertyValue.Clear(true)));
        }
        this.activeProjectContext = value;
        if (this.activeProjectContext == null)
          return;
        INotifyCollectionChanged collectionChanged1 = this.activeProjectContext.AssemblyReferences as INotifyCollectionChanged;
        if (collectionChanged1 != null)
          collectionChanged1.CollectionChanged += new NotifyCollectionChangedEventHandler(this.AssemblyReferences_CollectionChanged);
        this.ResolveCachedAmbientProperties();
      }
    }

    public AmbientPropertyManager(DesignerContext designerContext)
    {
      this.designerContext = designerContext;
      this.designerContext.ViewService.ActiveViewChanged += new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
      this.designerContext.DocumentService.DocumentClosed += new DocumentEventHandler(this.DocumentService_DocumentClosed);
    }

    public void Dispose()
    {
      this.designerContext.DocumentService.DocumentClosed -= new DocumentEventHandler(this.DocumentService_DocumentClosed);
      this.designerContext.ViewService.ActiveViewChanged -= new ViewChangedEventHandler(this.ViewService_ActiveViewChanged);
      this.designerContext = (DesignerContext) null;
    }

    private void DocumentService_DocumentClosed(object sender, DocumentEventArgs e)
    {
      EnumerableExtensions.ForEach<AmbientPropertyManager.AmbientPropertyValue>((IEnumerable<AmbientPropertyManager.AmbientPropertyValue>) this.ambientPropertyValues.Values, (Action<AmbientPropertyManager.AmbientPropertyValue>) (propertyValue => propertyValue.ClearValueOnDocumentClosed(e.Document as SceneDocument)));
    }

    private void ViewService_ActiveViewChanged(object sender, ViewChangedEventArgs e)
    {
      this.ActiveProjectContext = this.designerContext.ActiveProjectContext;
    }

    private void ResolveCachedAmbientProperties()
    {
      EnumerableExtensions.ForEach<AmbientPropertyManager.AmbientPropertyValue>((IEnumerable<AmbientPropertyManager.AmbientPropertyValue>) this.ambientPropertyValues.Values, (Action<AmbientPropertyManager.AmbientPropertyValue>) (propertyValue => propertyValue.ResolveProperties(this.ActiveProjectContext, this.designerContext.PropertyManager, new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.AmbientPropertyReferenceChanged))));
    }

    private void AssemblyReferences_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      this.ResolveCachedAmbientProperties();
    }

    public bool IsAmbientProperty(PropertyReference propertyReference)
    {
      ReferenceStep firstStep = propertyReference.FirstStep;
      if (firstStep != null)
        return this.GetAmbientPropertyValue(firstStep) != null;
      return false;
    }

    public object GetAmbientValue(PropertyReference propertyReference)
    {
      ReferenceStep firstStep = propertyReference.FirstStep;
      if (firstStep == null)
        return DependencyProperty.UnsetValue;
      object ambientValue = this.InternalGetAmbientValue(firstStep, (SceneNode) null);
      if (propertyReference.Count < 2)
        return ambientValue;
      return propertyReference.Subreference(1).GetValue(ambientValue);
    }

    private IEnumerable<ReferenceStep> GetResolvedAmbientProperties()
    {
      foreach (AmbientPropertyManager.AmbientPropertyValue ambientPropertyValue in this.ambientPropertyValues.Values)
      {
        foreach (ReferenceStep referenceStep in (IEnumerable<ReferenceStep>) ambientPropertyValue.ResolvedProperties)
          yield return referenceStep;
      }
    }

    public IDisposable SuppressApplyAmbientProperties()
    {
      return (IDisposable) new AmbientPropertyManager.SuppressApplyToken(this);
    }

    public void ApplyAmbientProperties(SceneNode node)
    {
      if (node == null || !Enumerable.Any<ITypeId>((IEnumerable<ITypeId>) AmbientPropertyManager.SupportedTypes, (Func<ITypeId, bool>) (type => type.IsAssignableFrom((ITypeId) node.Type))) || this.suppressApplyCount > 0)
        return;
      foreach (ReferenceStep referenceStep in this.GetResolvedAmbientProperties())
      {
        bool strictTypeCheck = true;
        PropertyReference propertyReference = SceneNodeObjectSet.FilterProperty(node, new PropertyReference(referenceStep), strictTypeCheck);
        if (propertyReference != null)
        {
          object valueToSet = this.InternalGetAmbientValue(referenceStep, node);
          DocumentNode documentNode1 = valueToSet as DocumentNode;
          if (documentNode1 != null)
          {
            DocumentNode documentNode2 = documentNode1.Clone(node.DocumentContext);
            DocumentCompositeNode documentCompositeNode = documentNode2 as DocumentCompositeNode;
            if (documentCompositeNode != null)
              documentCompositeNode.ClearValue((IPropertyId) documentCompositeNode.Type.Metadata.NameProperty);
            foreach (DocumentNode documentNode3 in documentNode2.SelectDescendantNodes((Predicate<DocumentNode>) (nodePredicate => nodePredicate.Parent.IsNameProperty((IPropertyId) nodePredicate.SitePropertyKey))))
              documentNode3.Parent.ClearValue((IPropertyId) documentNode3.SitePropertyKey);
            valueToSet = (object) documentNode2;
          }
          if (valueToSet == DependencyProperty.UnsetValue)
            node.ClearValueAsWpf(propertyReference);
          else
            node.SetValueAsWpf(propertyReference, valueToSet);
        }
      }
    }

    private object InternalGetAmbientValue(ReferenceStep property, SceneNode targetNode)
    {
      AmbientPropertyManager.AmbientPropertyValue ambientPropertyValue = this.GetAmbientPropertyValue(property);
      if (ambientPropertyValue != null)
      {
        if (ambientPropertyValue.Value == DependencyProperty.UnsetValue)
          return ambientPropertyValue.DefaultValue;
        DocumentNode node = ambientPropertyValue.Value as DocumentNode;
        if (node == null)
          return DependencyProperty.UnsetValue;
        if (targetNode == null && (DocumentNodeUtilities.IsStaticResource(node) || DocumentNodeUtilities.IsDynamicResource(node) || DocumentNodeUtilities.IsTemplateBinding(node)))
          return ambientPropertyValue.DefaultValue;
        DocumentCompositeNode documentCompositeNode = node as DocumentCompositeNode;
        if (DocumentNodeUtilities.IsStaticResource(node) || DocumentNodeUtilities.IsDynamicResource(node))
        {
          DocumentNode resourceKey = ResourceNodeHelper.GetResourceKey(documentCompositeNode);
          bool flag = new ExpressionEvaluator((IDocumentRootResolver) targetNode.ProjectContext).EvaluateResource(targetNode.DocumentNodePath, DocumentNodeUtilities.IsStaticResource(node) ? ResourceReferenceType.Static : ResourceReferenceType.Dynamic, resourceKey) != null;
          if (!flag)
          {
            object instance = targetNode.ViewModel.CreateInstance(new DocumentNodePath(resourceKey, resourceKey));
            if (instance != null)
              flag = targetNode.ViewModel.FindResource(instance) != null;
          }
          if (!flag)
            return ambientPropertyValue.DefaultValue;
        }
        else if (DocumentNodeUtilities.IsTemplateBinding(node))
        {
          ExpressionEvaluator expressionEvaluator = new ExpressionEvaluator((IDocumentRootResolver) targetNode.ProjectContext);
          if (documentCompositeNode == null || expressionEvaluator.EvaluateTemplateBinding(targetNode.DocumentNodePath, documentCompositeNode) == null)
            return ambientPropertyValue.DefaultValue;
        }
        return (object) node;
      }
      if (targetNode == null || !object.Equals(ambientPropertyValue.DefaultValue, targetNode.GetDefaultValueAsWpf((IPropertyId) property)))
        return ambientPropertyValue.DefaultValue;
      return DependencyProperty.UnsetValue;
    }

    private AmbientPropertyManager.AmbientPropertyValue GetAmbientPropertyValue(ReferenceStep property)
    {
      AmbientPropertyManager.AmbientPropertyValue ambientPropertyValue;
      if (this.ambientPropertyValues.TryGetValue(property.Name, out ambientPropertyValue) && ambientPropertyValue.ResolvedProperties.Contains(property))
        return ambientPropertyValue;
      return (AmbientPropertyManager.AmbientPropertyValue) null;
    }

    private void UpdateAmbientValueFromSceneElement(SceneElement element, PropertyReference propertyReference)
    {
      bool strictTypeCheck = true;
      PropertyReference propertyReference1 = SceneNodeObjectSet.FilterProperty((SceneNode) element, propertyReference, strictTypeCheck);
      if (!element.IsViewObjectValid || propertyReference1 == null || propertyReference1.PlatformMetadata != element.Platform.Metadata)
        return;
      AmbientPropertyManager.AmbientPropertyValue ambientPropertyValue = this.GetAmbientPropertyValue(propertyReference1.FirstStep);
      if (ambientPropertyValue == null)
        return;
      using (element.ViewModel.ForceBaseValue())
      {
        if (element.IsSet(propertyReference1) == PropertyState.Unset)
        {
          ambientPropertyValue.Value = (object) null;
        }
        else
        {
          DocumentNodePath valueAsDocumentNode = element.GetLocalValueAsDocumentNode(propertyReference1);
          if (!this.HasValidValue(element.ViewModel, valueAsDocumentNode))
            return;
          ambientPropertyValue.Value = (object) valueAsDocumentNode.Node;
        }
      }
    }

    private bool HasValidValue(SceneViewModel viewModel, DocumentNodePath documentNodePath)
    {
      if (documentNodePath != null && !DocumentNodeUtilities.IsBinding(documentNodePath.Node))
      {
        SceneView activeView = viewModel.DesignerContext.ActiveView;
        if (activeView != null)
        {
          IInstanceBuilderContext instanceBuilderContext = activeView.InstanceBuilderContext;
          IExceptionDictionary exceptionDictionary = instanceBuilderContext.ExceptionDictionary;
          ViewNode viewNode;
          if (instanceBuilderContext.ViewNodeManager.TryGetCorrespondingViewNode(documentNodePath, out viewNode))
            return !exceptionDictionary.Contains(viewNode);
          return true;
        }
      }
      return false;
    }

    private void AmbientPropertyReferenceChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      SceneElementSelectionSet elementSelectionSet = this.designerContext.SelectionManager.ElementSelectionSet;
      if (elementSelectionSet == null || elementSelectionSet.PrimarySelection == null)
        return;
      this.UpdateAmbientValueFromSceneElement(elementSelectionSet.PrimarySelection, new PropertyReference(e.PropertyReference[0]));
    }

    private class SuppressApplyToken : IDisposable
    {
      private AmbientPropertyManager manager;

      public SuppressApplyToken(AmbientPropertyManager manager)
      {
        this.manager = manager;
        ++manager.suppressApplyCount;
      }

      public void Dispose()
      {
        --this.manager.suppressApplyCount;
        GC.SuppressFinalize((object) this);
      }
    }

    private class AmbientPropertyValue
    {
      private object value = DependencyProperty.UnsetValue;
      private HashSet<ReferenceStep> resolvedProperties = new HashSet<ReferenceStep>();
      private IPropertyManager propertyManager;
      private Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler referenceChangedCallback;

      public object DefaultValue { get; private set; }

      public IEnumerable<IPropertyId> NeutralProperties { get; private set; }

      public object Value
      {
        get
        {
          return this.value;
        }
        set
        {
          this.value = value;
        }
      }

      public ICollection<ReferenceStep> ResolvedProperties
      {
        get
        {
          return (ICollection<ReferenceStep>) this.resolvedProperties;
        }
      }

      public AmbientPropertyValue(object defaultValue, params IPropertyId[] supportedProperties)
      {
        this.DefaultValue = defaultValue;
        this.NeutralProperties = (IEnumerable<IPropertyId>) supportedProperties;
        this.Value = DependencyProperty.UnsetValue;
      }

      public void Clear(bool clearValue)
      {
        if (this.ResolvedProperties != null)
        {
          foreach (ReferenceStep singleStep in (IEnumerable<ReferenceStep>) this.ResolvedProperties)
            this.propertyManager.UnregisterPropertyReferenceChangedHandler(new PropertyReference(singleStep), this.referenceChangedCallback);
        }
        this.resolvedProperties = (HashSet<ReferenceStep>) null;
        if (!clearValue)
          return;
        this.Value = DependencyProperty.UnsetValue;
      }

      public void ResolveProperties(IProjectContext projectContext, IPropertyManager propertyManager, Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler referenceChangedCallback)
      {
        this.Clear(false);
        this.resolvedProperties = new HashSet<ReferenceStep>();
        this.propertyManager = propertyManager;
        this.referenceChangedCallback = referenceChangedCallback;
        foreach (IPropertyId propertyId in this.NeutralProperties)
        {
          ReferenceStep singleStep = projectContext.ResolveProperty(propertyId) as ReferenceStep;
          if (singleStep != null)
          {
            this.resolvedProperties.Add(singleStep);
            propertyManager.RegisterPropertyReferenceChangedHandler(new PropertyReference(singleStep), referenceChangedCallback, true);
          }
        }
      }

      internal void ClearValueOnDocumentClosed(SceneDocument closedDocument)
      {
        if (closedDocument == null)
          return;
        DocumentNode documentNode = this.value as DocumentNode;
        if (documentNode == null || documentNode.DocumentRoot != closedDocument.XamlDocument)
          return;
        this.value = DependencyProperty.UnsetValue;
      }
    }
  }
}
