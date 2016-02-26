using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public interface IPlatformTypes : IPlatformMetadata, IMetadataResolver, IDisposable
	{
		Microsoft.Expression.DesignModel.Metadata.CommonProperties CommonProperties
		{
			get;
		}

		ReadOnlyCollection<IAssembly> DefaultAssemblies
		{
			get;
		}

		ReadOnlyCollection<IAssembly> DefaultAssemblyReferences
		{
			get;
		}

		ITypeResolver DefaultTypeResolver
		{
			get;
		}

		Microsoft.Expression.DesignModel.Metadata.DesignTimeProperties DesignTimeProperties
		{
			get;
		}

		ICollection<string> ImageFileExtensions
		{
			get;
		}

		IPlatformReferenceContext ReferenceContext
		{
			get;
		}

		IPlatformRuntimeContext RuntimeContext
		{
			get;
		}

		ITypeMetadataFactory TypeMetadataFactory
		{
			get;
		}

		IAssembly CreateAssembly(string name);

		IAssembly CreateAssembly(Assembly assembly, AssemblySource assemblySource);

		IAssembly CreateAssembly(Assembly assembly, AssemblySource assemblySource, bool isImplicitlyResolved);

		ITypeMetadataFactory CreateTypeMetadataFactory(ITypeResolver typeResolver);

		XmlnsDefinitionMap CreateXmlnsDefinitionMap(ITypeResolver typeResolver, IEnumerable<IAssembly> assemblies, IAssembly targetAssembly);

		bool EnsureAssemblyReferenced(ITypeResolver typeResolver, ITypeId type);

		IEnumerable<IAssemblyId> GetAssemblyGroup(AssemblyGroup assemblyGroup);

		IAssembly GetDesignToolAssembly(Assembly assembly);

		ISupportInitialize GetISupportInitialize(object target);

		IAssembly GetPlatformAssembly(Assembly assembly);

		IAssembly GetPlatformAssemblyUsingAssemblyName(Assembly assembly);

		IAssembly GetPlatformAssemblyUsingAssemblyName(IAssembly assembly);

		object GetPlatformCache(string cacheName);

		IType GetPlatformType(string typeName);

		IProperty GetProperty(ITypeResolver typeResolver, Type targetType, MemberType memberTypes, string memberName);

		IType GetType(Type type);

		bool IsBinding(Type type);

		bool IsDesignToolAssembly(IAssembly assembly);

		bool IsExpression(Type type);

		bool IsLooselySupported(ITypeResolver typeResolver, ITypeId type);

		bool IsResource(Type type);

		object MakePropertyPath(string path, params object[] parameters);

		void RefreshAssemblies(ITypeResolver typeResolver, IEnumerable<Assembly> designTimeAssemblies);

		void RegisterAssembly(Assembly assembly);

		void SetPlatformCache(string cacheName, object value);
	}
}