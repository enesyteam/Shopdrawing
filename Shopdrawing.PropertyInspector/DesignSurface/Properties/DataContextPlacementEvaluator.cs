// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Properties.DataContextPlacementEvaluator
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.DesignSurface.ViewModel;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignSurface.Properties
{
  public static class DataContextPlacementEvaluator
  {
    public static DataContextInfo FindDataContextPlacement(SceneNode sceneNode, IProperty targetProperty, DataSourceInfo dataSourceInfo)
    {
      DataContextInfo dataContextInfo = new DataContextEvaluator().Evaluate(sceneNode, (IPropertyId) targetProperty, true);
      if (!dataContextInfo.IsValid)
        return (DataContextInfo) null;
      if (dataContextInfo.Owner != null)
      {
        dataContextInfo.DataSourceMatch = dataContextInfo.DataSource.CompareSources(dataSourceInfo);
        if (dataContextInfo.DataSourceMatch == DataSourceMatchCriteria.Ignore)
        {
          using (sceneNode.ViewModel.CreateEditTransaction("", true))
          {
            if (targetProperty != null)
              sceneNode.ClearLocalValue((IPropertyId) targetProperty);
            if (DataContextPlacementEvaluator.IsAnySourcelessBindings(sceneNode))
              return (DataContextInfo) null;
            SceneNode sourcelessBindings = DataContextPlacementEvaluator.FindHighestDataContextLocationWithoutSourcelessBindings(dataContextInfo.GetOwnerSceneNode(sceneNode.ViewModel), sceneNode);
            if (sourcelessBindings == null)
            {
              dataContextInfo.Owner = (DocumentCompositeNode) null;
              dataContextInfo.Property = (IProperty) null;
              dataContextInfo.DataSourceMatch = DataSourceMatchCriteria.Ignore;
            }
            else
            {
              dataContextInfo.Owner = (DocumentCompositeNode) sourcelessBindings.DocumentNode;
              dataContextInfo.Property = DataContextHelper.GetDataContextProperty(dataContextInfo.Owner.Type);
              dataContextInfo.DataSourceMatch = DataSourceMatchCriteria.Any;
            }
          }
        }
      }
      else
      {
        SceneNode highestDataContextHost = DataContextPlacementEvaluator.GetHighestDataContextHost(sceneNode);
        if (highestDataContextHost == null)
          return (DataContextInfo) null;
        dataContextInfo = new DataContextInfo();
        dataContextInfo.Owner = (DocumentCompositeNode) highestDataContextHost.DocumentNode;
        dataContextInfo.DataSourceMatch = DataSourceMatchCriteria.Any;
      }
      return dataContextInfo;
    }

    private static SceneNode FindHighestDataContextLocationWithoutSourcelessBindings(SceneNode upperBound, SceneNode lowerBound)
    {
      SceneNode sceneNode1 = (SceneNode) null;
      SceneNode childToSkip = lowerBound;
      for (SceneNode sceneNode2 = lowerBound; sceneNode2 != upperBound; sceneNode2 = sceneNode2.Parent)
      {
        if (sceneNode2 == null)
          return (SceneNode) null;
        if (DataContextPlacementEvaluator.IsNodeWithoutSourcelessBindings(sceneNode2, sceneNode2 != lowerBound, childToSkip))
        {
          if (DataContextHelper.GetDataContextProperty(sceneNode2.Type) != null)
          {
            sceneNode1 = sceneNode2;
            childToSkip = sceneNode2;
          }
          if (sceneNode2.DocumentNode.NameScope != null)
            break;
        }
        else
          break;
      }
      return sceneNode1;
    }

    private static bool IsNodeWithoutSourcelessBindings(SceneNode sceneNode, bool checkChildren, SceneNode childToSkip)
    {
      if (DataContextPlacementEvaluator.IsAnySourcelessBindings(sceneNode))
        return false;
      if (!checkChildren)
        return true;
      foreach (SceneNode sceneNode1 in sceneNode.GetAllContent())
      {
        if (sceneNode1 != childToSkip && !DataContextPlacementEvaluator.IsNodeWithoutSourcelessBindings(sceneNode1, true, (SceneNode) null))
          return false;
      }
      return true;
    }

    private static bool IsAnySourcelessBindings(SceneNode sceneNode)
    {
      if (sceneNode == null)
        return false;
      DocumentCompositeNode documentCompositeNode = sceneNode.DocumentNode as DocumentCompositeNode;
      if (documentCompositeNode == null)
        return false;
      IProperty dataContextProperty = DataContextHelper.GetDataContextProperty(documentCompositeNode.Type);
      foreach (IProperty property in (IEnumerable<IProperty>) documentCompositeNode.Properties.Keys)
      {
        if (property.MemberType != MemberType.DesignTimeProperty && property != dataContextProperty)
        {
          BindingSceneNode binding = sceneNode.GetBinding((IPropertyId) property);
          if (binding != null && !DataContextHelper.GetDataSourceInfoFromBinding((DocumentCompositeNode) binding.DocumentNode).HasSource)
            return true;
        }
      }
      return false;
    }

    private static SceneNode GetHighestDataContextHost(SceneNode sceneNode)
    {
      SceneNode sceneNode1 = (SceneNode) null;
      for (SceneNode sceneNode2 = sceneNode; sceneNode2 != null && (!DocumentNodeHelper.IsStyleOrTemplate(sceneNode2.Type) && !PlatformTypes.DictionaryEntry.IsAssignableFrom((ITypeId) sceneNode2.Type)); sceneNode2 = sceneNode2.Parent)
      {
        if (DataContextHelper.HasDataContextProperty(sceneNode2.Type) && (sceneNode1 == null || sceneNode2.Parent != null))
          sceneNode1 = sceneNode2;
      }
      return sceneNode1;
    }
  }
}
