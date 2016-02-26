// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.AnnotationSceneNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Annotations;
using Microsoft.Expression.DesignSurface.Properties;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class AnnotationSceneNode : SceneNode
  {
    public static readonly IPropertyId ReferencesProperty = (IPropertyId) PlatformTypes.Annotation.GetMember(MemberType.AttachedProperty, "References", MemberAccessTypes.Public);
    public static readonly IPropertyId IdProperty = (IPropertyId) PlatformTypes.Annotation.GetMember(MemberType.LocalProperty, "Id", MemberAccessTypes.Public);
    public static readonly IPropertyId TextProperty = (IPropertyId) PlatformTypes.Annotation.GetMember(MemberType.LocalProperty, "Text", MemberAccessTypes.Public);
    public static readonly IPropertyId TimestampProperty = (IPropertyId) PlatformTypes.Annotation.GetMember(MemberType.LocalProperty, "Timestamp", MemberAccessTypes.Public);
    public static readonly IPropertyId TopProperty = (IPropertyId) PlatformTypes.Annotation.GetMember(MemberType.LocalProperty, "Top", MemberAccessTypes.Public);
    public static readonly IPropertyId LeftProperty = (IPropertyId) PlatformTypes.Annotation.GetMember(MemberType.LocalProperty, "Left", MemberAccessTypes.Public);
    public static readonly IPropertyId AuthorProperty = (IPropertyId) PlatformTypes.Annotation.GetMember(MemberType.LocalProperty, "Author", MemberAccessTypes.Public);
    public static readonly IPropertyId AuthorInitialsProperty = (IPropertyId) PlatformTypes.Annotation.GetMember(MemberType.LocalProperty, "AuthorInitials", MemberAccessTypes.Public);
    public static readonly IPropertyId SerialNumberProperty = (IPropertyId) PlatformTypes.Annotation.GetMember(MemberType.LocalProperty, "SerialNumber", MemberAccessTypes.Public);
    public static readonly IPropertyId VisibleAtRuntimeProperty = (IPropertyId) PlatformTypes.Annotation.GetMember(MemberType.LocalProperty, "VisibleAtRuntime", MemberAccessTypes.Public);
    public static readonly AnnotationSceneNode.ConcreteAnnotationSceneNodeFactory Factory = new AnnotationSceneNode.ConcreteAnnotationSceneNodeFactory();

    public string Id
    {
      get
      {
        return this.GetValue<string>(AnnotationSceneNode.IdProperty, AnnotationDefaults.Id);
      }
      set
      {
        this.SetValue(AnnotationSceneNode.IdProperty, (object) value);
      }
    }

    public string Text
    {
      get
      {
        return this.GetValue<string>(AnnotationSceneNode.TextProperty, AnnotationDefaults.Text);
      }
      set
      {
        this.SetValue(AnnotationSceneNode.TextProperty, (object) value);
      }
    }

    public DateTime Timestamp
    {
      get
      {
        return this.GetValue<DateTime>(AnnotationSceneNode.TimestampProperty, AnnotationDefaults.Timestamp);
      }
      set
      {
        this.SetValue(AnnotationSceneNode.TimestampProperty, (object) value);
      }
    }

    public double Top
    {
      get
      {
        return this.GetValue<double>(AnnotationSceneNode.TopProperty, AnnotationDefaults.Top);
      }
      set
      {
        this.SetValue(AnnotationSceneNode.TopProperty, (object) value);
      }
    }

    public double Left
    {
      get
      {
        return this.GetValue<double>(AnnotationSceneNode.LeftProperty, AnnotationDefaults.Left);
      }
      set
      {
        this.SetValue(AnnotationSceneNode.LeftProperty, (object) value);
      }
    }

    public string Author
    {
      get
      {
        return this.GetValue<string>(AnnotationSceneNode.AuthorProperty, AnnotationDefaults.Author);
      }
      set
      {
        this.SetValue(AnnotationSceneNode.AuthorProperty, (object) value);
      }
    }

    public string AuthorInitials
    {
      get
      {
        return this.GetValue<string>(AnnotationSceneNode.AuthorInitialsProperty, AnnotationDefaults.AuthorInitials);
      }
      set
      {
        this.SetValue(AnnotationSceneNode.AuthorInitialsProperty, (object) value);
      }
    }

    public int SerialNumber
    {
      get
      {
        return this.GetValue<int>(AnnotationSceneNode.SerialNumberProperty, AnnotationDefaults.SerialNumber);
      }
      set
      {
        this.SetValue(AnnotationSceneNode.SerialNumberProperty, (object) value);
      }
    }

    public bool VisibleAtRuntime
    {
      get
      {
        return this.GetValue<bool>(AnnotationSceneNode.VisibleAtRuntimeProperty, AnnotationDefaults.VisibleAtRuntime);
      }
      set
      {
        this.SetValue(AnnotationSceneNode.VisibleAtRuntimeProperty, (object) (bool) (value ? true : false));
      }
    }

    public Point Position
    {
      get
      {
        return new Point()
        {
          X = double.IsNaN(this.Left) ? 0.0 : this.Left,
          Y = double.IsNaN(this.Top) ? 0.0 : this.Top
        };
      }
      set
      {
        this.Left = value.X;
        this.Top = value.Y;
      }
    }

    public IEnumerable<SceneElement> AttachedElements
    {
      get
      {
        return this.ViewModel.AnnotationEditor.GetAttachedElements(this);
      }
    }

    internal AnnotationVisual Visual
    {
      get
      {
        return this.ViewModel.AnnotationEditor.GetAnnotationVisual(this);
      }
    }

    private T GetValue<T>(IPropertyId property, T defaultValue)
    {
      if (this.ViewModel.DocumentRoot != null && this.IsSet(property) == PropertyState.Set)
        return (T) this.GetLocalValue(property);
      return defaultValue;
    }

    public class ConcreteAnnotationSceneNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new AnnotationSceneNode();
      }

      internal AnnotationSceneNode Instantiate(SceneViewModel sceneViewModel)
      {
        return (AnnotationSceneNode) this.Instantiate(sceneViewModel, PlatformTypes.Annotation);
      }
    }
  }
}
