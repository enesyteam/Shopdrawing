// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor.ProjectionRotationTab
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
  public sealed class ProjectionRotationTab : Grid, ITransformEditorTab, IComponentConnector
  {
    internal ProjectionRotationTab ProjectionRotation;
    internal PropertyContainer RotateXEditor;
    internal PropertyContainer RotateYEditor;
    internal PropertyContainer RotateZEditor;
    private bool _contentLoaded;

    public ProjectionRotationTab()
    {
      this.InitializeComponent();
    }

    public void UpdatePropertyContainers(TransformPropertyLookup propertyLookup)
    {
      this.RotateXEditor.PropertyEntry = (PropertyEntry) propertyLookup.CreateProperty("PlaneProjection.RotationX");
      this.RotateYEditor.PropertyEntry = (PropertyEntry) propertyLookup.CreateProperty("PlaneProjection.RotationY");
      this.RotateZEditor.PropertyEntry = (PropertyEntry) propertyLookup.CreateProperty("PlaneProjection.RotationZ");
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/projectionrotationtab.xaml", UriKind.Relative));
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
      switch (connectionId)
      {
        case 1:
          this.ProjectionRotation = (ProjectionRotationTab) target;
          break;
        case 2:
          this.RotateXEditor = (PropertyContainer) target;
          break;
        case 3:
          this.RotateYEditor = (PropertyContainer) target;
          break;
        case 4:
          this.RotateZEditor = (PropertyContainer) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
