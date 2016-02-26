// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.ViewModel.VisualStateManagerSceneNode
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.View;
using Microsoft.Expression.Project;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Expression.DesignSurface.ViewModel
{
  public class VisualStateManagerSceneNode : SceneNode
  {
    public static readonly IPropertyId VisualStateGroupsProperty = (IPropertyId) ProjectNeutralTypes.VisualStateManager.GetMember(MemberType.AttachedProperty, "VisualStateGroups", MemberAccessTypes.Public);
    public static readonly IPropertyId CustomVisualStateManagerProperty = (IPropertyId) ProjectNeutralTypes.VisualStateManager.GetMember(MemberType.AttachedProperty, "CustomVisualStateManager", MemberAccessTypes.Public);
    public static readonly IPropertyId UseFluidLayoutProperty = (IPropertyId) ProjectNeutralTypes.ExtendedVisualStateManager.GetMember(MemberType.AttachedProperty, "UseFluidLayout", MemberAccessTypes.Public);
    public static readonly IPropertyId TransitionEffectProperty = (IPropertyId) ProjectNeutralTypes.ExtendedVisualStateManager.GetMember(MemberType.AttachedProperty, "TransitionEffect", MemberAccessTypes.Public);
    public static readonly VisualStateManagerSceneNode.ConcreteVisualStateManagerSceneNodeFactory Factory = new VisualStateManagerSceneNode.ConcreteVisualStateManagerSceneNodeFactory();

    public static string SketchFlowAnimationXamlDelimiter
    {
      get
      {
        return "_SketchFlowAnimation_";
      }
    }

    public static string SketchFlowAnimationHoldTimeStateName
    {
      get
      {
        return "holdtimes";
      }
    }

    public static IList<VisualStateGroupSceneNode> GetStateGroups(SceneNode ownerNode)
    {
      if (ownerNode.ViewModel != null && !ownerNode.ViewModel.ProjectContext.PlatformMetadata.IsNullType((ITypeId) ownerNode.ViewModel.ProjectContext.ResolveType(ProjectNeutralTypes.VisualStateManager)))
        return (IList<VisualStateGroupSceneNode>) new SceneNode.SceneNodeCollection<VisualStateGroupSceneNode>(ownerNode, VisualStateManagerSceneNode.VisualStateGroupsProperty);
      return (IList<VisualStateGroupSceneNode>) new List<VisualStateGroupSceneNode>();
    }

    public static VisualStateManagerSceneNode GetCustomVisualStateManager(SceneNode ownerNode)
    {
      return (VisualStateManagerSceneNode) ownerNode.GetLocalValueAsSceneNode(VisualStateManagerSceneNode.CustomVisualStateManagerProperty);
    }

    public static bool GetHasExtendedVisualStateManager(SceneNode ownerNode)
    {
      VisualStateManagerSceneNode visualStateManager = VisualStateManagerSceneNode.GetCustomVisualStateManager(ownerNode);
      if (visualStateManager != null)
        return ProjectNeutralTypes.ExtendedVisualStateManager.IsAssignableFrom((ITypeId) visualStateManager.Type);
      return false;
    }

    public static void SetHasExtendedVisualStateManager(SceneNode ownerNode, bool set)
    {
      ProjectContext projectContext = ownerNode.ProjectContext as ProjectContext;
      IAssemblyService assemblyService = ownerNode.DesignerContext.AssemblyService;
      if (set)
      {
        if (!VisualStateManagerSceneNode.EnsureExtendedAssemblyReferences((ITypeResolver) projectContext, assemblyService, ownerNode.DesignerContext.ViewUpdateManager))
          return;
        ownerNode.SetValueAsSceneNode(VisualStateManagerSceneNode.CustomVisualStateManagerProperty, ownerNode.ViewModel.CreateSceneNode(ProjectNeutralTypes.ExtendedVisualStateManager));
      }
      else
        ownerNode.ClearValue(VisualStateManagerSceneNode.CustomVisualStateManagerProperty);
    }

    public static bool EnsureExtendedAssemblyReferences(ITypeResolver typeResolver, IAssemblyService assemblyService, ViewUpdateManager viewUpdateManager)
    {
      ToolkitHelper.AddToolkitReferenceIfNeeded(typeResolver, viewUpdateManager);
      if (assemblyService != null)
      {
        string assemblyFullName1 = ((PlatformTypes) typeResolver.PlatformMetadata).InteractivityAssemblyFullName;
        string assemblyFullName2 = ((PlatformTypes) typeResolver.PlatformMetadata).InteractionsAssemblyFullName;
        Assembly assembly1 = assemblyService.ResolveLibraryAssembly(new AssemblyName(assemblyFullName1));
        if (assembly1 != (Assembly) null)
        {
          Assembly assembly2 = assemblyService.ResolveLibraryAssembly(new AssemblyName(assemblyFullName2));
          if (assembly2 != (Assembly) null)
          {
            bool flag = typeResolver.EnsureAssemblyReferenced(assembly1.Location) && typeResolver.EnsureAssemblyReferenced(assembly2.Location);
            if (flag)
              viewUpdateManager.RebuildPostponedViews();
            return flag;
          }
        }
      }
      return false;
    }

    public static bool CanSupportVisualStateManager(SceneNode editingContainer)
    {
      return editingContainer != null && (PlatformTypes.UserControl.IsAssignableFrom((ITypeId) editingContainer.Type) || PlatformTypes.ControlTemplate.IsAssignableFrom((ITypeId) editingContainer.Type) || !PlatformTypes.Style.IsAssignableFrom((ITypeId) editingContainer.Type) && (editingContainer.ProjectContext.IsCapabilitySet(PlatformCapability.VsmEverywhereByDefault) || VisualStateManagerSceneNode.CanSupportExtendedVisualStateManager(editingContainer)));
    }

    public static bool CanOnlySupportExtendedVisualStateManager(SceneNode editingContainer)
    {
      return !PlatformTypes.UserControl.IsAssignableFrom((ITypeId) editingContainer.Type) && !PlatformTypes.ControlTemplate.IsAssignableFrom((ITypeId) editingContainer.Type) && !editingContainer.ProjectContext.IsCapabilitySet(PlatformCapability.VsmEverywhereByDefault);
    }

    public static void UpdateHasExtendedVisualStateManager(SceneNode editingContainer)
    {
      SceneNode hostNode = VisualStateManagerSceneNode.GetHostNode(editingContainer);
      if (hostNode == null)
        return;
      IList<VisualStateGroupSceneNode> stateGroups = VisualStateManagerSceneNode.GetStateGroups(hostNode);
      bool flag1 = stateGroups.Count > 0;
      bool flag2 = false;
      if (flag1)
      {
        IProperty property1 = editingContainer.ProjectContext.ResolveProperty(VisualStateManagerSceneNode.UseFluidLayoutProperty);
        IProperty property2 = editingContainer.ProjectContext.ResolveProperty(VisualStateManagerSceneNode.TransitionEffectProperty);
        if (property1 != null)
        {
          foreach (VisualStateGroupSceneNode stateGroupSceneNode in (IEnumerable<VisualStateGroupSceneNode>) stateGroups)
          {
            if ((bool) stateGroupSceneNode.GetLocalOrDefaultValue((IPropertyId) property1))
            {
              flag2 = true;
              break;
            }
            if (property2 != null)
            {
              foreach (SceneNode sceneNode in (IEnumerable<VisualStateTransitionSceneNode>) stateGroupSceneNode.Transitions)
              {
                if (sceneNode.GetLocalOrDefaultValue((IPropertyId) property2) != null)
                {
                  flag2 = true;
                  break;
                }
              }
              if (flag2)
                break;
            }
          }
        }
      }
      bool set = flag2 || flag1 && VisualStateManagerSceneNode.CanOnlySupportExtendedVisualStateManager(editingContainer);
      if (VisualStateManagerSceneNode.GetHasExtendedVisualStateManager(hostNode) == set)
        return;
      VisualStateManagerSceneNode.SetHasExtendedVisualStateManager(hostNode, set);
    }

    public static void DeleteStateGroup(SceneNode ownerNode, VisualStateGroupSceneNode nodeToRemove)
    {
      IList<VisualStateGroupSceneNode> stateGroups = VisualStateManagerSceneNode.GetStateGroups(ownerNode);
      stateGroups.Remove(nodeToRemove);
      if (stateGroups.Count != 0)
        return;
      ownerNode.ClearLocalValue(VisualStateManagerSceneNode.VisualStateGroupsProperty);
    }

    public static VisualStateGroupSceneNode AddStateGroup(SceneNode ownerNode, SceneNode rootNode, string groupName)
    {
      ProjectContext projectContext = (ProjectContext) ownerNode.ProjectContext.GetService(typeof (ProjectContext));
      ToolkitHelper.AddToolkitReferenceIfNeeded((ITypeResolver) projectContext, ownerNode.DesignerContext.ViewUpdateManager);
      IType type = projectContext.ResolveType(ProjectNeutralTypes.VisualStateGroup);
      if (type == null || projectContext.PlatformMetadata.IsNullType((ITypeId) type))
        return (VisualStateGroupSceneNode) null;
      VisualStateGroupSceneNode stateGroupSceneNode = VisualStateGroupSceneNode.Factory.Instantiate(ownerNode.ViewModel);
      VisualStateManagerSceneNode.GetStateGroups(ownerNode).Add(stateGroupSceneNode);
      string validElementId = new SceneNodeIDHelper(ownerNode.ViewModel, rootNode).GetValidElementID((SceneNode) stateGroupSceneNode, groupName);
      stateGroupSceneNode.Name = validElementId;
      return stateGroupSceneNode;
    }

    public static void AddDefaultStates(SceneNode ownerNode, SceneNode rootNode, ITypeId controlTypeId)
    {
      IType controlType = ownerNode.ProjectContext.ResolveType(controlTypeId);
      IProjectContext projectContext = ownerNode.ProjectContext;
      if (projectContext != null && projectContext.IsCapabilitySet(PlatformCapability.VsmInToolkit) && (projectContext.IsCapabilitySet(PlatformCapability.SupportsVisualStateManager) && projectContext.PlatformMetadata.IsNullType((ITypeId) projectContext.ResolveType(ProjectNeutralTypes.VisualStateManager))))
      {
        IAssembly usingAssemblyName = ((PlatformTypes) projectContext.Platform.Metadata).GetPlatformAssemblyUsingAssemblyName(controlType.RuntimeAssembly);
        if (usingAssemblyName == null || !AssemblyHelper.IsPlatformAssembly(usingAssemblyName))
        {
          ToolkitProjectContext toolkitProjectContext = new ToolkitProjectContext(projectContext);
          if (ProjectAttributeHelper.GetDefaultStateRecords(controlType, (ITypeResolver) toolkitProjectContext).Count > 0)
            ToolkitHelper.AddToolkitReferenceIfNeeded((ITypeResolver) projectContext, ownerNode.DesignerContext.ViewUpdateManager);
        }
      }
      foreach (DefaultStateRecord defaultStateRecord in ProjectAttributeHelper.GetDefaultStateRecords(controlType, (ITypeResolver) ownerNode.ProjectContext.GetService(typeof (ProjectContext))))
      {
        IList<VisualStateGroupSceneNode> stateGroups = VisualStateManagerSceneNode.GetStateGroups(ownerNode);
        VisualStateGroupSceneNode stateGroupSceneNode1 = (VisualStateGroupSceneNode) null;
        foreach (VisualStateGroupSceneNode stateGroupSceneNode2 in (IEnumerable<VisualStateGroupSceneNode>) stateGroups)
        {
          if (stateGroupSceneNode2.Name == defaultStateRecord.GroupName)
            stateGroupSceneNode1 = stateGroupSceneNode2;
        }
        if (stateGroupSceneNode1 == null)
        {
          VisualStateManagerSceneNode.EnsureNameAvailable(rootNode, defaultStateRecord.GroupName);
          stateGroupSceneNode1 = VisualStateManagerSceneNode.AddStateGroup(ownerNode, rootNode, defaultStateRecord.GroupName);
          stateGroupSceneNode1.ShouldSerialize = false;
        }
        VisualStateSceneNode visualStateSceneNode1 = (VisualStateSceneNode) null;
        if (stateGroupSceneNode1 != null)
        {
          foreach (VisualStateSceneNode visualStateSceneNode2 in (IEnumerable<VisualStateSceneNode>) stateGroupSceneNode1.States)
          {
            if (visualStateSceneNode2.Name == defaultStateRecord.StateName)
              visualStateSceneNode1 = visualStateSceneNode2;
          }
          if (visualStateSceneNode1 == null)
          {
            VisualStateManagerSceneNode.EnsureNameAvailable(rootNode, defaultStateRecord.StateName);
            stateGroupSceneNode1.AddState(rootNode, defaultStateRecord.StateName);
          }
        }
      }
    }

    public static VisualStateGroupSceneNode FindGroupByName(SceneNode ownerNode, string name)
    {
      foreach (VisualStateGroupSceneNode stateGroupSceneNode in (IEnumerable<VisualStateGroupSceneNode>) VisualStateManagerSceneNode.GetStateGroups(ownerNode))
      {
        if (stateGroupSceneNode.Name == name)
          return stateGroupSceneNode;
      }
      return (VisualStateGroupSceneNode) null;
    }

    public static bool IsSupportedTransitionAnimationType(ITypeId animatedType)
    {
      if (!PlatformTypes.Double.Equals((object) animatedType) && !PlatformTypes.Color.Equals((object) animatedType))
        return PlatformTypes.Point.Equals((object) animatedType);
      return true;
    }

    public static SceneNode GetHostNode(SceneNode editingContainer)
    {
      SceneElement sceneElement = editingContainer as SceneElement;
      FrameworkTemplateElement frameworkTemplateElement = sceneElement as FrameworkTemplateElement;
      if (frameworkTemplateElement != null)
        return (SceneNode) frameworkTemplateElement.VisualTreeRoot;
      if (sceneElement != null && sceneElement.DefaultContent != null && sceneElement.DefaultContent.Count > 0)
        return sceneElement.DefaultContent[0];
      return (SceneNode) null;
    }

    public static void MoveStates(SceneElement source, SceneElement target)
    {
      if (source == null || target == null || source.ProjectContext.PlatformMetadata.IsNullType((ITypeId) source.ProjectContext.ResolveType(ProjectNeutralTypes.VisualStateManager)))
        return;
      IList<VisualStateGroupSceneNode> stateGroups = VisualStateManagerSceneNode.GetStateGroups((SceneNode) target);
      stateGroups.Clear();
      ISceneNodeCollection<SceneNode> collectionForProperty = source.GetCollectionForProperty(VisualStateManagerSceneNode.VisualStateGroupsProperty);
      List<VisualStateGroupSceneNode> list = new List<VisualStateGroupSceneNode>();
      foreach (SceneNode sceneNode in (IEnumerable<SceneNode>) collectionForProperty)
      {
        if (sceneNode is VisualStateGroupSceneNode)
          list.Add((VisualStateGroupSceneNode) VisualStateGroupSceneNode.Factory.Instantiate(target.ViewModel, sceneNode.DocumentNode.Clone(target.DocumentContext)));
      }
      collectionForProperty.Clear();
      foreach (VisualStateGroupSceneNode stateGroupSceneNode in list)
        stateGroups.Add(stateGroupSceneNode);
    }

    public static void EnsureNameAvailable(SceneNode rootNode, string name)
    {
      SceneNodeIDHelper sceneNodeIdHelper = new SceneNodeIDHelper(rootNode.ViewModel, rootNode);
      if (sceneNodeIdHelper.IsValidElementID(rootNode, name))
        return;
      DocumentNode node = sceneNodeIdHelper.FindNode(name);
      SceneElement sceneElement = rootNode.ViewModel.GetSceneNode(node) as SceneElement;
      if (sceneElement == null)
        return;
      sceneElement.Name = sceneNodeIdHelper.GetValidElementID(rootNode, name);
    }

    private static bool CanSupportExtendedVisualStateManager(SceneNode editingContainer)
    {
      if (editingContainer.ProjectContext.IsCapabilitySet(PlatformCapability.VsmInToolkit))
      {
        IType type = editingContainer.ProjectContext.ResolveType(ProjectNeutralTypes.VisualStateManager);
        if (!editingContainer.ProjectContext.PlatformMetadata.IsNullType((ITypeId) type) && !type.RuntimeAssembly.Version.ToString(4).Equals("3.5.40128.1", StringComparison.Ordinal))
          return false;
      }
      return BlendSdkHelper.IsSdkInstalled(editingContainer.ProjectContext.TargetFramework);
    }

    public class ConcreteVisualStateManagerSceneNodeFactory : SceneNode.ConcreteSceneNodeFactory
    {
      protected override SceneNode Instantiate()
      {
        return (SceneNode) new VisualStateManagerSceneNode();
      }

      public VisualStateManagerSceneNode Instantiate(SceneViewModel viewModel)
      {
        return (VisualStateManagerSceneNode) this.Instantiate(viewModel, ProjectNeutralTypes.VisualStateManager);
      }
    }
  }
}
