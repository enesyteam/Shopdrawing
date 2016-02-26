using System;
using System.Runtime.CompilerServices;
using System.Windows.Markup;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public class DesignDataExtension : MarkupExtension
	{
		internal object Instance
		{
			get;
			set;
		}

		[ConstructorArgument("source")]
		public string Source
		{
			get;
			set;
		}

		public DesignDataExtension()
		{
		}

		public DesignDataExtension(string source)
		{
			this.Source = source;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this.Instance;
		}
	}
}