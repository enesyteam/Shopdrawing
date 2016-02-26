using Microsoft.Windows.Design.PropertyEditing;
using System;

namespace Microsoft.Expression.DesignModel.Metadata
{
	public static class PropertyOrderTokens
	{
		private static PropertyOrder layoutSizePriority;

		private static PropertyOrder minLayoutSizePriority;

		private static PropertyOrder maxLayoutSizePriority;

		private static PropertyOrder scrollBarVisibilityPriority;

		private static PropertyOrder layoutAlignmentPriority;

		private static PropertyOrder propertyOrderTokenLevel1;

		private static PropertyOrder propertyOrderTokenLevel2;

		private static PropertyOrder propertyOrderTokenLevel3;

		private static PropertyOrder propertyOrderToken1;

		private static PropertyOrder propertyOrderToken2;

		private static PropertyOrder propertyOrderToken3;

		private static PropertyOrder lowerPriority;

		private static PropertyOrder mediumPriority;

		private static PropertyOrder higherPriority;

		public static PropertyOrder HigherPriority
		{
			get
			{
				if (PropertyOrderTokens.higherPriority == null)
				{
					PropertyOrderTokens.higherPriority = PropertyOrder.CreateBefore(PropertyOrder.Early);
				}
				return PropertyOrderTokens.higherPriority;
			}
		}

		public static PropertyOrder LayoutAlignmentPriority
		{
			get
			{
				if (PropertyOrderTokens.layoutAlignmentPriority == null)
				{
					PropertyOrderTokens.layoutAlignmentPriority = PropertyOrder.CreateAfter(PropertyOrderTokens.LayoutSizePriority);
				}
				return PropertyOrderTokens.layoutAlignmentPriority;
			}
		}

		public static PropertyOrder LayoutSizePriority
		{
			get
			{
				if (PropertyOrderTokens.layoutSizePriority == null)
				{
					PropertyOrderTokens.layoutSizePriority = PropertyOrder.CreateAfter(PropertyOrder.Early);
				}
				return PropertyOrderTokens.layoutSizePriority;
			}
		}

		public static PropertyOrder LowerPriority
		{
			get
			{
				if (PropertyOrderTokens.lowerPriority == null)
				{
					PropertyOrderTokens.lowerPriority = PropertyOrder.CreateAfter(PropertyOrder.Late);
				}
				return PropertyOrderTokens.lowerPriority;
			}
		}

		public static PropertyOrder MaxLayoutSizePriority
		{
			get
			{
				if (PropertyOrderTokens.maxLayoutSizePriority == null)
				{
					PropertyOrderTokens.maxLayoutSizePriority = PropertyOrder.CreateBefore(PropertyOrderTokens.PropertyOrderTokenLevel2);
				}
				return PropertyOrderTokens.maxLayoutSizePriority;
			}
		}

		public static PropertyOrder MediumPriority
		{
			get
			{
				if (PropertyOrderTokens.mediumPriority == null)
				{
					PropertyOrderTokens.mediumPriority = PropertyOrder.CreateAfter(PropertyOrder.Default);
				}
				return PropertyOrderTokens.mediumPriority;
			}
		}

		public static PropertyOrder MinLayoutSizePriority
		{
			get
			{
				if (PropertyOrderTokens.minLayoutSizePriority == null)
				{
					PropertyOrderTokens.minLayoutSizePriority = PropertyOrder.CreateBefore(PropertyOrderTokens.PropertyOrderTokenLevel1);
				}
				return PropertyOrderTokens.minLayoutSizePriority;
			}
		}

		public static PropertyOrder PropertyOrderToken1
		{
			get
			{
				if (PropertyOrderTokens.propertyOrderToken1 == null)
				{
					PropertyOrderTokens.propertyOrderToken1 = PropertyOrder.CreateBefore(PropertyOrderTokens.PropertyOrderTokenLevel1);
				}
				return PropertyOrderTokens.propertyOrderToken1;
			}
		}

		public static PropertyOrder PropertyOrderToken2
		{
			get
			{
				if (PropertyOrderTokens.propertyOrderToken2 == null)
				{
					PropertyOrderTokens.propertyOrderToken2 = PropertyOrder.CreateBefore(PropertyOrderTokens.PropertyOrderTokenLevel2);
				}
				return PropertyOrderTokens.propertyOrderToken2;
			}
		}

		public static PropertyOrder PropertyOrderToken3
		{
			get
			{
				if (PropertyOrderTokens.propertyOrderToken3 == null)
				{
					PropertyOrderTokens.propertyOrderToken3 = PropertyOrder.CreateBefore(PropertyOrderTokens.PropertyOrderTokenLevel3);
				}
				return PropertyOrderTokens.propertyOrderToken3;
			}
		}

		private static PropertyOrder PropertyOrderTokenLevel1
		{
			get
			{
				if (PropertyOrderTokens.propertyOrderTokenLevel1 == null)
				{
					PropertyOrderTokens.propertyOrderTokenLevel1 = PropertyOrder.CreateAfter(PropertyOrder.Early);
				}
				return PropertyOrderTokens.propertyOrderTokenLevel1;
			}
		}

		private static PropertyOrder PropertyOrderTokenLevel2
		{
			get
			{
				if (PropertyOrderTokens.propertyOrderTokenLevel2 == null)
				{
					PropertyOrderTokens.propertyOrderTokenLevel2 = PropertyOrder.CreateAfter(PropertyOrderTokens.PropertyOrderTokenLevel1);
				}
				return PropertyOrderTokens.propertyOrderTokenLevel2;
			}
		}

		private static PropertyOrder PropertyOrderTokenLevel3
		{
			get
			{
				if (PropertyOrderTokens.propertyOrderTokenLevel3 == null)
				{
					PropertyOrderTokens.propertyOrderTokenLevel3 = PropertyOrder.CreateAfter(PropertyOrderTokens.PropertyOrderTokenLevel2);
				}
				return PropertyOrderTokens.propertyOrderTokenLevel3;
			}
		}

		public static PropertyOrder ScrollBarVisibilityPriority
		{
			get
			{
				if (PropertyOrderTokens.scrollBarVisibilityPriority == null)
				{
					PropertyOrderTokens.scrollBarVisibilityPriority = PropertyOrder.CreateBefore(PropertyOrderTokens.PropertyOrderTokenLevel3);
				}
				return PropertyOrderTokens.scrollBarVisibilityPriority;
			}
		}
	}
}