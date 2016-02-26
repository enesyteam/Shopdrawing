// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TextAlignmentValueEditor
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.ViewModel.Text;
using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class TextAlignmentValueEditor : ContentControl
  {
    private TextAlignmentEditor textAlignmentEditor;

    public TextAlignmentValueEditor()
    {
      this.textAlignmentEditor = new TextAlignmentEditor();
      this.Content = (object) this.textAlignmentEditor;
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(this.FontFamilyValueEditor_DataContextChanged);
      this.Loaded += new RoutedEventHandler(this.FontFamilyValueEditor_Loaded);
    }

    private bool IsRichTextBoxRange(SceneNodeObjectSet sceneNodeObjectSet)
    {
      if (sceneNodeObjectSet.IsMultiSelection || sceneNodeObjectSet.Count != 1)
        return true;
      RichTextBoxRangeElement textBoxRangeElement = sceneNodeObjectSet.Objects[0] as RichTextBoxRangeElement;
      if (textBoxRangeElement == null)
        return false;
      return PlatformTypes.RichTextBox.IsAssignableFrom((ITypeId) textBoxRangeElement.TextEditProxy.TextSource.Type);
    }

    private IEnumerable GetTextAlignments(SceneNodeProperty sceneNodeProperty)
    {
      SceneNodeObjectSet sceneNodeObjectSet = sceneNodeProperty.SceneNodeObjectSet;
      bool flag = sceneNodeObjectSet.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsJustifyAlignmentEverywhere) || sceneNodeObjectSet.ObjectTypeId == null || PlatformTypes.RichTextBox.IsAssignableFrom((ITypeId) sceneNodeObjectSet.ObjectTypeId) || sceneNodeObjectSet.IsTextRange && this.IsRichTextBoxRange(sceneNodeObjectSet);
      IEnumerable source = (IEnumerable) sceneNodeProperty.StandardValues;
      if (!flag)
        source = (IEnumerable) Enumerable.Where<object>(Enumerable.Cast<object>(source), (Func<object, bool>) (standardValue => !standardValue.ToString().Equals("Justify", StringComparison.OrdinalIgnoreCase)));
      return source;
    }

    private void Rebuild()
    {
      SceneNodePropertyValue nodePropertyValue = this.DataContext as SceneNodePropertyValue;
      if (nodePropertyValue == null)
        return;
      this.textAlignmentEditor.TextAlignments = this.GetTextAlignments((SceneNodeProperty) nodePropertyValue.ParentProperty);
    }

    private void FontFamilyValueEditor_Loaded(object sender, RoutedEventArgs e)
    {
      this.Rebuild();
    }

    private void FontFamilyValueEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      if (!this.IsLoaded)
        return;
      this.Rebuild();
    }
  }
}
