// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor.ProjectionGlobalOffsetTab
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
  public sealed class ProjectionGlobalOffsetTab : Grid, ITransformEditorTab, IComponentConnector
  {
    internal ProjectionGlobalOffsetTab ProjectionGlobalOffset;
    internal PropertyContainer GlobalOffsetXEditor;
    internal PropertyContainer GlobalOffsetYEditor;
    internal PropertyContainer GlobalOffsetZEditor;
    private bool _contentLoaded;

    public ProjectionGlobalOffsetTab()
    {
      this.InitializeComponent();
    }

    public void UpdatePropertyContainers(TransformPropertyLookup propertyLookup)
    {
      this.GlobalOffsetXEditor.PropertyEntry = (PropertyEntry) propertyLookup.CreateProperty("PlaneProjection.GlobalOffsetX");
      this.GlobalOffsetYEditor.PropertyEntry = (PropertyEntry) propertyLookup.CreateProperty("PlaneProjection.GlobalOffsetY");
      this.GlobalOffsetZEditor.PropertyEntry = (PropertyEntry) propertyLookup.CreateProperty("PlaneProjection.GlobalOffsetZ");
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/projectionglobaloffsettab.xaml", UriKind.Relative));
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
          this.ProjectionGlobalOffset = (ProjectionGlobalOffsetTab) target;
          break;
        case 2:
          this.GlobalOffsetXEditor = (PropertyContainer) target;
          break;
        case 3:
          this.GlobalOffsetYEditor = (PropertyContainer) target;
          break;
        case 4:
          this.GlobalOffsetZEditor = (PropertyContainer) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
