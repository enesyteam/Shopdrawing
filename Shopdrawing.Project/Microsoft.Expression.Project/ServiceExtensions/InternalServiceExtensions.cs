using Microsoft.Expression.Framework;
using Microsoft.Expression.Project;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.ServiceExtensions
{
	internal static class InternalServiceExtensions
	{
		internal static bool OpenFile(this IServiceProvider source, string url)
		{
			bool flag;
			ISolution currentSolution = source.GetService<IProjectManager>().CurrentSolution;
			if (currentSolution == null)
			{
				return false;
			}
			using (IEnumerator<IProject> enumerator = currentSolution.Projects.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IProjectItem projectItem = enumerator.Current.Items.FindMatchByUrl<IProjectItem>(url);
					if (projectItem == null)
					{
						continue;
					}
					flag = projectItem.OpenView(true) != null;
					return flag;
				}
				return false;
			}
			return flag;
		}

		public static void SuspendNotificationsWhile(this IServiceProvider source, Action action)
		{
			using (IDisposable disposable = source.ExternalChanges().DelayNotification())
			{
				action();
			}
		}
	}
}