// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.Timeline.ChildPropertyTimelineItemFactory
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Windows.Design.PropertyEditing;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.UserInterface.Timeline
{
  internal static class ChildPropertyTimelineItemFactory
  {
    private static List<ChildPropertyTimelineItemFactory.ChildPropertyTimelineItemEntry> childPropertyTimelineItemFactory = new List<ChildPropertyTimelineItemFactory.ChildPropertyTimelineItemEntry>();

    static ChildPropertyTimelineItemFactory()
    {
      ChildPropertyTimelineItemFactory.childPropertyTimelineItemFactory.Add(new ChildPropertyTimelineItemFactory.ChildPropertyTimelineItemEntry(Base2DElement.EffectProperty, (ChildPropertyTimelineItemFactory.ChildPropertyTimelineItemCreator) ((timelineItemManager, key, elementTimelineItem, itemType) => (ChildPropertyTimelineItem) new EffectTimelineItem(timelineItemManager, key, elementTimelineItem, itemType)), true, true, ChildPropertyTimelineItemType.Effect));
      ChildPropertyTimelineItemFactory.childPropertyTimelineItemFactory.Add(new ChildPropertyTimelineItemFactory.ChildPropertyTimelineItemEntry(ModelVisual3DElement.ContentProperty, (ChildPropertyTimelineItemFactory.ChildPropertyTimelineItemCreator) ((timelineItemManager, key, elementTimelineItem, itemType) => (ChildPropertyTimelineItem) new ModelVisual3DContentTimelineItem(timelineItemManager, key, elementTimelineItem, itemType)), false, false, ChildPropertyTimelineItemType.Default));
    }

    public static ChildPropertyTimelineItem CreateChildPropertyTimelineItem(TimelineItemManager timelineItemManager, IProperty key, ElementTimelineItem elementTimelineItem)
    {
      ChildPropertyTimelineItem propertyTimelineItem = (ChildPropertyTimelineItem) null;
      foreach (ChildPropertyTimelineItemFactory.ChildPropertyTimelineItemEntry timelineItemEntry in ChildPropertyTimelineItemFactory.childPropertyTimelineItemFactory)
      {
        if (key.Equals((object) timelineItemEntry.property))
        {
          propertyTimelineItem = timelineItemEntry.creator(timelineItemManager, key, elementTimelineItem, timelineItemEntry.type);
          propertyTimelineItem.EnableSelection = timelineItemEntry.enableSelection;
          propertyTimelineItem.ExpandParentOnInsertion = timelineItemEntry.expandParentOnInsertion;
          break;
        }
      }
      if (propertyTimelineItem == null)
      {
        ReferenceStep referenceStep = key as ReferenceStep;
        bool isAlternateContent = referenceStep != null && (referenceStep.Attributes[typeof (AlternateContentPropertyAttribute)] != null || PlatformNeutralAttributeHelper.AttributeExists((IEnumerable) referenceStep.Attributes, PlatformTypes.AlternateContentPropertyAttribute));
        propertyTimelineItem = new ChildPropertyTimelineItem(timelineItemManager, key, elementTimelineItem, ChildPropertyTimelineItemType.Default, isAlternateContent);
      }
      return propertyTimelineItem;
    }

    private delegate ChildPropertyTimelineItem ChildPropertyTimelineItemCreator(TimelineItemManager timelineItemManager, IProperty key, ElementTimelineItem elementTimelineItem, ChildPropertyTimelineItemType itemType);

    private struct ChildPropertyTimelineItemEntry
    {
      internal IPropertyId property;
      internal ChildPropertyTimelineItemFactory.ChildPropertyTimelineItemCreator creator;
      internal bool enableSelection;
      internal ChildPropertyTimelineItemType type;
      internal bool expandParentOnInsertion;

      public ChildPropertyTimelineItemEntry(IPropertyId property, ChildPropertyTimelineItemFactory.ChildPropertyTimelineItemCreator creator, bool selectable, bool expandParentOnInsertion, ChildPropertyTimelineItemType type)
      {
        this.property = property;
        this.type = type;
        this.creator = creator;
        this.enableSelection = selectable;
        this.expandParentOnInsertion = expandParentOnInsertion;
      }
    }
  }
}
