// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.CameraCategory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.Selection;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class CameraCategory : SceneNodeCategory
  {
    private List<ProjectionCameraElement> selectedCameras = new List<ProjectionCameraElement>();
    private PropertyReferenceProperty upDirectionProperty;
    private PropertyReferenceProperty lookDirectionProperty;
    private object cameraType;

    public bool IsCameraOrthographic
    {
      get
      {
        return typeof (OrthographicCamera).Equals(this.cameraType);
      }
      set
      {
        if (!value)
          return;
        this.SetCameraTypeOnSelection(typeof (OrthographicCamera));
      }
    }

    public bool IsCameraPerspective
    {
      get
      {
        return typeof (PerspectiveCamera).Equals(this.cameraType);
      }
      set
      {
        if (!value)
          return;
        this.SetCameraTypeOnSelection(typeof (PerspectiveCamera));
      }
    }

    public double RotationAngle
    {
      get
      {
        if (this.lookDirectionProperty == null || this.upDirectionProperty == null || (this.lookDirectionProperty.IsMixedValue || this.upDirectionProperty.IsMixedValue))
          return 0.0;
        return this.SpinnerRotationAngleFromCameraUpVector((Vector3D) this.lookDirectionProperty.GetValue(), (Vector3D) this.upDirectionProperty.GetValue());
      }
      set
      {
        this.upDirectionProperty.SetValue((object) this.CameraUpVectorFromSpinnerRotationAngle(value));
        this.OnPropertyChanged("RotationAngle");
      }
    }

    public CameraCategory(string localizedName, IMessageLoggingService messageLogger)
      : base(CategoryLocalizationHelper.CategoryName.Camera, localizedName, messageLogger)
    {
    }

    public override void OnSelectionChanged(SceneNode[] selectedObjects)
    {
      base.OnSelectionChanged(selectedObjects);
      this.selectedCameras.Clear();
      this.cameraType = (object) null;
      if (this.upDirectionProperty != null)
      {
        this.upDirectionProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnRotationAngleChanged);
        this.upDirectionProperty = (PropertyReferenceProperty) null;
      }
      if (this.lookDirectionProperty != null)
      {
        this.lookDirectionProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnRotationAngleChanged);
        this.lookDirectionProperty = (PropertyReferenceProperty) null;
      }
      foreach (PropertyEntry propertyEntry in (Collection<PropertyEntry>) this.BasicProperties)
      {
        if (propertyEntry.PropertyName == "UpDirection")
          this.upDirectionProperty = propertyEntry as PropertyReferenceProperty;
        else if (propertyEntry.PropertyName == "LookDirection")
          this.lookDirectionProperty = propertyEntry as PropertyReferenceProperty;
      }
      if (this.upDirectionProperty != null)
        this.upDirectionProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnRotationAngleChanged);
      if (this.lookDirectionProperty != null)
        this.lookDirectionProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnRotationAngleChanged);
      foreach (object obj in selectedObjects)
      {
        ProjectionCameraElement projectionCameraElement = obj as ProjectionCameraElement;
        if (projectionCameraElement != null)
        {
          this.selectedCameras.Add(projectionCameraElement);
          if (this.cameraType == null)
            this.cameraType = (object) projectionCameraElement.TargetType;
          else if (!object.Equals(this.cameraType, (object) projectionCameraElement.TargetType))
            this.cameraType = MixedProperty.Mixed;
        }
      }
      this.OnPropertyChanged("IsCameraOrthographic");
      this.OnPropertyChanged("IsCameraPerspective");
      this.OnPropertyChanged("RotationAngle");
    }

    private void OnRotationAngleChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      this.OnPropertyChanged("RotationAngle");
    }

    private void SetCameraTypeOnSelection(Type cameraType)
    {
      if (this.selectedCameras.Count == 0 || this.cameraType.Equals((object) cameraType))
        return;
      this.cameraType = (object) cameraType;
      SceneViewModel viewModel = this.selectedCameras[0].ViewModel;
      SceneElementSelectionSet elementSelectionSet = viewModel.ElementSelectionSet;
      using (SceneEditTransaction editTransaction = viewModel.Document.CreateEditTransaction(StringTable.UndoUnitChangeCameraType))
      {
        foreach (ProjectionCameraElement projectionCameraElement1 in this.selectedCameras)
        {
          if (!(projectionCameraElement1.TargetType == cameraType))
          {
            ProjectionCameraElement projectionCameraElement2;
            if (typeof (OrthographicCamera).IsAssignableFrom(cameraType))
            {
              projectionCameraElement2 = (ProjectionCameraElement) OrthographicCameraElement.Factory.Instantiate(projectionCameraElement1.ViewModel);
              projectionCameraElement2.SetValue(OrthographicCameraElement.WidthProperty, (object) 10.0);
            }
            else if (typeof (PerspectiveCamera).IsAssignableFrom(cameraType))
            {
              projectionCameraElement2 = (ProjectionCameraElement) PerspectiveCameraElement.Factory.Instantiate(projectionCameraElement1.ViewModel);
              projectionCameraElement2.SetValue(PerspectiveCameraElement.FieldOfViewProperty, (object) 45.0);
            }
            else
              continue;
            projectionCameraElement1.ViewModel.AnimationEditor.DeleteAllAnimations((SceneNode) projectionCameraElement1);
            Dictionary<IPropertyId, SceneNode> properties = SceneElementHelper.StoreProperties((SceneNode) projectionCameraElement1);
            elementSelectionSet.RemoveSelection((SceneElement) projectionCameraElement1);
            SceneElement parentElement = projectionCameraElement1.ParentElement;
            ISceneNodeCollection<SceneNode> collectionForProperty = parentElement.GetCollectionForProperty((IPropertyId) parentElement.GetPropertyForChild((SceneNode) projectionCameraElement1));
            int index = collectionForProperty.IndexOf((SceneNode) projectionCameraElement1);
            collectionForProperty[index] = (SceneNode) projectionCameraElement2;
            SceneElementHelper.ApplyProperties((SceneNode) projectionCameraElement2, properties);
            elementSelectionSet.ExtendSelection((SceneElement) projectionCameraElement2);
          }
        }
        editTransaction.Commit();
      }
      this.OnPropertyChanged("IsCameraOrthographic");
      this.OnPropertyChanged("IsCameraPerspective");
    }

    private Vector3D CameraUpVectorFromSpinnerRotationAngle(double value)
    {
      double num = value + 90.0;
      Vector3D vector3D = -(Vector3D) this.lookDirectionProperty.GetValue();
      vector3D.Normalize();
      Vector3D vector = Vector3D.CrossProduct(Vector3D.CrossProduct(vector3D, new Vector3D(0.0, 1.0, 0.0)), vector3D);
      return new RotateTransform3D((Rotation3D) new AxisAngleRotation3D(vector3D, -num)).Transform(vector);
    }

    private double SpinnerRotationAngleFromCameraUpVector(Vector3D lookDirection, Vector3D cameraUp)
    {
      Vector3D vector3D = -lookDirection;
      vector3D.Normalize();
      Vector3D vector2 = Vector3D.CrossProduct(Vector3D.CrossProduct(vector3D, new Vector3D(0.0, 1.0, 0.0)), vector3D);
      double num;
      if (Vector3D.DotProduct(cameraUp, vector2) < 0.0)
      {
        double d = (cameraUp + vector2).Length / 2.0;
        if (d > 1.0)
          d = 1.0;
        num = 57.2957795130823 * (Math.PI - 2.0 * Math.Asin(d));
      }
      else
      {
        double d = (cameraUp - vector2).Length / 2.0;
        if (d > 1.0)
          d = 1.0;
        num = 114.591559026165 * Math.Asin(d);
      }
      if (Vector3D.DotProduct(Vector3D.CrossProduct(cameraUp, vector2), vector3D) < 0.0)
        num = 360.0 - num;
      return num - 90.0;
    }
  }
}
