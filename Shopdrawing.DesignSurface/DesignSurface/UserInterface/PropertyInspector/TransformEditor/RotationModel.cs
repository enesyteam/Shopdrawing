// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor.RotationModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System.ComponentModel;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor
{
  public class RotationModel : INotifyPropertyChanged
  {
    private ObjectSetBase objectSet;
    private PropertyReference propertyReference;
    private PropertyReferenceProperty rotationAnglesProperty;
    private PropertyReferenceProperty rotationProperty;
    private PropertyReferenceProperty transformGroupRotationProperty;
    private PropertyReference rotationAnglesReference;
    private PropertyReference rotationReference;

    public PropertyReference PropertyReference
    {
      get
      {
        return this.propertyReference;
      }
      set
      {
        this.propertyReference = value;
        this.UpdateProperties();
      }
    }

    public ObjectSetBase ObjectSet
    {
      get
      {
        return this.objectSet;
      }
      set
      {
        if (this.objectSet == value)
          return;
        this.objectSet = value;
        this.UpdateProperties();
      }
    }

    public object EulerX
    {
      get
      {
        Vector3D? ninchedRotationAngles = this.NinchedRotationAngles;
        if (ninchedRotationAngles.HasValue)
          return (object) ninchedRotationAngles.Value.X;
        return MixedProperty.Mixed;
      }
      set
      {
        Vector3D anglesFromPrimary = this.RotationAnglesFromPrimary;
        anglesFromPrimary.X = (double) value;
        this.NinchedRotationAngles = new Vector3D?(anglesFromPrimary);
      }
    }

    public object EulerY
    {
      get
      {
        Vector3D? ninchedRotationAngles = this.NinchedRotationAngles;
        if (ninchedRotationAngles.HasValue)
          return (object) ninchedRotationAngles.Value.Y;
        return MixedProperty.Mixed;
      }
      set
      {
        Vector3D anglesFromPrimary = this.RotationAnglesFromPrimary;
        anglesFromPrimary.Y = (double) value;
        this.NinchedRotationAngles = new Vector3D?(anglesFromPrimary);
      }
    }

    public object EulerZ
    {
      get
      {
        Vector3D? ninchedRotationAngles = this.NinchedRotationAngles;
        if (ninchedRotationAngles.HasValue)
          return (object) ninchedRotationAngles.Value.Z;
        return MixedProperty.Mixed;
      }
      set
      {
        Vector3D anglesFromPrimary = this.RotationAnglesFromPrimary;
        anglesFromPrimary.Z = (double) value;
        this.NinchedRotationAngles = new Vector3D?(anglesFromPrimary);
      }
    }

    public Rotation3D OrientationFromEulerAngles
    {
      get
      {
        Vector3D? ninchedRotationAngles = this.NinchedRotationAngles;
        if (ninchedRotationAngles.HasValue)
          return (Rotation3D) new QuaternionRotation3D(Helper3D.QuaternionFromEulerAngles(ninchedRotationAngles.Value));
        return (Rotation3D) null;
      }
      set
      {
        if (value == null)
          return;
        this.NinchedRotationAngles = new Vector3D?(Helper3D.EulerAnglesFromQuaternion(Helper3D.QuaternionFromRotation3D(value)));
      }
    }

    private Vector3D RotationAnglesFromPrimary
    {
      get
      {
        Vector3D? nullable = this.NinchedRotationAngles;
        if (!nullable.HasValue)
        {
          SceneNodeObjectSet sceneNodeObjectSet = this.objectSet as SceneNodeObjectSet;
          if (sceneNodeObjectSet != null)
          {
            Base3DElement base3Delement = sceneNodeObjectSet.RepresentativeSceneNode as Base3DElement;
            if (base3Delement != null)
              nullable = (Vector3D?) base3Delement.GetComputedValue(this.rotationAnglesReference);
          }
        }
        if (nullable.HasValue)
          return nullable.Value;
        return new Vector3D();
      }
    }

    private Vector3D? NinchedRotationAngles
    {
      get
      {
        SceneNodeObjectSet sceneNodeObjectSet = this.objectSet as SceneNodeObjectSet;
        if (this.rotationProperty != null && sceneNodeObjectSet != null && (sceneNodeObjectSet.ViewModel != null && sceneNodeObjectSet.ViewModel.AnimationEditor.IsKeyFraming) && !sceneNodeObjectSet.ViewModel.IsForcingBaseValue)
        {
          Rotation3D rotation3D = this.rotationProperty.GetValue() as Rotation3D;
          if (rotation3D != null)
            return new Vector3D?(Helper3D.EulerAnglesFromQuaternion(Helper3D.QuaternionFromRotation3D(rotation3D)));
          return new Vector3D?();
        }
        if (this.rotationAnglesProperty == null)
          return new Vector3D?();
        object obj = this.rotationAnglesProperty.GetValue();
        if (obj == MixedProperty.Mixed)
          return new Vector3D?();
        return (Vector3D?) obj;
      }
      set
      {
        if (!value.HasValue)
          return;
        this.rotationAnglesProperty.SetValue((object) value.Value);
        this.OnPropertyChanged("EulerX");
        this.OnPropertyChanged("EulerY");
        this.OnPropertyChanged("EulerZ");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void UpdateProperties()
    {
      if (this.rotationAnglesProperty != null)
      {
        this.rotationAnglesProperty.OnRemoveFromCategory();
        this.rotationAnglesProperty = (PropertyReferenceProperty) null;
      }
      if (this.rotationProperty != null)
      {
        this.rotationProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnRotationProperty_PropertyReferenceChanged);
        this.rotationProperty.OnRemoveFromCategory();
        this.rotationProperty = (PropertyReferenceProperty) null;
      }
      if (this.transformGroupRotationProperty != null)
      {
        this.transformGroupRotationProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnRotationProperty_PropertyReferenceChanged);
        this.transformGroupRotationProperty.OnRemoveFromCategory();
        this.transformGroupRotationProperty = (PropertyReferenceProperty) null;
      }
      if (this.objectSet == null)
        return;
      IPlatform platform = this.objectSet.ProjectContext.Platform;
      PropertyReference propertyReference1 = new PropertyReference((ReferenceStep) platform.Metadata.GetProperty(platform.Metadata.DefaultTypeResolver, typeof (Transform3DGroup), MemberType.LocalProperty, "Children"), (ReferenceStep) IndexedClrPropertyReferenceStep.GetReferenceStep(platform.Metadata.DefaultTypeResolver, typeof (Transform3DCollection), CanonicalTransformOrder.RotateIndex));
      PropertyReference drotationReference = platform.Metadata.CommonProperties.RotateTransform3DRotationReference;
      ReferenceStep step = (ReferenceStep) DesignTimeProperties.ResolveDesignTimeReferenceStep(DesignTimeProperties.EulerAnglesProperty, (IPlatformMetadata) platform.Metadata);
      PropertyReference propertyReference2 = platform.Metadata.CommonProperties.RotateTransform3DReference.Append(step);
      if (this.objectSet is SceneNodeObjectSet)
      {
        this.rotationAnglesReference = this.propertyReference.Append(propertyReference2);
        this.rotationReference = this.propertyReference.Append(drotationReference);
        propertyReference1 = this.propertyReference.Append(propertyReference1);
      }
      else
      {
        this.rotationAnglesReference = propertyReference2;
        this.rotationReference = drotationReference;
      }
      this.rotationAnglesProperty = this.objectSet.CreateProperty(this.rotationAnglesReference, (AttributeCollection) null);
      this.rotationProperty = this.objectSet.CreateProperty(this.rotationReference, (AttributeCollection) null);
      this.rotationProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnRotationProperty_PropertyReferenceChanged);
      this.transformGroupRotationProperty = this.objectSet.CreateProperty(propertyReference1, (AttributeCollection) null);
      this.transformGroupRotationProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnRotationProperty_PropertyReferenceChanged);
      this.OnPropertyChanged("EulerX");
      this.OnPropertyChanged("EulerY");
      this.OnPropertyChanged("EulerZ");
      this.OnPropertyChanged("OrientationFromEulerAngles");
    }

    private void OnRotationProperty_PropertyReferenceChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      this.rotationProperty.Recache();
      this.OnPropertyChanged("EulerX");
      this.OnPropertyChanged("EulerY");
      this.OnPropertyChanged("EulerZ");
      this.OnPropertyChanged("OrientationFromEulerAngles");
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
