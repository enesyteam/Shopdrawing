// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.LightCategory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class LightCategory : SceneNodeCategory
  {
    private List<LightElement> selectedLights = new List<LightElement>();
    private object lightType;

    public bool IsAmbientLight
    {
      get
      {
        return typeof (AmbientLight).Equals(this.lightType);
      }
      set
      {
        if (!value)
          return;
        this.SetLightTypeOnSelection(typeof (AmbientLight));
      }
    }

    public bool IsDirectionalLight
    {
      get
      {
        return typeof (DirectionalLight).Equals(this.lightType);
      }
      set
      {
        if (!value)
          return;
        this.SetLightTypeOnSelection(typeof (DirectionalLight));
      }
    }

    public bool IsPointLight
    {
      get
      {
        return typeof (PointLight).Equals(this.lightType);
      }
      set
      {
        if (!value)
          return;
        this.SetLightTypeOnSelection(typeof (PointLight));
      }
    }

    public bool IsSpotLight
    {
      get
      {
        return typeof (SpotLight).Equals(this.lightType);
      }
      set
      {
        if (!value)
          return;
        this.SetLightTypeOnSelection(typeof (SpotLight));
      }
    }

    public LightCategory(string localizedName, IMessageLoggingService messageLogger)
      : base(CategoryLocalizationHelper.CategoryName.Light, localizedName, messageLogger)
    {
    }

    public override void OnSelectionChanged(SceneNode[] selectedObjects)
    {
      base.OnSelectionChanged(selectedObjects);
      this.selectedLights.Clear();
      this.lightType = null;
      foreach (SceneNode sceneNode in selectedObjects)
      {
        LightElement lightElement = sceneNode as LightElement;
        if (lightElement != null)
        {
          this.selectedLights.Add(lightElement);
          if (this.lightType == null)
            this.lightType = (object) lightElement.TargetType;
          else if (!object.Equals(this.lightType, (object) lightElement.TargetType))
            this.lightType = MixedProperty.Mixed;
        }
      }
      this.OnPropertyChanged("IsAmbientLight");
      this.OnPropertyChanged("IsPointLight");
      this.OnPropertyChanged("IsDirectionalLight");
      this.OnPropertyChanged("IsSpotLight");
    }

    private void SetLightTypeOnSelection(Type lightType)
    {
      if (this.selectedLights.Count == 0 || this.lightType != null && this.lightType.Equals((object) lightType))
        return;
      this.lightType = (object) lightType;
      SceneViewModel viewModel = this.selectedLights[0].ViewModel;
      SceneElementSelectionSet elementSelectionSet = viewModel.ElementSelectionSet;
      List<KeyValuePair<LightElement, LightElement>> list = new List<KeyValuePair<LightElement, LightElement>>();
      foreach (LightElement key in (IEnumerable<LightElement>) elementSelectionSet.GetFilteredSelection<LightElement>())
      {
        if (!(key.TargetType == lightType))
        {
          LightElement lightElement = (LightElement) key.ViewModel.CreateSceneNode(lightType);
          DocumentNode documentNode = (DocumentNode) null;
          if (typeof (SpotLight).Equals(lightType))
          {
            DocumentNodePath valueAsDocumentNode = key.GetLocalValueAsDocumentNode(SpotLightElement.DirectionProperty);
            documentNode = valueAsDocumentNode != null ? valueAsDocumentNode.Node : (DocumentNode) null;
          }
          else if (typeof (DirectionalLight).Equals(lightType))
          {
            DocumentNodePath valueAsDocumentNode = key.GetLocalValueAsDocumentNode(DirectionalLightElement.DirectionProperty);
            documentNode = valueAsDocumentNode != null ? valueAsDocumentNode.Node : (DocumentNode) null;
          }
          if (typeof (DirectionalLight).Equals(lightType))
          {
            if (documentNode != null)
              lightElement.SetValue(DirectionalLightElement.DirectionProperty, (object) documentNode);
          }
          else if (typeof (PointLightBase).IsAssignableFrom(lightType))
          {
            if (!typeof (PointLightBase).IsAssignableFrom(key.TargetType))
            {
              lightElement.SetValue(LightElement.PositionProperty, (object) new Point3D(0.0, 0.0, 0.0));
              lightElement.SetValue(LightElement.RangeProperty, (object) 10.0);
              lightElement.SetValue(LightElement.ConstantAttenuationProperty, (object) 1.0);
              lightElement.SetValue(LightElement.LinearAttenuationProperty, (object) 0.0);
              lightElement.SetValue(LightElement.QuadraticAttenuationProperty, (object) 0.0);
            }
            if (typeof (SpotLight).Equals(lightType))
            {
              if (documentNode != null)
                lightElement.SetValue(SpotLightElement.DirectionProperty, (object) documentNode);
              lightElement.SetValue(SpotLightElement.OuterConeAngleProperty, (object) 40.0);
              lightElement.SetValue(SpotLightElement.InnerConeAngleProperty, (object) 30.0);
            }
          }
          list.Add(new KeyValuePair<LightElement, LightElement>(key, lightElement));
        }
      }
      if (list.Count != 0)
      {
        using (SceneEditTransaction editTransaction = viewModel.Document.CreateEditTransaction(StringTable.UndoUnitChangeLightType))
        {
          foreach (KeyValuePair<LightElement, LightElement> keyValuePair in list)
          {
            LightElement key = keyValuePair.Key;
            LightElement lightElement = keyValuePair.Value;
            viewModel.AnimationEditor.DeleteAllAnimations((SceneNode) key);
            Dictionary<IPropertyId, SceneNode> properties = SceneElementHelper.StoreProperties((SceneNode) key);
            elementSelectionSet.RemoveSelection((SceneElement) key);
            SceneElement parentElement = key.ParentElement;
            ISceneNodeCollection<SceneNode> collectionForProperty = parentElement.GetCollectionForProperty((IPropertyId) parentElement.GetPropertyForChild((SceneNode) key));
            int index = collectionForProperty.IndexOf((SceneNode) key);
            collectionForProperty[index] = (SceneNode) lightElement;
            SceneElementHelper.ApplyProperties((SceneNode) lightElement, properties);
            elementSelectionSet.ExtendSelection((SceneElement) lightElement);
          }
          editTransaction.Commit();
        }
      }
      this.OnPropertyChanged("IsAmbientLight");
      this.OnPropertyChanged("IsPointLight");
      this.OnPropertyChanged("IsDirectionalLight");
      this.OnPropertyChanged("IsSpotLight");
    }
  }
}
