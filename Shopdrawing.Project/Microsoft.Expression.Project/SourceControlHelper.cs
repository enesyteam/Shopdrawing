using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.SourceControl;
using Microsoft.Expression.Project.ServiceExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	internal static class SourceControlHelper
	{
		internal static void UpdateSourceControl(IEnumerable<DocumentReference> paths, UpdateSourceControlActions updateSourceControlAction, IServiceProvider serviceProvider)
		{
			SourceControlHelper.UpdateSourceControl(paths, updateSourceControlAction, new HandlerBasedProjectActionContext(serviceProvider));
		}

		internal static void UpdateSourceControl(IEnumerable<DocumentReference> paths, UpdateSourceControlActions updateSourceControlAction, IProjectActionContext context)
		{
			if (context == null)
			{
				return;
			}
			IProjectManager projectManager = context.ServiceProvider.ProjectManager();
			if (projectManager == null)
			{
				return;
			}
			ISolution currentSolution = projectManager.CurrentSolution;
			if (currentSolution == null || !currentSolution.IsSourceControlActive)
			{
				return;
			}
			SourceControlStatusCache.UpdateStatus(paths, context.ServiceProvider.SourceControlProvider());
			IEnumerable<DocumentReference> documentReferences = paths.Where<DocumentReference>((DocumentReference path) => {
				if (!context.IsSourceControlled(path))
				{
					return false;
				}
				return PathHelper.FileExists(path.Path);
			});
			IEnumerable<DocumentReference> documentReferences1 = Enumerable.Empty<DocumentReference>();
			if ((updateSourceControlAction & UpdateSourceControlActions.Checkout) == UpdateSourceControlActions.Checkout)
			{
				IEnumerable<DocumentReference> cachedStatus = 
					from path in documentReferences
					where SourceControlStatusCache.GetCachedStatus(path) == SourceControlStatus.CheckedIn
					select path;
				if (context.ServiceProvider.SourceControlProvider().Checkout((
					from path in cachedStatus
					select path.Path).ToArray<string>()) != SourceControlSuccess.Success)
				{
					documentReferences1.Concat<DocumentReference>(cachedStatus);
				}
				else
				{
					SourceControlStatusCache.SetCachedStatus(cachedStatus, SourceControlStatus.CheckedOut);
					foreach (DocumentReference cachedStatu in cachedStatus)
					{
						ProjectLog.LogSuccess(cachedStatu.Path, StringTable.MakeWritableAction, new object[0]);
					}
				}
			}
			if ((updateSourceControlAction & UpdateSourceControlActions.PendAdd) == UpdateSourceControlActions.PendAdd)
			{
				IEnumerable<DocumentReference> cachedStatus1 = 
					from path in documentReferences
					where SourceControlStatusCache.GetCachedStatus(path) == SourceControlStatus.None
					select path;
				if (context.ServiceProvider.SourceControlProvider().Add((
					from path in cachedStatus1
					select path.Path).ToArray<string>()) != SourceControlSuccess.Success)
				{
					documentReferences1.Concat<DocumentReference>(cachedStatus1);
				}
				else
				{
					SourceControlStatusCache.SetCachedStatus(cachedStatus1, SourceControlStatus.Add);
				}
			}
			SourceControlStatusCache.UpdateStatus(documentReferences1, context.ServiceProvider.SourceControlProvider());
		}
	}
}