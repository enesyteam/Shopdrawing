using Microsoft.Expression.Extensibility.Project;
using Microsoft.Expression.Framework;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace Microsoft.Expression.Project
{
	internal class ProjectAdapterService : IProjectAdapterService
	{
		private IServiceProvider serviceProvider;

		[ImportMany]
		public IEnumerable<IProjectAdapter> Adapters
		{
			get;
			set;
		}

		public ProjectAdapterService(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}

		public T FindAdapter<T>(Microsoft.Expression.Project.IProject project)
		{
			Microsoft.Expression.Extensibility.Project.IProject project1;
			return this.FindAdapter<T>(project, out project1);
		}

		public T FindAdapter<T>(Microsoft.Expression.Project.IProject project, out Microsoft.Expression.Extensibility.Project.IProject extensionProject)
		{
			T t;
			T t1;
			extensionProject = null;
			if (this.Adapters != null)
			{
				using (IEnumerator<IProjectAdapter> enumerator = this.Adapters.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IProjectAdapter current = enumerator.Current;
						if (!typeof(T).IsAssignableFrom(current.GetType()))
						{
							continue;
						}
						try
						{
							if (current.RequiredTargetFramework == null || project.TargetFramework != null && current.RequiredTargetFramework.Identifier.Equals(project.TargetFramework.Identifier, StringComparison.OrdinalIgnoreCase) && current.RequiredTargetFramework.Profile.Equals(project.TargetFramework.Profile, StringComparison.OrdinalIgnoreCase) && current.RequiredTargetFramework.Version <= project.TargetFramework.Version)
							{
								if (extensionProject == null)
								{
									extensionProject = new Microsoft.Expression.Extensibility.Project.Project(project);
								}
								if (current.AppliesToProject(extensionProject))
								{
									t = (T)current;
									return t;
								}
							}
						}
						catch (Exception exception)
						{
							this.serviceProvider.MessageLoggingService().WriteLine(exception.Message);
						}
					}
					t1 = default(T);
					return t1;
				}
				return t;
			}
			t1 = default(T);
			return t1;
		}

		public bool IsTargetFrameworkSupported(FrameworkName targetFramework)
		{
			bool flag;
			if (targetFramework == null)
			{
				return false;
			}
			if (this.Adapters != null)
			{
				using (IEnumerator<IProjectAdapter> enumerator = this.Adapters.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IProjectAdapter current = enumerator.Current;
						try
						{
							if (current.RequiredTargetFramework.Identifier.Equals(targetFramework.Identifier, StringComparison.OrdinalIgnoreCase) && current.RequiredTargetFramework.Profile.Equals(targetFramework.Profile, StringComparison.OrdinalIgnoreCase) && current.RequiredTargetFramework.Version <= targetFramework.Version)
							{
								flag = true;
								return flag;
							}
						}
						catch (Exception exception)
						{
							this.serviceProvider.MessageLoggingService().WriteLine(exception.Message);
						}
					}
					return false;
				}
				return flag;
			}
			return false;
		}
	}
}