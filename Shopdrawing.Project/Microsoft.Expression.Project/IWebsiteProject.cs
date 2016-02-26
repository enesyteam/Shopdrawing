using System;
using System.ComponentModel;

namespace Microsoft.Expression.Project
{
	public interface IWebsiteProject : IProject, INamedProject, IDocumentItem, IDisposable, INotifyPropertyChanged
	{
		void RefreshChildren(IDocumentItem item, bool selectNewlyCreatedItems);
	}
}