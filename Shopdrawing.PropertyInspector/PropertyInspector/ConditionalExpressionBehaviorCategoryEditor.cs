//// Decompiled with JetBrains decompiler
//// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.ConditionalExpressionBehaviorCategoryEditor
//// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
//// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
//// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

//using Microsoft.Expression.DesignModel.Metadata;
//using Microsoft.Expression.DesignSurface.ViewModel;
//using Microsoft.Windows.Design.PropertyEditing;
//using System;
//using System.Windows;

//namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
//{
//  public class ConditionalExpressionBehaviorCategoryEditor : CategoryEditor
//  {
//    private static readonly IPropertyId[] conditionalExpressionProperties = new IPropertyId[1]
//    {
//      ConditionalExpressionNode.ConditionsProperty
//    };
//    private string targetCategoryName;

//    public virtual DataTemplate EditorTemplate
//    {
//      get
//      {
//        DataTemplate dataTemplate = new DataTemplate();
//        dataTemplate.VisualTree = new FrameworkElementFactory(typeof (ConditionalExpressionBehaviorCategoryEditorControl));
//        return dataTemplate;
//      }
//    }

//    public virtual string TargetCategory
//    {
//      get
//      {
//        return this.targetCategoryName;
//      }
//    }

//    public ConditionalExpressionBehaviorCategoryEditor(string targetCategoryName)
//    {
//      this.\u002Ector();
//      this.targetCategoryName = targetCategoryName;
//    }

//    public static bool IsConditionalExpressionProperty(ReferenceStep referenceStep)
//    {
//      return Array.IndexOf<IPropertyId>(ConditionalExpressionBehaviorCategoryEditor.conditionalExpressionProperties, (IPropertyId) referenceStep) != -1;
//    }

//    public virtual object GetImage(Size desiredSize)
//    {
//      return null;
//    }

//    public virtual bool ConsumesProperty(PropertyEntry propertyEntry)
//    {
//      return ConditionalExpressionBehaviorCategoryEditor.IsConditionalExpressionProperty(((PropertyReferenceProperty) propertyEntry).Reference.LastStep);
//    }
//  }
//}
