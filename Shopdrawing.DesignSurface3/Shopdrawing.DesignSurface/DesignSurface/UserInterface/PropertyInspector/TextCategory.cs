// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TextCategory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.Text;
using Microsoft.Expression.Framework;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class TextCategory : SceneNodeCategory
  {
    private bool isTextRangeSelection;
    private bool supportsParagraphProperties;
    private bool supportsControlProperties;
    private bool supportsTextProperties;
    private bool supportsLineHeight;

    public bool IsTextRangeSelection
    {
      get
      {
        return this.isTextRangeSelection;
      }
      private set
      {
        this.SetIfDifferent(ref this.isTextRangeSelection, value, "IsTextRangeSelection");
      }
    }

    public bool SupportsParagraphProperties
    {
      get
      {
        return this.supportsParagraphProperties;
      }
      private set
      {
        this.SetIfDifferent(ref this.supportsParagraphProperties, value, "SupportsParagraphProperties");
      }
    }

    public bool SupportsControlProperties
    {
      get
      {
        return this.supportsControlProperties;
      }
      private set
      {
        this.SetIfDifferent(ref this.supportsControlProperties, value, "SupportsControlProperties");
      }
    }

    public bool SupportsTextProperties
    {
      get
      {
        return this.supportsTextProperties;
      }
      private set
      {
        this.SetIfDifferent(ref this.supportsTextProperties, value, "SupportsTextProperties");
      }
    }

    public bool SupportsLineHeight
    {
      get
      {
        return this.supportsLineHeight;
      }
      private set
      {
        this.SetIfDifferent(ref this.supportsLineHeight, value, "SupportsLineHeight");
      }
    }

    public TextCategory(string localizedName, IMessageLoggingService messageLogger)
      : base(CategoryLocalizationHelper.CategoryName.Text, localizedName, messageLogger)
    {
    }

    public override void OnSelectionChanged(SceneNode[] selectedObjects)
    {
      base.OnSelectionChanged(selectedObjects);
      bool flag1 = selectedObjects.Length > 0;
      bool flag2 = selectedObjects.Length > 0;
      bool flag3 = selectedObjects.Length > 0;
      bool flag4 = selectedObjects.Length > 0;
      bool flag5 = false;
      TextRangeElement textRangeElement;
      if (selectedObjects.Length == 1 && (textRangeElement = selectedObjects[0] as TextRangeElement) != null)
      {
        flag1 = textRangeElement.TextEditProxy.SupportsParagraphProperties && textRangeElement.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsBlockPaddingProperty);
        flag2 = true;
        flag3 = this.CanSetTextProperty((ITypeId) textRangeElement.TextEditProxy.TextSource.Type);
        flag4 = textRangeElement.TextEditProxy.SupportsRangeProperties && (PlatformTypes.TextBlock.IsAssignableFrom((ITypeId) textRangeElement.TextEditProxy.TextSource.Type) || textRangeElement.ProjectContext.IsCapabilitySet(PlatformCapability.SupportsBlockLineHeightProperty));
        flag5 = true;
      }
      else
      {
        foreach (SceneNode sceneNode in selectedObjects)
        {
          flag1 &= this.CanSetParagraphProperty(this.GetTargetType(sceneNode), sceneNode.ProjectContext);
          flag2 &= this.CanSetControlProperty(this.GetTargetType(sceneNode));
          flag3 &= this.CanSetTextProperty(this.GetTargetType(sceneNode));
          flag4 &= this.CanSetLineHeight(this.GetTargetType(sceneNode), sceneNode.ProjectContext);
        }
      }
      this.SupportsParagraphProperties = flag1;
      this.SupportsControlProperties = flag2;
      this.SupportsTextProperties = flag3;
      this.SupportsLineHeight = flag4;
      this.IsTextRangeSelection = flag5;
      this.OnPropertyChanged("Item[]");
    }

    private bool CanSetParagraphProperty(ITypeId type, IProjectContext projectContext)
    {
      if (!projectContext.IsCapabilitySet(PlatformCapability.SupportsBlockPaddingProperty))
        return false;
      if (!PlatformTypes.Paragraph.IsAssignableFrom(type) && !PlatformTypes.RichTextBox.IsAssignableFrom(type))
        return PlatformTypes.FlowDocumentScrollViewer.IsAssignableFrom(type);
      return true;
    }

    private bool CanSetTextProperty(ITypeId type)
    {
      if (!PlatformTypes.TextElement.IsAssignableFrom(type) && !PlatformTypes.TextBoxBase.IsAssignableFrom(type) && (!PlatformTypes.TextBlock.IsAssignableFrom(type) && !PlatformTypes.RichTextBox.IsAssignableFrom(type)) && !PlatformTypes.FlowDocumentScrollViewer.IsAssignableFrom(type))
        return PlatformTypes.AccessText.IsAssignableFrom(type);
      return true;
    }

    private bool CanSetControlProperty(ITypeId type)
    {
      if (!PlatformTypes.TextElement.IsAssignableFrom(type) && !PlatformTypes.TextBlock.IsAssignableFrom(type) && (!PlatformTypes.FlowDocumentScrollViewer.IsAssignableFrom(type) && !PlatformTypes.Control.IsAssignableFrom(type)) && !PlatformTypes.AccessText.IsAssignableFrom(type))
        return ProjectNeutralTypes.DataGridTextColumn.IsAssignableFrom(type);
      return true;
    }

    private bool CanSetLineHeight(ITypeId type, IProjectContext projectContext)
    {
      if (PlatformTypes.TextBlock.IsAssignableFrom(type))
        return true;
      if (!projectContext.IsCapabilitySet(PlatformCapability.SupportsBlockLineHeightProperty))
        return false;
      if (!PlatformTypes.FlowDocumentScrollViewer.IsAssignableFrom(type))
        return PlatformTypes.RichTextBox.IsAssignableFrom(type);
      return true;
    }

    private ITypeId GetTargetType(SceneNode sceneNode)
    {
      StyleNode styleNode = sceneNode as StyleNode;
      return styleNode == null ? (ITypeId) sceneNode.Type : (ITypeId) styleNode.StyleTargetTypeId;
    }

    private void SetIfDifferent(ref bool value, bool newValue, string propertyName)
    {
      if (value == newValue)
        return;
      value = newValue;
      this.OnPropertyChanged(propertyName);
    }
  }
}
