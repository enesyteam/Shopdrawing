// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector.TriggerCategory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Framework;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.UserInterface.PropertyInspector
{
  public class TriggerCategory : SceneNodeCategory
  {
    private IType triggerType;
    private SceneNode[] selectedObjects;
    private TriggerObjectSet objectSet;

    public IType TriggerType
    {
      get
      {
        return this.triggerType;
      }
      set
      {
        this.triggerType = value;
        this.OnPropertyChanged("TriggerType");
        this.OnPropertyChanged("TriggerTypeName");
        this.OnPropertyChanged("CategoryHelpContext");
      }
    }

    public string TriggerTypeName
    {
      get
      {
        return string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.BehaviorTriggerTypeFormatString, new object[1]
        {
          this.triggerType.Name
        });
      }
    }

    public override CategoryHelpContext CategoryHelpContext
    {
      get
      {
        return new CategoryHelpContext(this.TriggerType, string.Format((IFormatProvider) CultureInfo.CurrentCulture, StringTable.DefaultCategoryHelpToolTipFormat, new object[1]
        {
          this.TriggerType.Name
        }));
      }
    }

    public override bool IsEmpty
    {
      get
      {
        return this.objectSet == null;
      }
    }

    public TriggerCategory(string localizedName, IMessageLoggingService messageLogger)
      : base(CategoryLocalizationHelper.CategoryName.Triggers, localizedName, messageLogger)
    {
    }

    public override void OnSelectionChanged(SceneNode[] selectedObjects)
    {
      base.OnSelectionChanged(selectedObjects);
      if (this.objectSet != null)
      {
        this.objectSet.Dispose();
        this.objectSet = (TriggerObjectSet) null;
      }
      this.selectedObjects = selectedObjects;
      if (selectedObjects.Length > 0)
      {
        SceneNode sceneNode = selectedObjects[0];
        if (sceneNode != null && ProjectNeutralTypes.BehaviorTriggerAction.IsAssignableFrom((ITypeId) sceneNode.Type))
        {
          BehaviorTriggerBaseNode behaviorTriggerBaseNode = sceneNode.Parent as BehaviorTriggerBaseNode;
          if (behaviorTriggerBaseNode != null)
          {
            this.objectSet = new TriggerObjectSet((BehaviorTriggerActionNode) sceneNode);
            this.TriggerType = behaviorTriggerBaseNode.Type;
            List<TargetedReferenceStep> list = new List<TargetedReferenceStep>();
            foreach (IProperty property in ITypeExtensions.GetProperties(behaviorTriggerBaseNode.Type, MemberAccessTypes.Public, true))
            {
              ReferenceStep referenceStep = property as ReferenceStep;
              if (referenceStep != null)
                list.Add(new TargetedReferenceStep(referenceStep, behaviorTriggerBaseNode.Type));
            }
            foreach (TargetedReferenceStep targetedReferenceStep in list)
            {
              if (PropertyInspectorModel.IsPropertyBrowsable(this.selectedObjects, targetedReferenceStep))
              {
                AttributeCollection attributes = targetedReferenceStep.Attributes;
                Attribute[] attributeArray = new Attribute[attributes.Count + 1];
                int index;
                for (index = 0; index < attributes.Count; ++index)
                  attributeArray[index] = attributes[index];
                attributeArray[index] = (Attribute) new CategoryAttribute(CategoryLocalizationHelper.GetLocalizedCategoryName(CategoryLocalizationHelper.CategoryName.Triggers));
                SceneNodeProperty property = (SceneNodeProperty) this.objectSet.CreateProperty(new PropertyReference(targetedReferenceStep.ReferenceStep), new AttributeCollection(attributeArray));
                if (!PlatformTypes.IList.IsAssignableFrom((ITypeId) property.PropertyTypeId))
                  this.AddProperty(property);
              }
            }
          }
        }
      }
      this.OnPropertyChanged("IsEmpty");
    }

    internal void OnTriggerTypeButtonClicked(object sender, RoutedEventArgs args)
    {
      SceneNode sceneNode1 = this.selectedObjects[0];
      Type runtimeType = sceneNode1.ProjectContext.ResolveType(ProjectNeutralTypes.BehaviorTriggerBase).RuntimeType;
      Type newClrObject = ClrNewObjectDialog.CreateNewClrObject(sceneNode1.ViewModel, runtimeType, true);
      if (!(newClrObject != (Type) null))
        return;
      BehaviorTriggerActionNode triggerActionNode = (BehaviorTriggerActionNode) sceneNode1;
      using (SceneEditTransaction editTransaction = triggerActionNode.ViewModel.CreateEditTransaction(StringTable.ChangeBehaviorTriggerTypeUndoDescription))
      {
        SceneNode sceneNode2 = triggerActionNode.ViewModel.CreateSceneNode(newClrObject);
        this.objectSet.ReparentActionAndCopyBehaviors(this.objectSet.FindExistingTriggerMatchingDocumentNode(sceneNode2.DocumentNode) ?? (BehaviorTriggerBaseNode) sceneNode2);
        editTransaction.Commit();
      }
    }

    public override void ApplyFilter(PropertyFilter filter)
    {
      base.ApplyFilter(filter);
      TriggerCategory triggerCategory1 = this;
      int num1 = triggerCategory1.BasicPropertyMatchesFilter | this.get_MatchesFilter() ? 1 : 0;
      triggerCategory1.BasicPropertyMatchesFilter = num1 != 0;
      TriggerCategory triggerCategory2 = this;
      int num2 = triggerCategory2.AdvancedPropertyMatchesFilter | this.get_MatchesFilter() ? 1 : 0;
      triggerCategory2.AdvancedPropertyMatchesFilter = num2 != 0;
    }

    public override bool MatchesPredicate(PropertyFilterPredicate predicate)
    {
      if (!predicate.Match("Trigger"))
        return predicate.Match("TriggerType");
      return true;
    }
  }
}
