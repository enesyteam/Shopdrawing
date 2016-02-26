// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor.ProjectionCenterTab
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class ProjectionCenterTab : Grid, ITransformEditorTab, IComponentConnector
  {
    internal ProjectionCenterTab ProjectionCenter;
    internal PropertyContainer CenterOfRotationXEditor;
    internal PropertyContainer CenterOfRotationYEditor;
    internal PropertyContainer CenterOfRotationZEditor;
    private bool _contentLoaded;

    public ProjectionCenterTab()
    {
      this.InitializeComponent();
    }

    public void UpdatePropertyContainers(TransformPropertyLookup propertyLookup)
    {
      AttributeCollection attributes = new AttributeCollection(new Attribute[1]
      {
        (Attribute) new NumberIncrementsAttribute(new double?(0.01), new double?(0.1), new double?(1.0))
      });
      this.CenterOfRotationXEditor.PropertyEntry = (PropertyEntry) propertyLookup.CreateProperty("PlaneProjection.CenterOfRotationX", attributes);
      this.CenterOfRotationYEditor.PropertyEntry = (PropertyEntry) propertyLookup.CreateProperty("PlaneProjection.CenterOfRotationY", attributes);
      this.CenterOfRotationZEditor.PropertyEntry = (PropertyEntry) propertyLookup.CreateProperty("PlaneProjection.CenterOfRotationZ", attributes);
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/projectioncentertab.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.ProjectionCenter = (ProjectionCenterTab) target;
          break;
        case 2:
          this.CenterOfRotationXEditor = (PropertyContainer) target;
          break;
        case 3:
          this.CenterOfRotationYEditor = (PropertyContainer) target;
          break;
        case 4:
          this.CenterOfRotationZEditor = (PropertyContainer) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
