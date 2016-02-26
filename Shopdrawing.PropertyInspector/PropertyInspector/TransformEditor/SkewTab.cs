// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor.SkewTab
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
  public sealed class SkewTab : Grid, ITransformEditorTab, IComponentConnector
  {
    internal SkewTab SkewTabGrid;
    internal PropertyContainer SkewXEditor;
    internal PropertyContainer SkewYEditor;
    private bool _contentLoaded;

    public SkewTab()
    {
      this.InitializeComponent();
    }

    public void UpdatePropertyContainers(TransformPropertyLookup propertyLookup)
    {
      AttributeCollection attributes = new AttributeCollection(new Attribute[2]
      {
        (Attribute) new NumberRangesAttribute(new double?(-89.0), new double?(-89.0), new double?(89.0), new double?(89.0), new bool?()),
        (Attribute) new NumberIncrementsAttribute(new double?(0.1), new double?(1.0), new double?(5.0))
      });
      if (propertyLookup.TransformType == TransformType.Transform2D)
      {
        if (propertyLookup.Relative)
        {
          this.SkewXEditor.set_PropertyEntry((PropertyEntry) propertyLookup.CreateProperty("SkewX", attributes));
          this.SkewYEditor.set_PropertyEntry((PropertyEntry) propertyLookup.CreateProperty("SkewY", attributes));
        }
        else
        {
          this.SkewXEditor.set_PropertyEntry((PropertyEntry) propertyLookup.CreateProperty("SkewTransform/AngleX", attributes));
          this.SkewYEditor.set_PropertyEntry((PropertyEntry) propertyLookup.CreateProperty("SkewTransform/AngleY", attributes));
        }
      }
      else
      {
        this.SkewXEditor.set_PropertyEntry((PropertyEntry) null);
        this.SkewYEditor.set_PropertyEntry((PropertyEntry) null);
      }
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/categoryeditors/transform/skewtab.xaml", UriKind.Relative));
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
          this.SkewTabGrid = (SkewTab) target;
          break;
        case 2:
          this.SkewXEditor = (PropertyContainer) target;
          break;
        case 3:
          this.SkewYEditor = (PropertyContainer) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
