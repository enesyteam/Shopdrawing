// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor.RegistrationPointModel
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor
{
  public class RegistrationPointModel : NotifyingObject
  {
    private static readonly double RegistrationPointTolerance = 1E-15;
    private SceneNodeProperty property;
    private PropertyReference propertyReference;
    private SceneNodeObjectSet objectSet;

    public PropertyReference PropertyReference
    {
      get
      {
        return this.propertyReference;
      }
      set
      {
        if (this.property != null)
        {
          this.property.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.property_PropertyReferenceChanged);
          this.property.OnRemoveFromCategory();
        }
        this.propertyReference = value;
        if (this.propertyReference != null)
        {
          this.property = this.objectSet.CreateSceneNodeProperty(this.propertyReference, (AttributeCollection) null);
          this.property.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.property_PropertyReferenceChanged);
          this.OnPropertyChanged("RegistrationPoint");
        }
        else
          this.property = (SceneNodeProperty) null;
      }
    }

    public PropertyReference TranslationXPropertyReference
    {
      get
      {
        return this.PropertyReference.Append(this.objectSet.ProjectContext.Platform.Metadata.CommonProperties.TranslationX);
      }
    }

    public PropertyReference TranslationYPropertyReference
    {
      get
      {
        return this.PropertyReference.Append(this.objectSet.ProjectContext.Platform.Metadata.CommonProperties.TranslationY);
      }
    }

    public RegistrationPointFlags RegistrationPoint
    {
      get
      {
        return this.FindRegistrationPoint();
      }
      set
      {
        this.ChangeRegistrationPoint(value);
      }
    }

    private bool IsRenderTransform
    {
      get
      {
        if (this.PropertyReference != null && this.PropertyReference.Count == 1)
          return this.PropertyReference[0].Equals((object) Base2DElement.RenderTransformProperty);
        return false;
      }
    }

    internal RegistrationPointModel(SceneNodeObjectSet objectSet)
    {
      this.objectSet = objectSet;
    }

    private void property_PropertyReferenceChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      this.OnPropertyChanged("RegistrationPoint");
    }

    public void Unload()
    {
      this.PropertyReference = (PropertyReference) null;
    }

    private RegistrationPointFlags FindRegistrationPoint()
    {
      if (!this.IsRenderTransform || this.objectSet.ViewModel == null)
        return RegistrationPointFlags.None;
      RegistrationPointFlags registrationPointFlags = RegistrationPointFlags.None;
      bool flag = false;
      foreach (SceneNode sceneNode in this.objectSet.Objects)
      {
        BaseFrameworkElement element = sceneNode as BaseFrameworkElement;
        if (element != null)
        {
          RegistrationPointFlags registrationPointForElement = this.FindRegistrationPointForElement(element);
          if (!flag)
          {
            registrationPointFlags = registrationPointForElement;
            flag = true;
          }
          if (registrationPointForElement != registrationPointFlags)
            return RegistrationPointFlags.None;
        }
      }
      return registrationPointFlags;
    }

    private RegistrationPointFlags FindRegistrationPointForElement(BaseFrameworkElement element)
    {
      if (!element.IsViewObjectValid)
        return RegistrationPointFlags.None;
      Point elementCoordinates = element.RenderTransformOriginInElementCoordinates;
      Point point1 = elementCoordinates;
      if (this.IsRenderTransform)
      {
        Rect computedTightBounds = element.GetComputedTightBounds();
        point1 = new Point(elementCoordinates.X - computedTightBounds.Left, elementCoordinates.Y - computedTightBounds.Top);
        Point point2 = (Point) element.GetComputedValueAsWpf(Base2DElement.RenderTransformOriginProperty);
        Point point3 = new Point();
        bool flag = false;
        if (!Tolerances.NearZero(computedTightBounds.Width))
          point1.X /= computedTightBounds.Width;
        else if (!Tolerances.NearZero(point1.X))
          point1.X = -1.0;
        if (!Tolerances.NearZero(computedTightBounds.Height))
          point1.Y /= computedTightBounds.Height;
        else if (!Tolerances.NearZero(point1.Y))
          point1.Y = -1.0;
        if (!double.IsNaN(point2.X) && !double.IsNaN(point2.Y))
        {
          point3.X = point2.X;
          point3.Y = point2.Y;
          flag = true;
        }
        if ((Tolerances.NearZero(computedTightBounds.Width) || Tolerances.NearZero(computedTightBounds.Height)) && flag)
        {
          if (!Tolerances.NearZero(computedTightBounds.Width) && Tolerances.AreClose(point1.X, point3.X) && Tolerances.NearZero(point1.Y))
            point1 = point3;
          else if (!Tolerances.NearZero(computedTightBounds.Height) && Tolerances.AreClose(point1.Y, point3.Y) && Tolerances.NearZero(point1.X))
            point1 = point3;
          else if (Tolerances.NearZero(computedTightBounds.Width) && Tolerances.NearZero(computedTightBounds.Height) && Tolerances.NearZero(point1))
            point1 = point3;
        }
      }
      foreach (RegistrationPointFlags registrationPoint in Enum.GetValues(typeof (RegistrationPointFlags)))
      {
        if ((this.CenterFromRegistrationPoint(registrationPoint) - point1).LengthSquared <= RegistrationPointModel.RegistrationPointTolerance)
          return registrationPoint;
      }
      return RegistrationPointFlags.None;
    }

    private Point CenterFromRegistrationPoint(RegistrationPointFlags registrationPoint)
    {
      if (registrationPoint == RegistrationPointFlags.None)
        return new Point(double.NaN, double.NaN);
      double x = 0.5;
      double y = 0.5;
      if ((registrationPoint & RegistrationPointFlags.Right) != RegistrationPointFlags.Center)
        x = 1.0;
      if ((registrationPoint & RegistrationPointFlags.Left) != RegistrationPointFlags.Center)
        x = 0.0;
      if ((registrationPoint & RegistrationPointFlags.Bottom) != RegistrationPointFlags.Center)
        y = 1.0;
      if ((registrationPoint & RegistrationPointFlags.Top) != RegistrationPointFlags.Center)
        y = 0.0;
      return new Point(x, y);
    }

    private void ChangeRegistrationPoint(RegistrationPointFlags registrationPoint)
    {
      if (this.objectSet.ViewModel == null)
        return;
      bool flag1 = false;
      using (SceneEditTransaction editTransaction = this.objectSet.ViewModel.CreateEditTransaction(StringTable.UndoUnitTransformPaneCenterPoint))
      {
        Point point1 = this.CenterFromRegistrationPoint(registrationPoint);
        foreach (SceneElement sceneElement in this.objectSet.Objects)
        {
          BaseFrameworkElement frameworkElement = sceneElement as BaseFrameworkElement;
          if (frameworkElement != null)
          {
            flag1 = true;
            CanonicalTransform canonicalTransform1 = new CanonicalTransform((Transform) frameworkElement.GetComputedValueAsWpf(this.PropertyReference));
            CanonicalTransform canonicalTransform2 = new CanonicalTransform(canonicalTransform1);
            if (this.IsRenderTransform)
            {
              Point point2 = (Point) frameworkElement.GetComputedValueAsWpf(Base2DElement.RenderTransformOriginProperty);
              Rect computedTightBounds = frameworkElement.GetComputedTightBounds();
              Point oldOrigin = new Point(point2.X * computedTightBounds.Width + computedTightBounds.Left, point2.Y * computedTightBounds.Height + computedTightBounds.Top);
              Point newOrigin = new Point(point1.X * computedTightBounds.Width + computedTightBounds.Left, point1.Y * computedTightBounds.Height + computedTightBounds.Top);
              canonicalTransform2.UpdateForNewOrigin(oldOrigin, newOrigin);
              bool flag2 = false;
              if (!double.IsNaN(point1.X) && !object.Equals((object) point1.X, (object) point2.X))
              {
                point2.X = point1.X;
                flag2 = true;
              }
              if (!double.IsNaN(point1.Y) && !object.Equals((object) point1.Y, (object) point2.Y))
              {
                point2.Y = point1.Y;
                flag2 = true;
              }
              if (flag2)
                frameworkElement.SetValueAsWpf(Base2DElement.RenderTransformOriginProperty, (object) point2);
            }
            double num1 = RoundingHelper.RoundToDoublePrecision(canonicalTransform2.TranslationX, 6);
            double num2 = RoundingHelper.RoundToDoublePrecision(canonicalTransform2.TranslationY, 6);
            if (canonicalTransform1.TranslationX != num1)
              frameworkElement.SetValue(this.TranslationXPropertyReference, (object) num1);
            if (canonicalTransform1.TranslationY != num2)
              frameworkElement.SetValue(this.TranslationYPropertyReference, (object) num2);
          }
        }
        if (flag1)
          editTransaction.Commit();
        else
          editTransaction.Cancel();
      }
    }
  }
}
