using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	internal sealed class ProjectTypeInstanceBuilderHelper
	{
		private List<ProjectTypeInstanceBuilderHelper.InstanceBuilderInfo> creators = new List<ProjectTypeInstanceBuilderHelper.InstanceBuilderInfo>();

		public ProjectTypeInstanceBuilderHelper()
		{
		}

		public void Refresh(ITypeResolver typeResolver, IInstanceBuilderFactory factory)
		{
			foreach (ProjectTypeInstanceBuilderHelper.InstanceBuilderInfo creator in this.creators)
			{
				IType type = typeResolver.ResolveType(creator.ProjectType);
				if (!creator.AddIfMissing(type.RuntimeType))
				{
					continue;
				}
				factory.Register(creator.CreateDelegate(type));
			}
		}

		public void Register(ITypeId projectType, ProjectTypeInstanceBuilderHelper.CreateBuilder createDelegate)
		{
			this.creators.Add(new ProjectTypeInstanceBuilderHelper.InstanceBuilderInfo(projectType, createDelegate));
		}

		public delegate IInstanceBuilder CreateBuilder(IType resolvedType);

		private class InstanceBuilderInfo
		{
			private static ProjectTypeInstanceBuilderHelper.TypeComparer typeComparer;

			public ProjectTypeInstanceBuilderHelper.CreateBuilder CreateDelegate
			{
				get;
				private set;
			}

			public ITypeId ProjectType
			{
				get;
				private set;
			}

			public List<Type> RegisteredTypes
			{
				get;
				private set;
			}

			static InstanceBuilderInfo()
			{
				ProjectTypeInstanceBuilderHelper.InstanceBuilderInfo.typeComparer = new ProjectTypeInstanceBuilderHelper.TypeComparer();
			}

			public InstanceBuilderInfo(ITypeId projectType, ProjectTypeInstanceBuilderHelper.CreateBuilder createDelegate)
			{
				this.ProjectType = projectType;
				this.CreateDelegate = createDelegate;
				this.RegisteredTypes = new List<Type>();
			}

			public bool AddIfMissing(Type type)
			{
				if (type == null)
				{
					return false;
				}
				int num = this.RegisteredTypes.BinarySearch(type, ProjectTypeInstanceBuilderHelper.InstanceBuilderInfo.typeComparer);
				if (num >= 0)
				{
					return false;
				}
				this.RegisteredTypes.Insert(~num, type);
				return true;
			}
		}

		private class TypeComparer : IComparer<Type>
		{
			public TypeComparer()
			{
			}

			public int Compare(Type typeA, Type typeB)
			{
				if (typeA == typeB)
				{
					return 0;
				}
				string fullName = typeA.FullName ?? string.Empty;
				int num = string.CompareOrdinal(fullName, typeB.FullName ?? string.Empty);
				if (num != 0)
				{
					return num;
				}
				fullName = typeA.Assembly.FullName ?? string.Empty;
				num = string.CompareOrdinal(fullName, typeB.Assembly.FullName ?? string.Empty);
				return num;
			}
		}
	}
}