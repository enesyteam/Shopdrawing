// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TransformEditor.PositionTab
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
  public sealed class PositionTab : Grid, ITransformEditorTab, IComponentConnector
  {
    internal PositionTab PositionTabGrid;
    internal RegistrationPointSelector RegistrationPointSelector;
    internal PropertyContainer TranslateXEditor;
    internal PropertyContainer TranslateYEditor;
    internal PropertyContainer TranslateZEditor;
    private bool _contentLoaded;

    public PositionTab()
    {
      this.InitializeComponent();
    }

    public void UpdatePropertyContainers(TransformPropertyLookup propertyLookup)
    {
      if (propertyLookup.TransformType == TransformType.Transform3D)
      {
        if (propertyLookup.Relative)
        {
          this.TranslateXEditor.PropertyEntry = (PropertyEntry) propertyLookup.CreateProperty("TranslationX");
          this.TranslateYEditor.PropertyEntry = (PropertyEntry) propertyLookup.CreateProperty("TranslationY");
          this.TranslateZEditor.PropertyEntry = (PropertyEntry) propertyLookup.CreateProperty("TranslationZ");
        }
        else
        {
          this.TranslateXEditor.PropertyEntry = (PropertyEntry) propertyLookup.CreateProperty("TranslateTransform/OffsetX");
          this.TranslateYEditor.PropertyEntry = (PropertyEntry) propertyLookup.CreateProperty("TranslateTransform/OffsetY");
          this.TranslateZEditor.PropertyEntry = (PropertyEntry) propertyLookup.CreateProperty("TranslateTransform/OffsetZ");
        }
      }
      else
      {
        if (propertyLookup.Relative)
        {
          this.TranslateXEditor.PropertyEntry = (PropertyEntry) propertyLookup.CreateProperty("TranslationX");
          this.TranslateYEditor.PropertyEntry = (PropertyEntry) propertyLookup.CreateProperty("TranslationY");
        }
        else
        {
          this.TranslateXEditor.PropertyEntry = (PropertyEntry) propertyLookup.CreateProperty("TranslateTransform/X");
          this.TranslateYEditor.PropertyEntry = (PropertyEntry) propertyLookup.CreateProperty("TranslateTransform/Y");
        }
        this.TranslateZEditor.PropertyEntry = (PropertyEntry) null;
      }
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/categoryeditors/transform/positiontab.xaml", UriKind.Relative));
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
          this.PositionTabGrid = (PositionTab) target;
          break;
        case 2:
          this.RegistrationPointSelector = (RegistrationPointSelector) target;
          break;
        case 3:
          this.TranslateXEditor = (PropertyContainer) target;
          break;
        case 4:
          this.TranslateYEditor = (PropertyContainer) target;
          break;
        case 5:
          this.TranslateZEditor = (PropertyContainer) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
