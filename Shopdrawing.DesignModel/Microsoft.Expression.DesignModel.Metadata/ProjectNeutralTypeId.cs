using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public sealed class ProjectNeutralTypeId : FullNameTypeId
	{
		private Microsoft.Expression.DesignModel.Metadata.AssemblyGroup assemblyGroup;

		private string[] potentialFullNames;

		private KnownUnreferencedType knownUnreferencedType;

		public Microsoft.Expression.DesignModel.Metadata.AssemblyGroup AssemblyGroup
		{
			get
			{
				return this.assemblyGroup;
			}
		}

		public IEnumerable<string> PotentialFullNames
		{
			get
			{
				return this.potentialFullNames;
			}
		}

		public ProjectNeutralTypeId(string canonicalFullName, Microsoft.Expression.DesignModel.Metadata.AssemblyGroup assemblyGroup, params string[] potentialFullNames) : base(canonicalFullName)
		{
			this.assemblyGroup = assemblyGroup;
			this.potentialFullNames = potentialFullNames;
		}

		public ProjectNeutralTypeId(string canonicalFullName, Microsoft.Expression.DesignModel.Metadata.AssemblyGroup assemblyGroup, KnownUnreferencedType knownUnreferencedType, params string[] potentialFullNames) : this(canonicalFullName, assemblyGroup, potentialFullNames)
		{
			this.knownUnreferencedType = knownUnreferencedType;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (this == obj)
			{
				return true;
			}
			ITypeId typeId = obj as ITypeId;
			if (typeId != null && (base.FullName == typeId.FullName || this.MatchesPotentialFullName(typeId.FullName)))
			{
				ProjectContextType projectContextType = typeId as ProjectContextType;
				if (projectContextType == null)
				{
					return true;
				}
				IPlatformTypes platformMetadata = (IPlatformTypes)projectContextType.PlatformMetadata;
				using (IEnumerator<IAssemblyId> enumerator = platformMetadata.GetAssemblyGroup(this.AssemblyGroup).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IAssemblyId current = enumerator.Current;
						if (projectContextType.RuntimeAssembly.Name != current.Name)
						{
							continue;
						}
						flag = true;
						return flag;
					}
					return false;
				}
				return flag;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool IsAssignableFrom(ITypeId type)
		{
			if (type == null || !type.IsResolvable)
			{
				return false;
			}
			if (this.knownUnreferencedType != KnownUnreferencedType.None)
			{
				IUnreferencedTypeId unreferencedTypeId = type as IUnreferencedTypeId;
				if (unreferencedTypeId != null && unreferencedTypeId.IsKnownUnreferencedType(this.knownUnreferencedType))
				{
					return true;
				}
			}
			IType type1 = (IType)type;
			IType type2 = type1.PlatformMetadata.ResolveType(this);
			if (type2.IsResolvable)
			{
				return type2.IsAssignableFrom(type);
			}
			ProjectContextType projectContextType = type1 as ProjectContextType;
			if (projectContextType != null)
			{
				type2 = projectContextType.TypeResolver.ResolveType(this);
				if (type2 != null)
				{
					return type2.IsAssignableFrom(type);
				}
			}
			return false;
		}

		private bool MatchesPotentialFullName(string otherFullName)
		{
			if (this.potentialFullNames == null)
			{
				return false;
			}
			for (int i = 0; i < (int)this.potentialFullNames.Length; i++)
			{
				if (otherFullName.Equals(this.potentialFullNames[i]))
				{
					return true;
				}
			}
			return false;
		}
	}
}