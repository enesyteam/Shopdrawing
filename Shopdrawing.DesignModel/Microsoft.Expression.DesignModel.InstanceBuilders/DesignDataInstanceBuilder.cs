using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public class DesignDataInstanceBuilder : MarkupExtensionInstanceBuilderBase
	{
		public override Type BaseType
		{
			get
			{
				return typeof(DesignDataExtension);
			}
		}

		public DesignDataInstanceBuilder()
		{
		}

		public static string GetSourceFilePath(DocumentCompositeNode designDataNode)
		{
			string value = null;
			try
			{
				IProperty member = (IProperty)designDataNode.Type.GetMember(MemberType.LocalProperty, "Source", MemberAccessTypes.Public);
				DocumentPrimitiveNode item = designDataNode.Properties[member] as DocumentPrimitiveNode;
				if (item != null)
				{
					value = item.GetValue<string>();
					Uri uri = designDataNode.Context.MakeDesignTimeUri(new Uri(value, UriKind.RelativeOrAbsolute));
					value = uri.LocalPath;
				}
				else
				{
					return null;
				}
			}
			catch (InvalidOperationException invalidOperationException)
			{
			}
			catch (IOException oException)
			{
			}
			return value;
		}

		public static IDocumentRoot GetSourceXamlDocument(DocumentCompositeNode designDataNode)
		{
			return DesignDataInstanceBuilder.GetSourceXamlDocumentInternal(designDataNode, DesignDataInstanceBuilder.GetSourceFilePath(designDataNode));
		}

		private static IDocumentRoot GetSourceXamlDocumentInternal(DocumentCompositeNode designDataNode, string fullPath)
		{
			IDocumentRoot documentRoot = null;
			try
			{
				if (!string.IsNullOrEmpty(fullPath))
				{
					documentRoot = designDataNode.Context.GetDocumentRoot(fullPath);
				}
			}
			catch (InvalidOperationException invalidOperationException)
			{
			}
			catch (IOException oException)
			{
			}
			return documentRoot;
		}

		public override bool Instantiate(IInstanceBuilderContext context, ViewNode viewNode)
		{
			string str;
			DocumentNode rootNode;
			if (viewNode.Instance == null)
			{
				DesignDataExtension instance = null;
				using (IDisposable disposable = context.ChangeSerializationContext(null))
				{
					base.Instantiate(context, viewNode);
					instance = viewNode.Instance as DesignDataExtension;
				}
				instance.Instance = null;
				if (instance.Source != null)
				{
					DocumentCompositeNode documentNode = (DocumentCompositeNode)viewNode.DocumentNode;
					string sourceFilePath = DesignDataInstanceBuilder.GetSourceFilePath(documentNode);
					IDocumentRoot sourceXamlDocumentInternal = DesignDataInstanceBuilder.GetSourceXamlDocumentInternal(documentNode, sourceFilePath);
					if (sourceXamlDocumentInternal == null && sourceFilePath != null)
					{
						if (!File.Exists(sourceFilePath))
						{
							CultureInfo invariantCulture = CultureInfo.InvariantCulture;
							string designDataNotFound = ExceptionStringTable.DesignDataNotFound;
							object[] objArray = new object[] { sourceFilePath };
							str = string.Format(invariantCulture, designDataNotFound, objArray);
						}
						else
						{
							CultureInfo cultureInfo = CultureInfo.InvariantCulture;
							string designDataHasErrors = ExceptionStringTable.DesignDataHasErrors;
							object[] objArray1 = new object[] { sourceFilePath };
							str = string.Format(cultureInfo, designDataHasErrors, objArray1);
						}
						context.WarningDictionary.SetWarning(viewNode, documentNode, str);
					}
					if (sourceXamlDocumentInternal != null)
					{
						rootNode = sourceXamlDocumentInternal.RootNode;
					}
					else
					{
						rootNode = null;
					}
					DocumentNode documentNode1 = rootNode;
					if (documentNode1 != null)
					{
						context.DocumentRootResolver.GetDocumentRoot(documentNode1.DocumentRoot.DocumentContext.DocumentUrl);
						context.ViewNodeManager.AddRelatedDocumentRoot(viewNode, documentNode1.DocumentRoot);
						IInstanceBuilder builder = context.InstanceBuilderFactory.GetBuilder(documentNode1.TargetType);
						ViewNode viewNode1 = builder.GetViewNode(context, documentNode1);
						try
						{
							builder.Initialize(context, viewNode1, builder.Instantiate(context, viewNode1));
							instance.Instance = viewNode1.Instance;
						}
						catch (Exception exception)
						{
							instance.Instance = null;
							CultureInfo invariantCulture1 = CultureInfo.InvariantCulture;
							string designDataHasErrors1 = ExceptionStringTable.DesignDataHasErrors;
							object[] objArray2 = new object[] { sourceFilePath };
							string str1 = string.Format(invariantCulture1, designDataHasErrors1, objArray2);
							context.WarningDictionary.SetWarning(viewNode, documentNode, str1);
						}
						if (context.IsSerializationScope)
						{
							viewNode.Instance = instance.Instance;
							viewNode.InstanceState = InstanceState.Valid;
						}
						else
						{
							viewNode.Instance = instance;
							viewNode.InstanceState = InstanceState.Valid;
						}
					}
				}
				if (viewNode.Instance == instance && instance.Instance == null)
				{
					viewNode.Instance = null;
					viewNode.InstanceState = InstanceState.Valid;
				}
			}
			return viewNode.Instance != null;
		}

		public override void OnViewNodeInvalidating(IInstanceBuilderContext context, ViewNode target, ViewNode child, ref bool doesInvalidRootsContainTarget, List<ViewNode> invalidRoots)
		{
			base.OnViewNodeInvalidating(context, target, child, ref doesInvalidRootsContainTarget, invalidRoots);
			if (target.InstanceState != InstanceState.Invalid)
			{
				InstanceBuilderOperations.SetInvalid(context, target, ref doesInvalidRootsContainTarget, invalidRoots);
			}
		}
	}
}