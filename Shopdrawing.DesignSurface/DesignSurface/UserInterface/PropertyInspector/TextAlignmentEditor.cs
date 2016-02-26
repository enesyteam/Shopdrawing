// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TextAlignmentEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.Framework.ValueEditors;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
  public sealed class TextAlignmentEditor : Decorator, IComponentConnector
  {
    public static readonly DependencyProperty TextAlignmentsProperty = DependencyProperty.Register("TextAlignments", typeof (IEnumerable), typeof (TextAlignmentEditor), new PropertyMetadata((object) new List<object>()));
    internal TextAlignmentEditor Root;
    internal ChoiceEditor TextAlignmentChoiceEditor;
    private bool _contentLoaded;

    public IEnumerable TextAlignments
    {
      get
      {
        return (IEnumerable) this.GetValue(TextAlignmentEditor.TextAlignmentsProperty);
      }
      set
      {
        this.SetValue(TextAlignmentEditor.TextAlignmentsProperty, (object) value);
      }
    }

    public TextAlignmentEditor()
    {
      this.InitializeComponent();
    }

    [DebuggerNonUserCode]
    public void InitializeComponent()
    {
      if (this._contentLoaded)
        return;
      this._contentLoaded = true;
      Application.LoadComponent((object) this, new Uri("/Microsoft.Expression.DesignSurface;component/userinterface/propertyinspector/complexvalueeditors/textalignmenteditor.xaml", UriKind.Relative));
    }

    [DebuggerNonUserCode]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
      switch (connectionId)
      {
        case 1:
          this.Root = (TextAlignmentEditor) target;
          break;
        case 2:
          this.TextAlignmentChoiceEditor = (ChoiceEditor) target;
          break;
        default:
          this._contentLoaded = true;
          break;
      }
    }
  }
}
