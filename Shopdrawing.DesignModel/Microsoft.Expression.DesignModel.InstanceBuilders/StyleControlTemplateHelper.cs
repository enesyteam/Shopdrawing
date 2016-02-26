using Microsoft.Expression.DesignModel.Metadata;
using System;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public abstract class StyleControlTemplateHelper
	{
		private static ITypeId[] typesToSkip;

		static StyleControlTemplateHelper()
		{
			ITypeId[] style = new ITypeId[] { PlatformTypes.Style, PlatformTypes.ControlTemplate, PlatformTypes.Setter, PlatformTypes.SetterBaseCollection, PlatformTypes.Trigger, PlatformTypes.TriggerCollection };
			StyleControlTemplateHelper.typesToSkip = style;
		}

		protected StyleControlTemplateHelper()
		{
		}

		public static ViewNode FindContainingControlTemplate(ViewNode viewNode)
		{
			while (viewNode != null && !viewNode.Type.Equals(PlatformTypes.ControlTemplate))
			{
				viewNode = viewNode.Parent;
			}
			return viewNode;
		}

		public static ViewNode FindStyleTemplateOwningViewNode(ViewNode viewNode)
		{
			while (viewNode.Parent != null)
			{
				bool flag = false;
				ITypeId[] typeIdArray = StyleControlTemplateHelper.typesToSkip;
				int num = 0;
				while (num < (int)typeIdArray.Length)
				{
					ITypeId typeId = typeIdArray[num];
					if (!viewNode.Parent.Type.Equals(typeId))
					{
						num++;
					}
					else
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					break;
				}
				viewNode = viewNode.Parent;
			}
			return viewNode.Parent;
		}
	}
}