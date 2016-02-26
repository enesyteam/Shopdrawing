// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.PropertyEditorTemplateLookup
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.MaterialEditor;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Framework.Controls;
using Microsoft.Expression.Framework.ValueEditors;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public static class PropertyEditorTemplateLookup
  {
    private static ResourceDictionary resources = Microsoft.Expression.DesignSurface.FileTable.GetResourceDictionary("UserInterface\\PropertyInspector\\ValueEditors.xaml");
    private static PropertyValueEditor objectEditorTemplate;

    public static DataTemplate ObjectEditorInlineEditorTemplate
    {
      get
      {
        return (DataTemplate) PropertyEditorTemplateLookup.resources[(object) "ObjectViewTemplate"];
      }
    }

    public static DataTemplate ObjectEditorExtendedEditorTemplate
    {
      get
      {
        return (DataTemplate) PropertyEditorTemplateLookup.resources[(object) "ObjectEditTemplate"];
      }
    }

    public static DataTemplate ElementPickerTemplate
    {
      get
      {
        return (DataTemplate) PropertyEditorTemplateLookup.resources[(object) "ElementPickerTemplate"];
      }
    }

    public static DataTemplate PropertyBindingPickerTemplate
    {
      get
      {
        return (DataTemplate) PropertyEditorTemplateLookup.resources[(object) "PropertyBindingPickerTemplate"];
      }
    }

    public static DataTemplate ElementPropertyBindingPickerTemplate
    {
      get
      {
        return (DataTemplate) PropertyEditorTemplateLookup.resources[(object) "ElementPropertyBindingPickerTemplate"];
      }
    }

    public static DataTemplate ElementBindingPickerTemplate
    {
      get
      {
        return (DataTemplate) PropertyEditorTemplateLookup.resources[(object) "ElementBindingPickerTemplate"];
      }
    }

    public static DataTemplate BehaviorElementPickerTemplate
    {
      get
      {
        return (DataTemplate) PropertyEditorTemplateLookup.resources[(object) "BehaviorElementPickerTemplate"];
      }
    }

    public static DataTemplate MediaElementPickerTemplate
    {
      get
      {
        return (DataTemplate) PropertyEditorTemplateLookup.resources[(object) "MediaElementPickerTemplate"];
      }
    }

    public static DataTemplate INavigateElementPickerTemplate
    {
      get
      {
        return (DataTemplate) PropertyEditorTemplateLookup.resources[(object) "INavigateElementPickerTemplate"];
      }
    }

    public static DataTemplate TimerTriggerTickEditorTemplate
    {
      get
      {
        return (DataTemplate) PropertyEditorTemplateLookup.resources[(object) "TimerTriggerTickTemplate"];
      }
    }

    public static DataTemplate DataStoreBindingObjectEditorTemplate
    {
      get
      {
        return (DataTemplate) PropertyEditorTemplateLookup.resources[(object) "DataStoreBindingObjectTemplate"];
      }
    }

    public static DataTemplate DataStoreBindingObjectPropertyNameTemplate
    {
      get
      {
        return (DataTemplate) PropertyEditorTemplateLookup.resources[(object) "DataStoreBindingObjectPropertyNameTemplate"];
      }
    }

    public static DataTemplate StringValueEditorTemplate
    {
      get
      {
        return (DataTemplate) PropertyEditorTemplateLookup.resources[(object) "StringViewTemplate"];
      }
    }

    public static DataTemplate StringViewOfObjectEditorTemplate
    {
      get
      {
        return (DataTemplate) PropertyEditorTemplateLookup.resources[(object) "StringViewOfObjectTemplate"];
      }
    }

    private static PropertyValueEditor ObjectEditorTemplate
    {
      get
      {
        if (PropertyEditorTemplateLookup.objectEditorTemplate == null)
          PropertyEditorTemplateLookup.objectEditorTemplate = (PropertyValueEditor) new ExtendedPropertyValueEditor(PropertyEditorTemplateLookup.ObjectEditorExtendedEditorTemplate, PropertyEditorTemplateLookup.ObjectEditorInlineEditorTemplate);
        return PropertyEditorTemplateLookup.objectEditorTemplate;
      }
    }

    private static PropertyValueEditor GetPropertyEditorTemplate(IProjectContext projectContext, DesignerContext designerContext, IType type)
    {
      if (PlatformTypes.Double.IsAssignableFrom((ITypeId) type) || PlatformTypes.Double.IsAssignableFrom((ITypeId) type.NullableType))
        return new PropertyValueEditor((DataTemplate) PropertyEditorTemplateLookup.resources[(object) "DoubleViewTemplate"]);
      if (PlatformTypes.Int32.IsAssignableFrom((ITypeId) type))
        return new PropertyValueEditor((DataTemplate) PropertyEditorTemplateLookup.resources[(object) "IntViewTemplate"]);
      if (PlatformTypes.UInt32.IsAssignableFrom((ITypeId) type))
        return new PropertyValueEditor((DataTemplate) PropertyEditorTemplateLookup.resources[(object) "UIntViewTemplate"]);
      if (PlatformTypes.Byte.IsAssignableFrom((ITypeId) type))
        return new PropertyValueEditor((DataTemplate) PropertyEditorTemplateLookup.resources[(object) "ByteViewTemplate"]);
      if (PlatformTypes.Int16.IsAssignableFrom((ITypeId) type))
        return new PropertyValueEditor((DataTemplate) PropertyEditorTemplateLookup.resources[(object) "ShortViewTemplate"]);
      if (PlatformTypes.UInt16.IsAssignableFrom((ITypeId) type))
        return new PropertyValueEditor((DataTemplate) PropertyEditorTemplateLookup.resources[(object) "UShortViewTemplate"]);
      if (PlatformTypes.Single.IsAssignableFrom((ITypeId) type))
        return new PropertyValueEditor((DataTemplate) PropertyEditorTemplateLookup.resources[(object) "FloatViewTemplate"]);
      if (PlatformTypes.TextAlignment.IsAssignableFrom((ITypeId) type))
      {
        DataTemplate inlineEditorTemplate = new DataTemplate();
        inlineEditorTemplate.VisualTree = new FrameworkElementFactory(typeof (TextAlignmentValueEditor));
        return new PropertyValueEditor(inlineEditorTemplate);
      }
      if (PlatformTypes.Enum.IsAssignableFrom((ITypeId) type))
        return new PropertyValueEditor(!EnumsThatUseButtons.ShouldUseButtonsForEnum((ITypeId) type) ? (!EnumsThatUseButtons.ShouldUseToggleButtonsForEnum((ITypeId) type) ? (DataTemplate) PropertyEditorTemplateLookup.resources[(object) "ChoiceComboViewTemplate"] : (DataTemplate) PropertyEditorTemplateLookup.resources[(object) "ChoiceToggleButtonsViewTemplate"]) : (DataTemplate) PropertyEditorTemplateLookup.resources[(object) "ChoiceButtonsViewTemplate"]);
      if (PlatformTypes.Boolean.IsAssignableFrom((ITypeId) type) || PlatformTypes.Boolean.IsAssignableFrom((ITypeId) type.NullableType))
        return new PropertyValueEditor((DataTemplate) PropertyEditorTemplateLookup.resources[(object) "BoolViewTemplate"]);
      if (PlatformTypes.String.IsAssignableFrom((ITypeId) type))
        return new PropertyValueEditor((DataTemplate) PropertyEditorTemplateLookup.resources[(object) "StringViewTemplate"]);
      if (PlatformTypes.Vector3D.IsAssignableFrom((ITypeId) type))
        return new PropertyValueEditor((DataTemplate) PropertyEditorTemplateLookup.resources[(object) "Vector3DEditorTemplate"]);
      if (PlatformTypes.Point3D.IsAssignableFrom((ITypeId) type))
        return new PropertyValueEditor((DataTemplate) PropertyEditorTemplateLookup.resources[(object) "Point3DEditorTemplate"]);
      if (PlatformTypes.Point.IsAssignableFrom((ITypeId) type) || PlatformTypes.Vector.IsAssignableFrom((ITypeId) type))
        return new PropertyValueEditor((DataTemplate) PropertyEditorTemplateLookup.resources[(object) "Point2DEditorTemplate"]);
      if (PlatformTypes.Color.IsAssignableFrom((ITypeId) type) || PlatformTypes.Color.IsAssignableFrom((ITypeId) type.NullableType))
      {
        DataTemplate inlineEditorTemplate = (DataTemplate) PropertyEditorTemplateLookup.resources[(object) "ColorViewTemplate"];
        return (PropertyValueEditor) new ExtendedPropertyValueEditor((DataTemplate) PropertyEditorTemplateLookup.resources[(object) "ColorExtendedTemplate"], inlineEditorTemplate);
      }
      if (PlatformTypes.Brush.IsAssignableFrom((ITypeId) type))
      {
        DataTemplate inlineEditorTemplate = (DataTemplate) PropertyEditorTemplateLookup.resources[(object) "BrushViewTemplate"];
        return (PropertyValueEditor) new ExtendedPropertyValueEditor((DataTemplate) PropertyEditorTemplateLookup.resources[(object) "BrushEditorTemplate"], inlineEditorTemplate);
      }
      if (PlatformTypes.Material.IsAssignableFrom((ITypeId) type))
      {
        DataTemplate extendedEditorTemplate = new DataTemplate();
        extendedEditorTemplate.VisualTree = new FrameworkElementFactory(typeof (MaterialEditor));
        DataTemplate inlineEditorTemplate = (DataTemplate) PropertyEditorTemplateLookup.resources[(object) "MaterialViewTemplate"];
        return (PropertyValueEditor) new ExtendedPropertyValueEditor(extendedEditorTemplate, inlineEditorTemplate);
      }
      if (PlatformTypes.Transform.IsAssignableFrom((ITypeId) type) || PlatformTypes.Transform3D.IsAssignableFrom((ITypeId) type))
        return new PropertyValueEditor((DataTemplate) PropertyEditorTemplateLookup.resources[(object) "TransformEditorTemplate"]);
      if (PlatformTypes.Projection.IsAssignableFrom((ITypeId) type))
        return new PropertyValueEditor((DataTemplate) PropertyEditorTemplateLookup.resources[(object) "ProjectionEditorTemplate"]);
      if (PlatformTypes.Thickness.IsAssignableFrom((ITypeId) type))
      {
        DataTemplate inlineEditorTemplate = new DataTemplate();
        inlineEditorTemplate.VisualTree = new FrameworkElementFactory(typeof (ThicknessEditor));
        return new PropertyValueEditor(inlineEditorTemplate);
      }
      if (PlatformTypes.GridLength.IsAssignableFrom((ITypeId) type))
      {
        DataTemplate inlineEditorTemplate = new DataTemplate();
        inlineEditorTemplate.VisualTree = new FrameworkElementFactory(typeof (GridLengthEditor));
        return new PropertyValueEditor(inlineEditorTemplate);
      }
      if (PlatformTypes.FontFamily.IsAssignableFrom((ITypeId) type))
      {
        DataTemplate inlineEditorTemplate = new DataTemplate();
        inlineEditorTemplate.VisualTree = new FrameworkElementFactory(typeof (FontFamilyValueEditor));
        return new PropertyValueEditor(inlineEditorTemplate);
      }
      if (PlatformTypes.DrawingImage.IsAssignableFrom((ITypeId) type))
        return new PropertyValueEditor((DataTemplate) PropertyEditorTemplateLookup.resources[(object) "DrawingImageTemplate"]);
      if (PlatformTypes.KeySpline.IsAssignableFrom((ITypeId) type))
        return new PropertyValueEditor((DataTemplate) PropertyEditorTemplateLookup.resources[(object) "KeySplineEditorTemplate"]);
      if (PlatformTypes.IEasingFunction.IsAssignableFrom((ITypeId) type))
        return new PropertyValueEditor((DataTemplate) PropertyEditorTemplateLookup.resources[(object) "EasingFunctionEditorTemplate"]);
      if (ProjectNeutralTypes.TransitionEffect.IsAssignableFrom((ITypeId) type))
        return new PropertyValueEditor((DataTemplate) PropertyEditorTemplateLookup.resources[(object) "TransitionEffectEditorTemplate"]);
      if (PlatformTypes.RepeatBehavior.IsAssignableFrom((ITypeId) type))
        return new PropertyValueEditor((DataTemplate) PropertyEditorTemplateLookup.resources[(object) "RepeatBehaviorEditorTemplate"]);
      if (ProjectNeutralTypes.GeometryEffect.IsAssignableFrom((ITypeId) type))
        return new PropertyValueEditor((DataTemplate) PropertyEditorTemplateLookup.resources[(object) "GeometryEffectEditorTemplate"]);
      if (ProjectNeutralTypes.LayoutPathCollection.IsAssignableFrom((ITypeId) type))
        return new PropertyValueEditor((DataTemplate) PropertyEditorTemplateLookup.resources[(object) "LayoutPathCollectionEditorTemplate"]);
      if (PlatformTypes.DoubleCollection.IsAssignableFrom((ITypeId) type))
        return (PropertyValueEditor) null;
      if (PlatformTypes.PointCollection.IsAssignableFrom((ITypeId) type))
        return (PropertyValueEditor) null;
      if (PropertyEditorTemplateLookup.ShouldUseCollectionEditor((ITypeId) type))
      {
        DataTemplate inlineEditorTemplate = (DataTemplate) PropertyEditorTemplateLookup.resources[(object) "CollectionDialogInlineTemplate"];
        return (PropertyValueEditor) new PropertyEditorTemplateLookup.CollectionDialogPropertyValueEditor((DataTemplate) PropertyEditorTemplateLookup.resources[(object) "CollectionDialogEditorTemplate"], inlineEditorTemplate, designerContext.MessageDisplayService);
      }
      if (PropertyEditorTemplateLookup.ShouldUseObjectEditor(type))
        return PropertyEditorTemplateLookup.ObjectEditorTemplate;
      return (PropertyValueEditor) null;
    }

    public static PropertyValueEditor GetPropertyEditorTemplate(PropertyReferenceProperty property)
    {
      bool flag = false;
      IProjectContext projectContext = property.ObjectSet.ProjectContext;
      PropertyValueEditor propertyValueEditor = PropertyEditorTemplateLookup.GetPropertyEditorTemplate(projectContext, property.ObjectSet.DesignerContext, property.PropertyTypeId);
      if (propertyValueEditor == PropertyEditorTemplateLookup.ObjectEditorTemplate && property.PropertyValue.CanConvertFromString)
      {
        flag = true;
        propertyValueEditor = (PropertyValueEditor) null;
      }
      SceneNodeProperty sceneNodeProperty = property as SceneNodeProperty;
      if (propertyValueEditor == null)
      {
        if (property.Converter != null && property.Converter.GetPropertiesSupported())
          return PropertyEditorTemplateLookup.ObjectEditorTemplate;
        if (sceneNodeProperty != null && UriEditor.IsPropertySupportedOnType((PropertyReferenceProperty) sceneNodeProperty, sceneNodeProperty.SceneNodeObjectSet.ObjectType))
        {
          DataTemplate inlineEditorTemplate = new DataTemplate();
          inlineEditorTemplate.VisualTree = new FrameworkElementFactory(typeof (UriEditor));
          propertyValueEditor = new PropertyValueEditor(inlineEditorTemplate);
        }
      }
      if (propertyValueEditor == null && !flag)
      {
        IType computedValueTypeId = property.ComputedValueTypeId;
        if (computedValueTypeId != null)
        {
          propertyValueEditor = PropertyEditorTemplateLookup.GetPropertyEditorTemplate(projectContext, property.ObjectSet.DesignerContext, computedValueTypeId);
          if (propertyValueEditor is PropertyEditorTemplateLookup.CollectionDialogPropertyValueEditor)
            propertyValueEditor = (PropertyValueEditor) null;
        }
      }
      if (property.IsReadOnly && !(propertyValueEditor is PropertyEditorTemplateLookup.CollectionDialogPropertyValueEditor) && !ProjectNeutralTypes.LayoutPathCollection.IsAssignableFrom((ITypeId) property.PropertyTypeId))
        return (PropertyValueEditor) null;
      if (sceneNodeProperty != null && propertyValueEditor is PropertyEditorTemplateLookup.CollectionDialogPropertyValueEditor)
      {
        bool isMixed;
        DocumentNode valueAsDocumentNode = sceneNodeProperty.GetLocalValueAsDocumentNode(true, out isMixed);
        if (valueAsDocumentNode != null && valueAsDocumentNode is DocumentPrimitiveNode)
          return (PropertyValueEditor) null;
        if (property.PropertyTypeId.IsArray && !property.PropertyTypeId.PlatformMetadata.ResolveType(PlatformTypes.ArrayExtension).IsResolvable)
          return (PropertyValueEditor) null;
      }
      return propertyValueEditor;
    }

    private static bool ShouldUseObjectEditor(IType type)
    {
      TypeConverter typeConverter = Microsoft.Expression.DesignModel.Metadata.MetadataStore.GetTypeConverter(type.RuntimeType);
      return typeConverter == null || !typeConverter.CanConvertFrom(typeof (string)) && typeConverter.GetPropertiesSupported();
    }

    private static bool ShouldUseCollectionEditor(ITypeId type)
    {
      return PlatformTypes.IList.IsAssignableFrom(type);
    }

    private class CollectionDialogPropertyValueEditor : DialogPropertyValueEditor
    {
      private IMessageDisplayService messageDisplayService;
      private Dialog activeDialog;
      private DataTemplate template;

      public CollectionDialogPropertyValueEditor(DataTemplate dialogEditorTemplate, DataTemplate inlineEditorTemplate, IMessageDisplayService messageDisplayService)
        : base((DataTemplate) null, inlineEditorTemplate)
      {
        this.template = dialogEditorTemplate;
        this.messageDisplayService = messageDisplayService;
      }

      public override void ShowDialog(Microsoft.Windows.Design.PropertyEditing.PropertyValue propertyValue, IInputElement commandSource)
      {
        this.activeDialog = (Dialog) new DialogValueEditorHost(propertyValue, this.template);
        ValueEditorUtils.SetHandlesCommitKeys((DependencyObject) this.activeDialog, true);
        Type genericCollectionType = CollectionAdapterDescription.GetGenericCollectionType(propertyValue.ParentProperty.PropertyType);
        string str = "";
        if (genericCollectionType != (Type) null)
          str = genericCollectionType.Name + " ";
        this.activeDialog.Title = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.CollectionEditorDialogTitle, new object[2]
        {
          (object) str,
          (object) propertyValue.ParentProperty.PropertyName
        });
        this.activeDialog.ResizeMode = ResizeMode.CanResize;
        this.activeDialog.Width = 600.0;
        this.activeDialog.Height = 600.0;
        this.activeDialog.SizeToContent = SizeToContent.Manual;
        this.activeDialog.CommandBindings.Add(new CommandBinding((ICommand) PropertyValueEditorCommands.FinishEditing, new ExecutedRoutedEventHandler(this.OnPropertyValueFinishEditingCommand)));
        PropertyValueEditorCommands.BeginTransaction.Execute((object) new PropertyTransactionParameters()
        {
          TransactionDescription = string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.PropertyChangeUndoDescription, new object[1]
          {
            (object) propertyValue.ParentProperty.PropertyName
          }),
          TransactionType = SceneEditTransactionType.Normal
        }, commandSource);
        bool? nullable = new bool?();
        try
        {
          nullable = this.activeDialog.ShowDialog();
        }
        catch
        {
          this.messageDisplayService.ShowError(StringTable.CollectionEditorErrorMessage);
          this.activeDialog.Close();
        }
        if (nullable.HasValue && nullable.Value)
          PropertyValueEditorCommands.CommitTransaction.Execute((object) null, commandSource);
        else
          PropertyValueEditorCommands.AbortTransaction.Execute((object) null, commandSource);
      }

      private void OnPropertyValueFinishEditingCommand(object sender, ExecutedRoutedEventArgs eventArgs)
      {
        Keyboard.Focus(LogicalTreeHelper.FindLogicalNode((DependencyObject) this.activeDialog, "AcceptButton") as IInputElement);
      }
    }
  }
}
