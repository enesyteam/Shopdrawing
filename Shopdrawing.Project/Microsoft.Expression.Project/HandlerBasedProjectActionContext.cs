using Microsoft.Expression.Framework.Documents;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	public class HandlerBasedProjectActionContext : IProjectActionContext
	{
		private IServiceProvider serviceProvider;

		public Func<DocumentReference, bool> CanOverwriteHandler
		{
			get;
			set;
		}

		public Func<DocumentReference, Exception, bool> ExceptionHandler
		{
			get;
			set;
		}

		public Func<DocumentReference, bool> IsSourceControlledHandler
		{
			get;
			set;
		}

		public IServiceProvider ServiceProvider
		{
			get
			{
				return this.serviceProvider;
			}
		}

		public HandlerBasedProjectActionContext(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}

		public bool CanOverwrite(DocumentReference documentReference)
		{
			if (this.CanOverwriteHandler == null)
			{
				return true;
			}
			return this.CanOverwriteHandler(documentReference);
		}

		public bool HandleException(DocumentReference documentReference, Exception exception)
		{
			if (this.ExceptionHandler == null)
			{
				return true;
			}
			return this.ExceptionHandler(documentReference, exception);
		}

		public bool IsSourceControlled(DocumentReference documentReference)
		{
			if (this.IsSourceControlledHandler == null)
			{
				return true;
			}
			return this.IsSourceControlledHandler(documentReference);
		}
	}
}