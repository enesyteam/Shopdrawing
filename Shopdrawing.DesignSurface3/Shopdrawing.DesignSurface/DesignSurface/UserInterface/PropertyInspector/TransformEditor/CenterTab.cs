// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor.CenterTab
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel;
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
  public sealed class CenterTab : Grid, ITransformEditorTab, IComponentConnector
  {
    private PropertyReference renderTransformOriginXProperty;
    private PropertyReference renderTransformOriginYProperty;
    internal CenterTab CenterTabGrid;
    internal PropertyContainer CenterPointXEditor;
    internal PropertyContainer CenterPointYEditor;
    internal PropertyContainer CenterPointZEditor;
    private bool _contentLoaded;

    public CenterTab()
    {
      this.InitializeComponent();
    }

    public void UpdatePropertyContainers(TransformPropertyLookup propertyLookup)
    {
      AttributeCollection attributes = new AttributeCollection(new Attribute[2]
      {
        (Attribute) new NumberRangesAttribute(new double?(double.NegativeInfinity), new double?(0.0), new double?(1.0), new double?(double.PositiveInfinity), new bool?()),
        (Attribute) new NumberIncrementsAttribute(new double?(0.01), new double?(0.05), new double?(0.1))
      });
      ITypeResolver typeResolver = (ITypeResolver) propertyLookup.TransformProperty.SceneNodeObjectSet.ProjectContext;
      PropertyReference propertyReference = new PropertyReference(typeResolver.ResolveProperty(Base2DElement.RenderTransformOriginProperty) as ReferenceStep);
      ReferenceStep property1 = PlatformTypeHelper.GetProperty(typeResolver, PlatformTypes.Point, MemberType.LocalProperty, "X");
      ReferenceStep property2 = PlatformTypeHelper.GetProperty(typeResolver, PlatformTypes.Point, MemberType.LocalProperty, "Y");
      this.renderTransformOriginXProperty = propertyReference.Append(property1);
      this.renderTransformOriginYProperty = propertyReference.Append(property2);
      PropertyReference reference = propertyLookup.TransformProperty.Reference;
      if (reference.Count == 1 && reference.FirstStep.Equals((object) Base2DElement.RenderTransformProperty))
      {
        if (propertyLookup.Relative)
        {
          this.CenterPointXEditor.PropertyEntry = (PropertyEntry) null;
          this.CenterPointYEditor.PropertyEntry = (PropertyEntry) null;
        }
        else
        {
          this.CenterPointXEditor.PropertyEntry = (PropertyEntry) propertyLookup.CreateNormalProperty(this.renderTransformOriginXProperty, attributes);
          this.CenterPointYEditor.PropertyEntry = (PropertyEntry) propertyLookup.CreateNormalProperty(this.renderTransformOriginYProperty, attributes);
        }
      }
      else
      {
        this.CenterPointXEditor.PropertyEntry = (PropertyEntry) propertyLookup.CreateProperty("CenterX", attributes);
        this.CenterPointYEditor.PropertyEntry = (PropertyEntry) propertyLookup.CreateProperty("CenterY", attributes);
      }
      if (propertyLookup.TransformType == TransformType.Transform3D)
        this.CenterPointZEditor.PropertyEntry = (PropertyEntry) propertyLookup.CreateProperty("CenterZ", attributes);
      else
        this.CenterPointZEditor.PropertyEntry = (PropertyEntry) null;
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/categoryeditors/transform/centertab.xaml", UriKind.Relative));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerNonUserCode]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.CenterTabGrid = (CenterTab) target;
          break;
        case 2:
          this.CenterPointXEditor = (PropertyContainer) target;
          break;
        case 3:
          this.CenterPointYEditor = (PropertyContainer) target;
          break;
        case 4:
          this.CenterPointZEditor = (PropertyContainer) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
