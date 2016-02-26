using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Expression.Project
{
	internal class ProjectTypeManager : IProjectTypeManager
	{
		private readonly ProjectTypeManager.ProjectTypeCollection projectTypes = new ProjectTypeManager.ProjectTypeCollection();

		private readonly IProjectType unknownProjectType = new Microsoft.Expression.Project.UnknownProjectType();

		public IProjectTypeCollection ProjectTypes
		{
			get
			{
				return this.projectTypes;
			}
		}

		public IProjectType UnknownProjectType
		{
			get
			{
				return this.unknownProjectType;
			}
		}

		public ProjectTypeManager()
		{
			this.ProjectTypes.Add(this.unknownProjectType);
		}

		public IProjectType GetProjectTypeForProject(IProjectStore projectStore)
		{
			IProjectType item;
			PerformanceUtility.StartPerformanceSequence(PerformanceEvent.GetProjectTypeForProject);
			try
			{
				int count = this.projectTypes.Count - 1;
				while (count >= 0)
				{
					if (!this.projectTypes[count].IsValidTypeForProject(projectStore))
					{
						count--;
					}
					else
					{
						item = this.projectTypes[count];
						return item;
					}
				}
				return this.unknownProjectType;
			}
			finally
			{
				PerformanceUtility.EndPerformanceSequence(PerformanceEvent.GetProjectTypeForProject);
			}
			return item;
		}

		public void Register(IProjectType projectType)
		{
			if (projectType == null)
			{
				throw new ArgumentNullException("projectType");
			}
			this.projectTypes.Add(projectType);
		}

		public void Unregister(IProjectType projectType)
		{
			this.projectTypes.Remove(projectType);
		}

		private class ProjectTypeCollection : IProjectTypeCollection, ICollection, IEnumerable
		{
			private List<IProjectType> list;

			public int Count
			{
				get
				{
					return this.list.Count;
				}
			}

			public bool IsSynchronized
			{
				get
				{
					throw new NotSupportedException();
				}
			}

			public IProjectType this[string identifier]
			{
				get
				{
					PerformanceUtility.StartPerformanceSequence(PerformanceEvent.SearchByName);
					ProjectType projectType = null;
					foreach (ProjectType projectType1 in this)
					{
						if (projectType1.Identifier != identifier)
						{
							continue;
						}
						projectType = projectType1;
						break;
					}
					PerformanceUtility.EndPerformanceSequence(PerformanceEvent.SearchByName);
					return projectType;
				}
			}

			public IProjectType this[int index]
			{
				get
				{
					return this.list[index];
				}
			}

			public object SyncRoot
			{
				get
				{
					throw new NotSupportedException();
				}
			}

			public ProjectTypeCollection()
			{
			}

			public void Add(IProjectType value)
			{
				this.list.Add(value);
			}

			public bool Contains(IProjectType value)
			{
				return this.list.Contains(value);
			}

			public void CopyTo(Array array, int index)
			{
				throw new NotSupportedException();
			}

			public IEnumerator GetEnumerator()
			{
				return this.list.GetEnumerator();
			}

			public void Remove(IProjectType value)
			{
				this.list.Remove(value);
			}
		}
	}
}