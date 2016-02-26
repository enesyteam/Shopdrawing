// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor.ScaleTab
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
  public sealed class ScaleTab : Grid, ITransformEditorTab, IComponentConnector
  {
    internal ScaleTab ScaleTabGrid;
    internal PropertyContainer ScaleXEditor;
    internal PropertyContainer ScaleYEditor;
    internal PropertyContainer ScaleZEditor;
    private bool _contentLoaded;

    public ScaleTab()
    {
      this.InitializeComponent();
    }

    public void UpdatePropertyContainers(TransformPropertyLookup propertyLookup)
    {
      double num = propertyLookup.TransformType == TransformType.Transform3D ? 0.0 : -5.0;
      AttributeCollection attributes = new AttributeCollection(new Attribute[2]
      {
        (Attribute) new NumberRangesAttribute(new double?(propertyLookup.TransformType == TransformType.Transform3D ? 0.0 : double.NegativeInfinity), new double?(num), new double?(5.0), new double?(double.PositiveInfinity), new bool?()),
        (Attribute) new NumberIncrementsAttribute(new double?(0.01), new double?(0.05), new double?(0.1))
      });
      if (propertyLookup.TransformType == TransformType.Transform3D)
      {
        if (propertyLookup.Relative)
        {
          this.ScaleXEditor.set_PropertyEntry((PropertyEntry) propertyLookup.CreateProperty("ScaleX", attributes));
          this.ScaleYEditor.set_PropertyEntry((PropertyEntry) propertyLookup.CreateProperty("ScaleY", attributes));
          this.ScaleZEditor.set_PropertyEntry((PropertyEntry) propertyLookup.CreateProperty("ScaleZ", attributes));
        }
        else
        {
          this.ScaleXEditor.set_PropertyEntry((PropertyEntry) propertyLookup.CreateProperty("ScaleTransform/ScaleX", attributes));
          this.ScaleYEditor.set_PropertyEntry((PropertyEntry) propertyLookup.CreateProperty("ScaleTransform/ScaleY", attributes));
          this.ScaleZEditor.set_PropertyEntry((PropertyEntry) propertyLookup.CreateProperty("ScaleTransform/ScaleZ", attributes));
        }
      }
      else
      {
        if (propertyLookup.Relative)
        {
          this.ScaleXEditor.set_PropertyEntry((PropertyEntry) propertyLookup.CreateProperty("ScaleX", attributes));
          this.ScaleYEditor.set_PropertyEntry((PropertyEntry) propertyLookup.CreateProperty("ScaleY", attributes));
        }
        else
        {
          this.ScaleXEditor.set_PropertyEntry((PropertyEntry) propertyLookup.CreateProperty("ScaleTransform/ScaleX", attributes));
          this.ScaleYEditor.set_PropertyEntry((PropertyEntry) propertyLookup.CreateProperty("ScaleTransform/ScaleY", attributes));
        }
        this.ScaleZEditor.set_PropertyEntry((PropertyEntry) null);
      }
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/categoryeditors/transform/scaletab.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
      return Delegate.CreateDelegate(delegateType, (object) this, handler);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.ScaleTabGrid = (ScaleTab) target;
          break;
        case 2:
          this.ScaleXEditor = (PropertyContainer) target;
          break;
        case 3:
          this.ScaleYEditor = (PropertyContainer) target;
          break;
        case 4:
          this.ScaleZEditor = (PropertyContainer) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
