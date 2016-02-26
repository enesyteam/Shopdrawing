using System;
using System.Runtime.CompilerServices;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public class DesignInstanceExtension : MarkupExtension
	{
		public bool CreateList
		{
			get;
			set;
		}

		internal object Instance
		{
			get;
			set;
		}

		public bool IsDesignTimeCreatable
		{
			get;
			set;
		}

		[ConstructorArgument("targetType")]
		public System.Type Type
		{
			get;
			set;
		}

		public DesignInstanceExtension()
		{
		}

		public DesignInstanceExtension(System.Type targetType)
		{
			this.Type = targetType;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this.Instance;
		}
	}
}