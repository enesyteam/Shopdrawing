// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor.ProjectionLocalOffsetTab
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
  public sealed class ProjectionLocalOffsetTab : Grid, ITransformEditorTab, IComponentConnector
  {
    internal ProjectionLocalOffsetTab ProjectionLocalOffset;
    internal PropertyContainer LocalOffsetXEditor;
    internal PropertyContainer LocalOffsetYEditor;
    internal PropertyContainer LocalOffsetZEditor;
    private bool _contentLoaded;

    public ProjectionLocalOffsetTab()
    {
      this.InitializeComponent();
    }

    public void UpdatePropertyContainers(TransformPropertyLookup propertyLookup)
    {
      this.LocalOffsetXEditor.set_PropertyEntry((PropertyEntry) propertyLookup.CreateProperty("PlaneProjection.LocalOffsetX"));
      this.LocalOffsetYEditor.set_PropertyEntry((PropertyEntry) propertyLookup.CreateProperty("PlaneProjection.LocalOffsetY"));
      this.LocalOffsetZEditor.set_PropertyEntry((PropertyEntry) propertyLookup.CreateProperty("PlaneProjection.LocalOffsetZ"));
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/projectionlocaloffsettab.xaml", UriKind.Relative));
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
          this.ProjectionLocalOffset = (ProjectionLocalOffsetTab) target;
          break;
        case 2:
          this.LocalOffsetXEditor = (PropertyContainer) target;
          break;
        case 3:
          this.LocalOffsetYEditor = (PropertyContainer) target;
          break;
        case 4:
          this.LocalOffsetZEditor = (PropertyContainer) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
