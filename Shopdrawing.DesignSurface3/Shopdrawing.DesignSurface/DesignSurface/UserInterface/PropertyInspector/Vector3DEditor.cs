// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.Vector3DEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.Framework;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Media3D;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class Vector3DEditor : Grid, INotifyPropertyChanged, IComponentConnector
  {
    private static Vector3D pointingDirection = new Vector3D(0.0, 0.0, 1.0);
    private static Vector3D perpendicularDirection = new Vector3D(0.0, 1.0, 0.0);
    private SceneNodeProperty editingProperty;
    private double x;
    private double y;
    private double z;
    private bool arcBallVisible;
    internal Vector3DEditor TreeRoot;
    private bool _contentLoaded;

    public double X
    {
      get
      {
        return this.x;
      }
      set
      {
        this.x = value;
        this.UpdateValue("X");
      }
    }

    public double Y
    {
      get
      {
        return this.y;
      }
      set
      {
        this.y = value;
        this.UpdateValue("Y");
      }
    }

    public double Z
    {
      get
      {
        return this.z;
      }
      set
      {
        this.z = value;
        this.UpdateValue("Z");
      }
    }

    public bool ArcBallVisible
    {
      get
      {
        return this.arcBallVisible;
      }
      set
      {
        this.arcBallVisible = value;
        this.FirePropertyChanged("ArcBallVisible");
      }
    }

    public Rotation3D Orientation3D
    {
      get
      {
        return Vector3DEditor.GetRotation3DFromDirection(new Vector3D(this.x, this.y, this.z));
      }
      set
      {
        Vector3D vector2 = new RotateTransform3D(value).Transform(Vector3DEditor.pointingDirection);
        if (Tolerances.AreClose(new Vector3D(this.x, this.y, this.z), vector2))
          return;
        this.x = vector2.X;
        this.y = vector2.Y;
        this.z = vector2.Z;
        this.UpdateValue("Orientation3D");
        this.FirePropertyChanged("X");
        this.FirePropertyChanged("Y");
        this.FirePropertyChanged("Z");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public Vector3DEditor()
    {
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.OnDataContextChangedRebuild);
      this.InitializeComponent();
    }

    private void OnDataContextChangedRebuild(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (this.editingProperty != null)
      {
        this.editingProperty.PropertyReferenceChanged -= new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnVector3DChanged);
        this.editingProperty.PropertyChanged -= new PropertyChangedEventHandler(this.OnEditingPropertyPropertyChanged);
        this.editingProperty = (SceneNodeProperty) null;
      }
      Microsoft.Windows.Design.PropertyEditing.PropertyValue propertyValue = this.DataContext as Microsoft.Windows.Design.PropertyEditing.PropertyValue;
      if (propertyValue != null)
        this.editingProperty = (SceneNodeProperty) propertyValue.ParentProperty;
      if (this.editingProperty != null)
      {
        this.editingProperty.PropertyChanged += new PropertyChangedEventHandler(this.OnEditingPropertyPropertyChanged);
        this.editingProperty.PropertyReferenceChanged += new Microsoft.Expression.DesignSurface.Documents.PropertyReferenceChangedEventHandler(this.OnVector3DChanged);
      }
      this.Rebuild();
    }

    private void OnVector3DChanged(object sender, PropertyReferenceChangedEventArgs e)
    {
      this.Rebuild();
    }

    private void OnEditingPropertyPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (!(e.PropertyName == "Associated"))
        return;
      this.Rebuild();
    }

    private void UpdateValue(string propertyName)
    {
      if (typeof (Vector3D).IsAssignableFrom(this.editingProperty.PropertyType))
        this.editingProperty.SetValue((object) new Vector3D(this.x, this.y, this.z));
      else if (typeof (Point3D).IsAssignableFrom(this.editingProperty.PropertyType))
        this.editingProperty.SetValue((object) new Point3D(this.x, this.y, this.z));
      this.FirePropertyChanged(propertyName);
    }

    private void Rebuild()
    {
      if (this.editingProperty != null && this.editingProperty.Associated)
      {
        object obj = this.editingProperty.GetValue();
        if (obj is Vector3D)
        {
          Vector3D vector3D = (Vector3D) obj;
          this.x = vector3D.X;
          this.y = vector3D.Y;
          this.z = vector3D.Z;
        }
        else if (obj is Point3D)
        {
          Point3D point3D = (Point3D) obj;
          this.x = point3D.X;
          this.y = point3D.Y;
          this.z = point3D.Z;
        }
      }
      else
      {
        this.x = 0.0;
        this.y = 0.0;
        this.z = 0.0;
      }
      this.FirePropertyChanged("X");
      this.FirePropertyChanged("Y");
      this.FirePropertyChanged("Z");
      this.FirePropertyChanged("Orientation3D");
    }

    private void FirePropertyChanged(string propertyName)
    {
      if (this.PropertyChanged == null)
        return;
      this.PropertyChanged((object) this, new PropertyChangedEventArgs(propertyName));
    }

    public static Rotation3D GetRotation3DFromDirection(Vector3D direction)
    {
      direction.Normalize();
      Vector3D axis = Vector3D.CrossProduct(Vector3DEditor.pointingDirection, direction);
      double d = Vector3D.DotProduct(Vector3DEditor.pointingDirection, direction);
      return !Tolerances.AreClose(axis.LengthSquared, 0.0) ? (Rotation3D) new AxisAngleRotation3D(axis, Math.Acos(d) * 57.2957795130823) : (!Tolerances.AreClose(d, 1.0) ? (Rotation3D) new AxisAngleRotation3D(Vector3DEditor.perpendicularDirection, 180.0) : Rotation3D.Identity);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/vector3deditor.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      if (connectionId == 1)
        this.TreeRoot = (Vector3DEditor) target;
      else
        this._contentLoaded = true;
    }
  }
}
