using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.InstanceBuilders;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.DesignModel.ViewObjects
{
	public class ContentProviderParameters
	{
		public DocumentCompositeNode ActivePropertyTrigger
		{
			get;
			set;
		}

		public object ExistingContent
		{
			get;
			set;
		}

		public object Instance
		{
			get;
			set;
		}

		public IInstanceBuilderContext InstanceBuilderContext
		{
			get;
			set;
		}

		public ICollection<IProperty> OverriddenProperties
		{
			get;
			set;
		}

		public ViewNode Target
		{
			get;
			set;
		}

		public object TemplateDataContext
		{
			get;
			set;
		}

		public ViewContentValueProvider ValueProviderObject
		{
			get;
			set;
		}

		public ContentProviderParameters()
		{
		}
	}
}