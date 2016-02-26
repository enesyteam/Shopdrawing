// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.Tools.DataBindingDragDropAddTriggerHandler
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface.Documents;
using Microsoft.Expression.DesignSurface.Pipeline;
using Microsoft.Expression.DesignSurface.Tools.Assets;
using Microsoft.Expression.DesignSurface.UserInterface.DataPane;
using Microsoft.Expression.DesignSurface.ViewModel;
using Microsoft.Expression.Project;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows;

namespace Microsoft.Expression.DesignSurface.Tools
{
  internal class DataBindingDragDropAddTriggerHandler : DataBindingDragDropHandler
  {
    private static IPropertyId MethodNameProperty = (IPropertyId) ProjectNeutralTypes.CallMethodAction.GetMember(MemberType.LocalProperty, "MethodName", MemberAccessTypes.Public);
    private static IPropertyId TargetObjectProperty = (IPropertyId) ProjectNeutralTypes.CallMethodAction.GetMember(MemberType.LocalProperty, "TargetObject", MemberAccessTypes.Public);
    private static IPropertyId CommandProperty = (IPropertyId) ProjectNeutralTypes.InvokeCommandAction.GetMember(MemberType.LocalProperty, "Command", MemberAccessTypes.Public);

    public override bool CanHandle()
    {
      if (this.DragModel.DataSource.FindIndex((Predicate<DataSchemaNodePath>) (schemaPath =>
      {
        if (schemaPath.IsMethod)
          return true;
        if (schemaPath.IsProperty)
          return PlatformTypes.ICommand.IsAssignableFrom((ITypeId) schemaPath.Type);
        return false;
      })) < 0)
        return false;
      if (this.DragModel.DataSource.Count == 1)
      {
        DataSchemaNodePath primarySchemaNodePath = this.DragModel.DataSource.PrimarySchemaNodePath;
        if (primarySchemaNodePath.IsProperty)
        {
          BindingPropertyMatchInfo bindingPropertyInfo = BindingPropertyHelper.GetDefaultBindingPropertyInfo(this.DragModel.TargetNode, primarySchemaNodePath.Type);
          if (bindingPropertyInfo.Property != null && bindingPropertyInfo.Compatibility == BindingPropertyCompatibility.Assignable)
            return false;
        }
      }
      if (!this.UpdateDragModelInternal())
        this.DragModel.DropFlags = DataBindingDragDropFlags.None;
      return this.DragModel.DropFlags != DataBindingDragDropFlags.None;
    }

    private bool UpdateDragModelInternal()
    {
      if (!DataBindingDragDropAddTriggerHandler.IsEnabled((ITypeResolver) this.ProjectContext) || this.DragModel.InsertionPoint.Property != null || (this.DragModel.DataSource.Count > 1 || !this.UpdateRelativeSchemaPath()) || DataBindingModeModel.Instance.NormalizedMode == DataBindingMode.Default && this.DragModel.RelativeDropSchemaPath.CollectionDepth > 0)
        return false;
      if (PlatformTypes.FrameworkElement.IsAssignableFrom((ITypeId) this.DragModel.TargetNode.Type))
      {
        this.DragModel.DropFlags = DataBindingDragDropFlags.CreateElement;
        if (this.DragModel.RelativeDropSchemaPath.IsMethod)
        {
          this.DragModel.NewElementType = ProjectNeutralTypes.CallMethodAction;
          this.DragModel.NewElementProperty = DataBindingDragDropAddTriggerHandler.MethodNameProperty;
        }
        else
        {
          this.DragModel.NewElementType = ProjectNeutralTypes.InvokeCommandAction;
          this.DragModel.NewElementProperty = DataBindingDragDropAddTriggerHandler.CommandProperty;
        }
      }
      else if (ProjectNeutralTypes.CallMethodAction.IsAssignableFrom((ITypeId) this.DragModel.TargetNode.Type) && this.DragModel.RelativeDropSchemaPath.IsMethod)
      {
        this.DragModel.DropFlags = DataBindingDragDropFlags.SetBinding;
        this.DragModel.TargetProperty = this.ProjectContext.ResolveProperty(DataBindingDragDropAddTriggerHandler.MethodNameProperty);
      }
      else if (ProjectNeutralTypes.InvokeCommandAction.IsAssignableFrom((ITypeId) this.DragModel.TargetNode.Type) && !this.DragModel.RelativeDropSchemaPath.IsMethod)
      {
        this.DragModel.DropFlags = DataBindingDragDropFlags.SetBinding;
        this.DragModel.TargetProperty = this.ProjectContext.ResolveProperty(DataBindingDragDropAddTriggerHandler.CommandProperty);
      }
      this.UpdateTooltip();
      return true;
    }

    public override bool Handle(Point artboardSnappedDropPoint)
    {
      if (!this.ProjectContext.EnsureAssemblyReferenced("Microsoft.Expression.Interactions"))
        return false;
      this.DragModel.TargetNode.DesignerContext.ViewUpdateManager.RebuildPostponedViews();
      using (SceneEditTransaction editTransaction = this.DragModel.Document.CreateEditTransaction(StringTable.UndoUnitDragDropCreateDataboundControl))
      {
        if (DataBindingModeModel.Instance.NormalizedMode == DataBindingMode.Details)
        {
          this.GetReusableDetailsContainer(true, true);
          editTransaction.Update();
        }
        SceneNode actionNode;
        if (this.DragModel.CheckDropFlags(DataBindingDragDropFlags.CreateElement))
        {
          IType triggerType = this.ProjectContext.ResolveType(this.DragModel.NewElementType);
          if (this.ProjectContext.PlatformMetadata.IsNullType((ITypeId) triggerType))
            return false;
          actionNode = TriggerActionAsset.CreateTrigger(this.DragModel.TargetNode, triggerType);
          editTransaction.Update();
        }
        else
          actionNode = this.DragModel.TargetNode;
        if (actionNode == null)
          return false;
        if (!this.DragModel.DataSource.PrimarySchemaNodePath.IsMethod ? this.UpdateInvokeCommandAction(actionNode) : this.UpdateCallMethodAction(actionNode))
        {
          editTransaction.Commit();
          return true;
        }
      }
      return false;
    }

    private bool UpdateCallMethodAction(SceneNode actionNode)
    {
      if (!ProjectNeutralTypes.CallMethodAction.IsAssignableFrom((ITypeId) actionNode.Type))
        return false;
      DataSchemaNodePath primaryAbsolutePath = this.DragModel.DataSource.PrimaryAbsolutePath;
      DataSchemaNode node = primaryAbsolutePath.Node;
      DataSchemaNodePath bindingPath = new DataSchemaNodePath(primaryAbsolutePath.Schema, node.Parent);
      BindingSceneNode bindingSceneNode = this.DragModel.ViewModel.BindingEditor.CreateAndSetBindingOrData(actionNode, DataBindingDragDropAddTriggerHandler.TargetObjectProperty, bindingPath) as BindingSceneNode;
      if (bindingSceneNode == null)
        return false;
      bindingSceneNode.ClearLocalValue(BindingSceneNode.ModeProperty);
      actionNode.SetValue(DataBindingDragDropAddTriggerHandler.MethodNameProperty, (object) node.PathName);
      return true;
    }

    private bool UpdateInvokeCommandAction(SceneNode actionNode)
    {
      if (!ProjectNeutralTypes.InvokeCommandAction.IsAssignableFrom((ITypeId) actionNode.Type))
        return false;
      DataSchemaNodePath primaryAbsolutePath = this.DragModel.DataSource.PrimaryAbsolutePath;
      BindingSceneNode bindingSceneNode = this.DragModel.ViewModel.BindingEditor.CreateAndSetBindingOrData(actionNode, DataBindingDragDropAddTriggerHandler.CommandProperty, primaryAbsolutePath) as BindingSceneNode;
      if (bindingSceneNode == null)
        return false;
      bindingSceneNode.ClearLocalValue(BindingSceneNode.ModeProperty);
      return true;
    }

    private void UpdateTooltip()
    {
      DataSchemaNodePath primaryAbsolutePath = this.DragModel.DataSource.PrimaryAbsolutePath;
      if (primaryAbsolutePath.IsMethod)
      {
        string pathName = primaryAbsolutePath.Node.PathName;
        string str = new DataSchemaNodePath(primaryAbsolutePath.Schema, primaryAbsolutePath.Node.Parent).Path;
        if (string.IsNullOrEmpty(str))
          str = this.DragModel.DataSourceName;
        this.DragModel.Tooltip = string.Format((IFormatProvider) CultureInfo.InvariantCulture, StringTable.ArtboardBindTriggerActionTooltip, new object[2]
        {
          (object) pathName,
          (object) str
        });
      }
      else
      {
        string str = primaryAbsolutePath.Path;
        if (string.IsNullOrEmpty(str))
          str = this.DragModel.DataSourceName;
        this.DragModel.Tooltip = string.Format((IFormatProvider) CultureInfo.InvariantCulture, StringTable.ArtboardBindInvokeCommandActionTooltip, new object[1]
        {
          (object) str
        });
      }
    }

    public static List<MethodInfo> GetSupportedMethods(ITypeResolver typeResolver, Type type)
    {
      List<MethodInfo> list = new List<MethodInfo>();
      if (!DataBindingDragDropAddTriggerHandler.IsEnabled(typeResolver))
        return list;
      StringComparison comparison = DataBindingDragDropAddTriggerHandler.GetMethodNameComparison(typeResolver);
      foreach (MethodInfo methodInfo in DesignTypeGenerator.GetCommandMethods(type, typeResolver.PlatformMetadata, true))
      {
        MethodInfo method = methodInfo;
        object[] customAttributes = method.GetCustomAttributes(typeof (BrowsableAttribute), true);
        if ((customAttributes.Length <= 0 || ((BrowsableAttribute) customAttributes[0]).Browsable) && list.FindIndex((Predicate<MethodInfo>) (m => string.Compare(method.Name, m.Name, comparison) == 0)) < 0)
          list.Add(method);
      }
      return list;
    }

    private static StringComparison GetMethodNameComparison(ITypeResolver typeResolver)
    {
      StringComparison stringComparison = StringComparison.Ordinal;
      ProjectXamlContext projectXamlContext = ProjectXamlContext.FromProjectContext(typeResolver as IProjectContext);
      if (projectXamlContext == null)
        return stringComparison;
      IProject project = (IProject) projectXamlContext.GetService(typeof (IProject));
      if (project == null || project.CodeDocumentType == null)
        return stringComparison;
      ICodeGeneratorHost codeGeneratorHost = project.CodeDocumentType as ICodeGeneratorHost;
      if (codeGeneratorHost == null || (codeGeneratorHost.CodeDomProvider.LanguageOptions & LanguageOptions.CaseInsensitive) != LanguageOptions.CaseInsensitive)
        return stringComparison;
      stringComparison = StringComparison.OrdinalIgnoreCase;
      return stringComparison;
    }

    public static bool IsEnabled(ITypeResolver typeResolver)
    {
      return typeResolver.IsCapabilitySet(PlatformCapability.SupportAddTriggerDataDragDrop) && BlendSdkHelper.IsSdkInstalled(typeResolver.PlatformMetadata.TargetFramework);
    }
  }
}
