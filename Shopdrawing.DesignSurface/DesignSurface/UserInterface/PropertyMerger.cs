// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.UserInterface.PropertyMerger
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.DesignSurface.ViewModel.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Expression.DesignSurface.UserInterface
{
  internal class PropertyMerger
  {
    public static event EventHandler PropertiesUpdated;

    public static IList<TargetedReferenceStep> GetMergedProperties(IEnumerable<SceneNode> selectedSceneNodes)
    {
      if (selectedSceneNodes == null || !Enumerable.Any<SceneNode>(selectedSceneNodes))
        return (IList<TargetedReferenceStep>) new List<TargetedReferenceStep>();
      return (IList<TargetedReferenceStep>) PropertyMerger.GetPropertiesFromSelection(selectedSceneNodes);
    }

    public static List<TargetedReferenceStep> GetPropertiesFromSelection(IEnumerable<SceneNode> selectedSceneNodes)
    {
      List<TargetedReferenceStep> properties = new List<TargetedReferenceStep>();
      ProjectXamlContext projectXamlContext = (ProjectXamlContext) Enumerable.First<SceneNode>(selectedSceneNodes).ProjectContext;
      PropertyMerger.GetMergedPropertiesForSelection(selectedSceneNodes, properties);
      PropertyMerger.ReferenceStepComparer referenceStepComparer = new PropertyMerger.ReferenceStepComparer();
      int count = properties.Count;
      using (IAttachedPropertiesAccessToken propertiesAccessToken = projectXamlContext.AttachedProperties.Access())
      {
        if (!projectXamlContext.AttachedProperties.IsFinished)
          projectXamlContext.AttachedProperties.FinishedScanning += new EventHandler(PropertyMerger.OnAttachedPropertiesFinishedScanning);
        foreach (IAttachedPropertyMetadata propertyMetadata in propertiesAccessToken.AllAttachedProperties())
        {
          IType type = projectXamlContext.GetType(propertyMetadata.OwnerType);
          if (type != null)
          {
            DependencyPropertyReferenceStep propertyReferenceStep = type.GetMember(MemberType.AttachedProperty, propertyMetadata.Name, MemberAccessTypes.Public) as DependencyPropertyReferenceStep;
            if (propertyReferenceStep != null)
            {
              TargetedReferenceStep targetedReferenceStep = new TargetedReferenceStep((ReferenceStep) propertyReferenceStep, propertyReferenceStep.DeclaringType);
              if (properties.BinarySearch(0, count, targetedReferenceStep, (IComparer<TargetedReferenceStep>) referenceStepComparer) < 0)
                properties.Add(targetedReferenceStep);
            }
          }
        }
      }
      return properties;
    }

    private static void OnAttachedPropertiesFinishedScanning(object sender, EventArgs e)
    {
      ((IAttachedPropertiesMetadata) sender).FinishedScanning -= new EventHandler(PropertyMerger.OnAttachedPropertiesFinishedScanning);
      if (PropertyMerger.PropertiesUpdated == null)
        return;
      PropertyMerger.PropertiesUpdated((object) null, EventArgs.Empty);
    }

    private static int ReferenceStepCompare(TargetedReferenceStep r1, TargetedReferenceStep r2)
    {
      if (r1 == r2)
        return 0;
      DependencyPropertyReferenceStep propertyReferenceStep1 = r1.ReferenceStep as DependencyPropertyReferenceStep;
      DependencyPropertyReferenceStep propertyReferenceStep2 = r2.ReferenceStep as DependencyPropertyReferenceStep;
      if (propertyReferenceStep1 != null && propertyReferenceStep2 != null && propertyReferenceStep1.DependencyProperty == propertyReferenceStep2.DependencyProperty)
        return 0;
      int num = string.Compare(r1.ReferenceStep.Name, r2.ReferenceStep.Name, StringComparison.Ordinal);
      if (num != 0)
        return num;
      if (r1.ReferenceStep.PropertyType.Equals((object) r2.ReferenceStep.PropertyType) && r1.ReferenceStep.MemberType == r2.ReferenceStep.MemberType)
        return 0;
      return string.Compare(r1.ReferenceStep.DeclaringType.UniqueName, r2.ReferenceStep.DeclaringType.UniqueName, StringComparison.Ordinal);
    }

    private static void GetMergedPropertiesForSelection(IEnumerable<SceneNode> selectedSceneNodes, List<TargetedReferenceStep> properties)
    {
      PropertyMerger.ReferenceStepComparer referenceStepComparer = new PropertyMerger.ReferenceStepComparer();
      HashSet<IType> hashSet = new HashSet<IType>();
      bool flag = true;
      foreach (SceneNode sceneNode in selectedSceneNodes)
      {
        IType typeFromSceneNode = PropertyMerger.GetTypeFromSceneNode(sceneNode);
        if (hashSet.Add(typeFromSceneNode))
        {
          List<TargetedReferenceStep> list = new List<TargetedReferenceStep>();
          foreach (ReferenceStep referenceStep in PropertyMerger.GetPropertiesForSceneNode(sceneNode))
            list.Add(new TargetedReferenceStep(referenceStep, typeFromSceneNode));
          list.Sort((IComparer<TargetedReferenceStep>) referenceStepComparer);
          if (flag)
          {
            properties.AddRange((IEnumerable<TargetedReferenceStep>) list);
            flag = false;
          }
          else
          {
            for (int index = properties.Count - 1; index >= 0; --index)
            {
              if (list.BinarySearch(properties[index], (IComparer<TargetedReferenceStep>) referenceStepComparer) < 0)
                properties.RemoveAt(index);
            }
          }
        }
      }
    }

    private static IEnumerable<ReferenceStep> GetPropertiesForSceneNode(SceneNode node)
    {
      TextRangeElement textRange = node as TextRangeElement;
      if (textRange != null)
      {
        foreach (IPropertyId propertyId in textRange.RangeProperties)
        {
          DependencyPropertyReferenceStep dp = node.ProjectContext.ResolveProperty(propertyId) as DependencyPropertyReferenceStep;
          if (dp != null)
            yield return (ReferenceStep) dp;
        }
      }
      else
      {
        foreach (IProperty property in ITypeExtensions.GetProperties(PropertyMerger.GetTypeFromSceneNode(node), MemberAccessTypes.Public, true))
        {
          ReferenceStep referenceStep = property as ReferenceStep;
          if (referenceStep != null)
          {
            DependencyPropertyReferenceStep dependencyPropertyReferenceStep = referenceStep as DependencyPropertyReferenceStep;
            if (dependencyPropertyReferenceStep == null || !dependencyPropertyReferenceStep.IsAttachable)
              yield return referenceStep;
          }
        }
      }
    }

    private static IType GetTypeFromSceneNode(SceneNode sceneNode)
    {
      StyleNode styleNode = sceneNode as StyleNode;
      return styleNode == null ? sceneNode.ProjectContext.GetType(sceneNode.TrueTargetType) : sceneNode.ProjectContext.GetType(styleNode.StyleTargetType);
    }

    private class ReferenceStepComparer : IComparer<TargetedReferenceStep>
    {
      public int Compare(TargetedReferenceStep r1, TargetedReferenceStep r2)
      {
        return PropertyMerger.ReferenceStepCompare(r1, r2);
      }
    }
  }
}
