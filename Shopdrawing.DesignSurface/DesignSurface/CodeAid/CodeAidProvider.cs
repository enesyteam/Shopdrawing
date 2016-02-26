// Decompiled with JetBrains decompiler
// Type: Microsoft.Expression.DesignSurface.CodeAid.CodeAidProvider
// Assembly: Microsoft.Expression.DesignSurface, Version=4.0.20525.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 59645F1A-1518-4EC7-B4B6-3D42DEC6EBA3
// Assembly location: C:\Program Files (x86)\Microsoft Expression\Blend 4\Microsoft.Expression.DesignSurface.dll

using Microsoft.Expression.DesignModel.Code;
using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignSurface;
using Microsoft.Expression.DesignSurface.Properties;
using Microsoft.Expression.Framework.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.Expression.DesignSurface.CodeAid
{
  internal class CodeAidProvider : ICodeAidProvider
  {
    internal const string DefaultDescription = null;

    internal DesignerContext Context { get; private set; }

    internal IProjectContext ProjectContext
    {
      get
      {
        return this.Context.ActiveSceneViewModel.ProjectContext;
      }
    }

    private IAttachedPropertiesProvider AttachedPropertiesProvider
    {
      get
      {
        return this.ProjectContext.GetService(typeof (IAttachedPropertiesProvider)) as IAttachedPropertiesProvider;
      }
    }

    internal CodeAidProvider(DesignerContext context)
    {
      this.Context = context;
    }

    private static bool IsTypeInXmlNamespace(IProjectContext project, IType type, string xmlNamespaceString)
    {
      if (project == null || project.ProjectNamespaces == null)
        return false;
      IXmlNamespace @namespace = project.ProjectNamespaces.GetNamespace(type.RuntimeAssembly, type.Namespace);
      IXmlNamespace xmlNamespace = (IXmlNamespace) XmlNamespace.ToNamespace(xmlNamespaceString, XmlNamespace.GetNamespaceCanonicalization((ITypeResolver) project));
      return @namespace != null && @namespace.Value == xmlNamespace.Value;
    }

    private IEnumerable<IAssembly> AllAssembliesInProject()
    {
      IProjectContext project = this.ProjectContext;
      foreach (IAssembly assembly in (IEnumerable<IAssembly>) project.AssemblyReferences)
      {
        if (assembly.IsLoaded)
          yield return assembly;
      }
    }

    private IEnumerable<IType> AllTypesInProject()
    {
      return Enumerable.SelectMany<IAssembly, IType, IType>(this.AllAssembliesInProject(), (Func<IAssembly, IEnumerable<IType>>) (assembly => this.AllXamlRelevantTypesInAssembly(assembly)), (Func<IAssembly, IType, IType>) ((assembly, type) => type));
    }

    private IEnumerable<IType> AllXamlRelevantTypesInAssembly(IAssembly assembly)
    {
      IProjectContext project = this.ProjectContext;
      bool supportInternal = assembly.Equals((object) project.ProjectAssembly);
      Type[] assemblyTypes = Type.EmptyTypes;
      try
      {
        assemblyTypes = AssemblyHelper.GetTypes(assembly);
      }
      catch (ReflectionTypeLoadException ex)
      {
        assemblyTypes = Type.EmptyTypes;
      }
      IAttachedPropertiesProvider attachedPropertiesProvider = this.AttachedPropertiesProvider;
      if (attachedPropertiesProvider == null)
        throw new NotSupportedException();
      Type[] allTypesWithAttachedProperties = (Type[]) null;
      using (IAttachedPropertiesAccessToken token = attachedPropertiesProvider.AttachedProperties.Access())
      {
        CodeAidProvider.EnsurePreloadKnownAttachedProperties(token, project);
        allTypesWithAttachedProperties = Enumerable.ToArray<Type>(Enumerable.Select<IGrouping<Type, IAttachedPropertyMetadata>, Type>(Enumerable.GroupBy<IAttachedPropertyMetadata, Type>((IEnumerable<IAttachedPropertyMetadata>) token.AttachedPropertiesForAssembly(assembly.Name), (Func<IAttachedPropertyMetadata, Type>) (property => property.OwnerType)), (Func<IGrouping<Type, IAttachedPropertyMetadata>, Type>) (type => type.Key)));
      }
      foreach (Type type3 in assemblyTypes)
      {
        IType yieldedTypeId = (IType) null;
        try
        {
          if (!type3.IsGenericType)
          {
            if (type3.IsVisible)
            {
              if (!type3.IsNested)
              {
                if (!typeof (Attribute).IsAssignableFrom(type3))
                {
                  if (!typeof (Exception).IsAssignableFrom(type3))
                  {
                    if (type3.IsPublic)
                    {
                      if (TypeUtilities.HasDefaultConstructor(type3, supportInternal) && TypeUtilities.CanCreateTypeInXaml((ITypeResolver) project, type3))
                      {
                        IType type1 = project.GetType(type3);
                        if (JoltHelper.TypeSupported((ITypeResolver) project, (ITypeId) type1))
                          yieldedTypeId = type1;
                      }
                      else if (allTypesWithAttachedProperties != null)
                      {
                        if (OrderedListExtensions.GenericBinarySearch<Type, Type>(allTypesWithAttachedProperties, type3, (Func<Type, Type, int>) ((type1, type2) => type1.Name.CompareTo(type2.Name))) >= 0)
                          yieldedTypeId = project.GetType(type3);
                      }
                    }
                  }
                }
              }
            }
          }
        }
        catch (FileNotFoundException ex)
        {
        }
        if (yieldedTypeId != null)
          yield return yieldedTypeId;
      }
    }

    private IEnumerable<IType> GetAllAttachedPropertyTypesInProject()
    {
      IProjectContext project = this.ProjectContext;
      IAttachedPropertiesProvider propertiesProvider = this.AttachedPropertiesProvider;
      if (propertiesProvider == null)
        throw new NotSupportedException();
      using (IAttachedPropertiesAccessToken token = propertiesProvider.AttachedProperties.Access())
      {
        CodeAidProvider.EnsurePreloadKnownAttachedProperties(token, project);
        return (IEnumerable<IType>) Enumerable.ToArray<IType>(Enumerable.Select(Enumerable.Where(Enumerable.Select(Enumerable.GroupBy<IAttachedPropertyMetadata, Type>((IEnumerable<IAttachedPropertyMetadata>) token.AllAttachedProperties(), (Func<IAttachedPropertyMetadata, Type>) (property => property.OwnerType)), typeGroup => new
        {
          typeGroup = typeGroup,
          type = project.GetType(typeGroup.Key)
        }), param0 => param0.type != null), param0 => param0.type));
      }
    }

    private static void EnsurePreloadKnownAttachedProperties(IAttachedPropertiesAccessToken token, IProjectContext project)
    {
      token.PreloadType(project.ResolveType(PlatformTypes.Canvas));
      token.PreloadType(project.ResolveType(PlatformTypes.Grid));
    }

    public IEnumerable<ICodeAidTypeInfo> GetTypesInXmlNamespace(string xmlNamespaceString)
    {
      IProjectContext project = this.ProjectContext;
      foreach (IType type in this.AllTypesInProject())
      {
        if (CodeAidProvider.IsTypeInXmlNamespace(project, type, xmlNamespaceString))
          yield return (ICodeAidTypeInfo) new CodeAidTypeInfo(this, type);
      }
    }

    public IEnumerable<ICodeAidTypeInfo> GetTypesInClrNamespace(string assemblyName, string namespaceName)
    {
      IAssembly assembly = string.IsNullOrEmpty(assemblyName) ? this.ProjectContext.ProjectAssembly : Enumerable.FirstOrDefault<IAssembly>(this.AllAssembliesInProject(), (Func<IAssembly, bool>) (a =>
      {
        if (string.Equals(a.Name, assemblyName, StringComparison.OrdinalIgnoreCase))
          return !a.IsResolvedImplicitAssembly;
        return false;
      }));
      if (assembly == null)
        return Enumerable.Empty<ICodeAidTypeInfo>();
      return Enumerable.Cast<ICodeAidTypeInfo>((IEnumerable) Enumerable.Select<IType, CodeAidTypeInfo>(Enumerable.Where<IType>(this.AllXamlRelevantTypesInAssembly(assembly), (Func<IType, bool>) (type => type.Namespace == namespaceName)), (Func<IType, CodeAidTypeInfo>) (type => new CodeAidTypeInfo(this, type))));
    }

    public ICodeAidTypeInfo GetTypeByName(string uri, string typeName)
    {
      IProjectContext projectContext = this.ProjectContext;
      IXmlNamespace xmlNamespace = (IXmlNamespace) XmlNamespace.ToNamespace(uri, XmlNamespace.GetNamespaceCanonicalization((ITypeResolver) projectContext));
      if (xmlNamespace == null)
        return (ICodeAidTypeInfo) null;
      IType type = projectContext.GetType(xmlNamespace, typeName);
      if (type != null)
        return (ICodeAidTypeInfo) new CodeAidTypeInfo(this, type);
      return (ICodeAidTypeInfo) null;
    }

    public ICodeAidTypeInfo GetTypeByName(string assemblyName, string namespaceName, string typeName)
    {
      IType type = this.ProjectContext.GetType(assemblyName, TypeHelper.CombineNamespaceAndTypeName(namespaceName, typeName));
      if (type != null)
        return (ICodeAidTypeInfo) new CodeAidTypeInfo(this, type);
      return (ICodeAidTypeInfo) null;
    }

    public IEnumerable<ICodeAidTypeInfo> GetAttachedPropertyTypesInXmlNamespace(string uri)
    {
      return Enumerable.Cast<ICodeAidTypeInfo>((IEnumerable) Enumerable.Select<IType, CodeAidTypeInfo>(Enumerable.Where<IType>(this.GetAllAttachedPropertyTypesInProject(), (Func<IType, bool>) (type => CodeAidProvider.IsTypeInXmlNamespace(this.ProjectContext, type, uri))), (Func<IType, CodeAidTypeInfo>) (type => new CodeAidTypeInfo(this, type))));
    }

    public IEnumerable<ICodeAidTypeInfo> GetAttachedPropertyTypesInClrNamespace(string assembly, string namespaceName)
    {
      IProjectContext project = this.ProjectContext;
      if (string.IsNullOrEmpty(assembly))
        assembly = this.ProjectContext.ProjectAssembly.Name;
      IAttachedPropertiesProvider propertiesProvider = this.AttachedPropertiesProvider;
      if (propertiesProvider == null)
        throw new NotSupportedException();
      using (IAttachedPropertiesAccessToken propertiesAccessToken = propertiesProvider.AttachedProperties.Access())
        return Enumerable.Select<IGrouping<Type, IAttachedPropertyMetadata>, ICodeAidTypeInfo>(Enumerable.GroupBy<IAttachedPropertyMetadata, Type>((IEnumerable<IAttachedPropertyMetadata>) propertiesAccessToken.AttachedPropertiesForAssemblyAndNamespace(assembly, namespaceName), (Func<IAttachedPropertyMetadata, Type>) (property => property.OwnerType)), (Func<IGrouping<Type, IAttachedPropertyMetadata>, ICodeAidTypeInfo>) (typeGroup => (ICodeAidTypeInfo) new CodeAidTypeInfo(this, project.GetType(typeGroup.Key))));
    }

    public IEnumerable<ICodeAidAssemblyInfo> GetReferenceAssemblies()
    {
      throw new NotImplementedException();
    }

    public IEnumerable<ICodeAidMarkupExtensionInfo> GetMarkupExtensions()
    {
      throw new NotImplementedException();
    }

    public IEnumerable<ICodeAidMemberInfo> GetSystemBrushes()
    {
      throw new NotImplementedException();
    }

    public IEnumerable<ICodeAidMemberInfo> GetSystemColors()
    {
      throw new NotImplementedException();
    }

    public IEnumerable<ICodeAidMemberInfo> GetRelativeSources()
    {
      throw new NotImplementedException();
    }
  }
}
