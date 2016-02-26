using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public static class UserControlInstanceBuilderHelper
	{
		public static bool IsCurrentlyPreviewing(ViewNode userControlViewNode)
		{
			if (userControlViewNode == null)
			{
				return false;
			}
			if (userControlViewNode.Instance is IPreviewControl)
			{
				return true;
			}
			DocumentNode instance = userControlViewNode.Instance as DocumentNode;
			if (instance != null)
			{
				if (instance.Type.RuntimeType == null)
				{
					return true;
				}
				if (typeof(IPreviewControl).IsAssignableFrom(instance.Type.RuntimeType))
				{
					return true;
				}
			}
			return false;
		}

		public static bool NeedsRebuild(IInstanceBuilderContext context, ViewNode viewNode, string closedDocumentPath)
		{
			bool flag;
			bool flag1;
			if (!PlatformTypes.UserControl.Equals(viewNode.Type))
			{
				DocumentCompositeNode documentNode = viewNode.DocumentNode as DocumentCompositeNode;
				IProperty property = documentNode.TypeResolver.ResolveProperty(DesignTimeProperties.ClassProperty);
				if (documentNode != null && !documentNode.Properties.Contains(property))
				{
					string xamlSourcePath = viewNode.Type.XamlSourcePath;
					if (!string.IsNullOrEmpty(xamlSourcePath))
					{
						if (closedDocumentPath != null && closedDocumentPath.Equals(xamlSourcePath))
						{
							return true;
						}
						uint? changeStampWhenInstantiated = null;
						IPreviewControl instance = viewNode.Instance as IPreviewControl;
						if (instance != null)
						{
							changeStampWhenInstantiated = instance.ChangeStampWhenInstantiated;
						}
						IInstantiatedElementViewNode instantiatedElementViewNode = viewNode as IInstantiatedElementViewNode;
						if (instantiatedElementViewNode != null)
						{
							foreach (object instantiatedElement in instantiatedElementViewNode.InstantiatedElements)
							{
								instance = instantiatedElement as IPreviewControl;
								if (instance == null)
								{
									continue;
								}
								if (changeStampWhenInstantiated.HasValue && instance.ChangeStampWhenInstantiated.HasValue)
								{
									uint? nullable = changeStampWhenInstantiated;
									uint? changeStampWhenInstantiated1 = instance.ChangeStampWhenInstantiated;
									if ((nullable.GetValueOrDefault() != changeStampWhenInstantiated1.GetValueOrDefault() ? true : nullable.HasValue != changeStampWhenInstantiated1.HasValue))
									{
										flag = true;
										return flag;
									}
								}
								if (changeStampWhenInstantiated.HasValue)
								{
									continue;
								}
								changeStampWhenInstantiated = instance.ChangeStampWhenInstantiated;
							}
						}
						try
						{
							XamlDocument documentRoot = (XamlDocument)context.DocumentRootResolver.GetDocumentRoot(xamlSourcePath);
							if (documentRoot == null)
							{
								return false;
							}
							else
							{
								bool flag2 = UserControlInstanceBuilderHelper.ShouldUseDocumentForPreview(context, documentRoot);
								if (changeStampWhenInstantiated.HasValue != flag2)
								{
									flag1 = true;
								}
								else if (!changeStampWhenInstantiated.HasValue)
								{
									flag1 = false;
								}
								else
								{
									uint? nullable1 = changeStampWhenInstantiated;
									uint changeStamp = documentRoot.ChangeStamp;
									flag1 = (nullable1.GetValueOrDefault() != changeStamp ? true : !nullable1.HasValue);
								}
								flag = flag1;
							}
						}
						catch (IOException oException)
						{
							return false;
						}
						return flag;
					}
				}
			}
			return false;
		}

		public static bool ShouldInstantiatePreviewControl(IInstanceBuilderContext context, ViewNode viewNode, out XamlDocument sourceDocument, out string sourcePath)
		{
			sourceDocument = null;
			sourcePath = null;
			if (!PlatformTypes.UserControl.Equals(viewNode.Type))
			{
				DocumentCompositeNode documentNode = viewNode.DocumentNode as DocumentCompositeNode;
				IProperty property = documentNode.TypeResolver.ResolveProperty(DesignTimeProperties.ClassProperty);
				if (documentNode != null && !documentNode.Properties.Contains(property))
				{
					sourcePath = viewNode.Type.XamlSourcePath;
					if (!string.IsNullOrEmpty(sourcePath))
					{
						if (context.CurrentlyInstantiatingUserControlPreviews.Contains(sourcePath))
						{
							return false;
						}
						try
						{
							sourceDocument = (XamlDocument)context.DocumentRootResolver.GetDocumentRoot(sourcePath);
						}
						catch (FileNotFoundException fileNotFoundException)
						{
						}
						if (sourceDocument != null && UserControlInstanceBuilderHelper.ShouldUseDocumentForPreview(context, sourceDocument))
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private static bool ShouldUseDocumentForPreview(IInstanceBuilderContext context, XamlDocument xamlDocument)
		{
			if (!PlatformTypes.PlatformsCompatible(context.Platform.Metadata, xamlDocument.TypeResolver.PlatformMetadata))
			{
				return false;
			}
			return context.ShouldInstantiatePreviewControl(xamlDocument);
		}
	}
}