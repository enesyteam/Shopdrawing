using System;

namespace Microsoft.Expression.DesignModel.Core
{
	public static class CategoryNames
	{
		public readonly static string CategoryBrushes;

		public readonly static string CategoryMaterials;

		public readonly static string CategoryAppearance;

		public readonly static string CategoryLayout;

		public readonly static string CategoryLayoutPaths;

		public readonly static string CategoryCommonProperties;

		public readonly static string CategoryDataVisualization;

		public readonly static string CategoryText;

		public readonly static string CategoryTransform;

		public readonly static string CategoryMedia;

		public readonly static string CategoryCamera;

		public readonly static string CategoryLight;

		public readonly static string CategoryEasing;

		public readonly static string CategoryTrigger;

		public readonly static string CategoryMisc;

		public readonly static string CategoryAnimationProperties;

		public readonly static string CategoryConditions;

		public readonly static string CategoryTagProperties;

		static CategoryNames()
		{
			CategoryNames.CategoryBrushes = PresentationFrameworkStringTable.Category_Brushes;
			CategoryNames.CategoryMaterials = PresentationFrameworkStringTable.Category_Materials;
			CategoryNames.CategoryAppearance = PresentationFrameworkStringTable.Category_Appearance;
			CategoryNames.CategoryLayout = PresentationFrameworkStringTable.Category_Layout;
			CategoryNames.CategoryLayoutPaths = PresentationFrameworkStringTable.Category_LayoutPaths;
			CategoryNames.CategoryCommonProperties = PresentationFrameworkStringTable.Category_Common_Properties;
			CategoryNames.CategoryDataVisualization = PresentationFrameworkStringTable.Category_Data_Visualization;
			CategoryNames.CategoryText = PresentationFrameworkStringTable.Category_Text;
			CategoryNames.CategoryTransform = PresentationFrameworkStringTable.Category_Transform;
			CategoryNames.CategoryMedia = PresentationFrameworkStringTable.Category_Media;
			CategoryNames.CategoryCamera = PresentationFrameworkStringTable.Category_Camera;
			CategoryNames.CategoryLight = PresentationFrameworkStringTable.Category_Light;
			CategoryNames.CategoryEasing = PresentationFrameworkStringTable.Category_Easing;
			CategoryNames.CategoryTrigger = PresentationFrameworkStringTable.Category_Trigger;
			CategoryNames.CategoryMisc = PresentationFrameworkStringTable.Category_Misc;
			CategoryNames.CategoryAnimationProperties = PresentationFrameworkStringTable.Category_Animation_Properties;
			CategoryNames.CategoryConditions = PresentationFrameworkStringTable.Category_Conditions;
			CategoryNames.CategoryTagProperties = PresentationFrameworkStringTable.Category_TagProperties;
		}
	}
}