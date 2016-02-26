using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Xaml;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public static class InstanceBuilderOperations
	{
		public static bool ClearValue(object target, IProperty propertyKey)
		{
			if (!InstanceBuilderOperations.IsSupported(propertyKey))
			{
				return false;
			}
			DocumentCompositeNode documentCompositeNode = target as DocumentCompositeNode;
			if (documentCompositeNode != null)
			{
				documentCompositeNode.Properties[propertyKey] = null;
				return true;
			}
			ReferenceStep referenceStep = propertyKey as ReferenceStep;
			if (referenceStep == null)
			{
				return false;
			}
			referenceStep.ClearValue(target);
			return true;
		}

		public static object FalseValue(IInstanceBuilderContext context)
		{
			if (!context.IsSerializationScope)
			{
				return false;
			}
			return context.DocumentContext.CreateNode(PlatformTypes.Boolean, new DocumentNodeStringValue(bool.FalseString));
		}

		public static bool GetIsInlinedResourceWithoutNamescope(ViewNode viewNode)
		{
			bool flag = false;
			ViewNode parent = viewNode;
			do
			{
				if (parent == null || parent.Parent == null)
				{
					break;
				}
				if (parent.Parent.DocumentNode == parent.DocumentNode.Parent || parent.DocumentNode.DocumentRoot == null)
				{
					parent = parent.Parent;
				}
				else
				{
					flag = true;
					break;
				}
			}
			while (!parent.DocumentNode.TypeResolver.PlatformMetadata.GetIsTypeItsOwnNameScope(parent.Type));
			return flag;
		}

		public static IList GetListAdapter(object instance)
		{
			if (instance != null)
			{
				CollectionAdapterDescription adapterDescription = CollectionAdapterDescription.GetAdapterDescription(instance.GetType());
				if (adapterDescription != null)
				{
					return adapterDescription.GetCollectionAdapter(instance) as IList;
				}
			}
			return null;
		}

		public static object InstantiateType(Type type, bool supportInternal)
		{
			ConstructorAccessibility constructorAccessibility;
			object obj;
			ConstructorInfo defaultConstructor = PlatformTypeHelper.GetDefaultConstructor(type, supportInternal, out constructorAccessibility);
			if (!(defaultConstructor != null) && constructorAccessibility != ConstructorAccessibility.TypeIsValueType)
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string instanceBuilderCannotInstantiateType = ExceptionStringTable.InstanceBuilderCannotInstantiateType;
				object[] name = new object[] { type.Name };
				throw new InstanceBuilderException(string.Format(currentCulture, instanceBuilderCannotInstantiateType, name), null, null);
			}
			try
			{
				obj = (defaultConstructor == null ? Activator.CreateInstance(type) : defaultConstructor.Invoke(null));
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				if (!(exception.InnerException is LicenseException))
				{
					CultureInfo cultureInfo = CultureInfo.CurrentCulture;
					string str = ExceptionStringTable.InstanceBuilderCannotInstantiateType;
					object[] objArray = new object[] { type.Name };
					throw new InstanceBuilderException(string.Format(cultureInfo, str, objArray), exception, null);
				}
				throw new InstanceBuilderException(exception.InnerException.Message);
			}
			return obj;
		}

		public static bool IsSupported(IMember member)
		{
			if (member.DeclaringType.RuntimeType != null)
			{
				return true;
			}
			return member.DeclaringType is XDataType;
		}

		public static Uri ProvideContextBaseUri(IDocumentContext documentContext)
		{
			string documentUrl = documentContext.DocumentUrl;
			if (string.IsNullOrEmpty(documentUrl))
			{
				return null;
			}
			return new Uri(string.Concat(Path.GetDirectoryName(documentUrl), Path.DirectorySeparatorChar));
		}

		public static void SetInvalid(IInstanceBuilderContext context, ViewNode target, ref bool doesInvalidRootsContainTarget, List<ViewNode> invalidRoots)
		{
			target.MergeState(InstanceState.Invalid);
			if (!doesInvalidRootsContainTarget)
			{
				invalidRoots.Add(target);
				doesInvalidRootsContainTarget = true;
			}
		}

		public static bool SetValue(object target, IProperty propertyKey, object value)
		{
            if (!InstanceBuilderOperations.IsSupported((IMember)propertyKey))
                return false;
            if (DesignTimeProperties.InlineXmlProperty.Equals((object)propertyKey))
            {
                IXmlSerializable serializable = target as IXmlSerializable;
                if (serializable == null)
                    return false;
                InstanceBuilderOperations.SetXmlContent(serializable, (string)value);
                return true;
            }
            DocumentCompositeNode documentCompositeNode = target as DocumentCompositeNode;
            if (documentCompositeNode != null)
            {
                IProperty property = (IProperty)propertyKey.Clone(documentCompositeNode.TypeResolver);
                documentCompositeNode.Properties[(IPropertyId)property] = (DocumentNode)value;
                return true;
            }
            ReferenceStep referenceStep = propertyKey as ReferenceStep;
            if (referenceStep != null)
            {
                object obj = value;
                MarkupExtension markupExtension;
                if ((markupExtension = obj as MarkupExtension) != null)
                {
                    if (markupExtension is TemplateBindingExtension)
                        throw new InstanceBuilderException(ExceptionStringTable.InvalidTemplateBindingInstanceBuilderException);
                    DependencyPropertyReferenceStep propertyReferenceStep1;
                    object property;
                    if ((propertyReferenceStep1 = referenceStep as DependencyPropertyReferenceStep) != null)
                    {
                        property = propertyReferenceStep1.DependencyProperty;
                    }
                    else
                    {
                        ClrPropertyReferenceStep propertyReferenceStep2;
                        if ((propertyReferenceStep2 = referenceStep as ClrPropertyReferenceStep) == null)
                            return false;
                        property = (object)propertyReferenceStep2.PropertyInfo;
                    }
                    DynamicResourceExtension resourceExtension = markupExtension as DynamicResourceExtension;
                    FrameworkElement frameworkElement = target as FrameworkElement;
                    FrameworkContentElement frameworkContentElement = target as FrameworkContentElement;
                    if (resourceExtension != null && resourceExtension.ResourceKey != null && propertyReferenceStep1 != null && (frameworkElement != null || frameworkContentElement != null))
                    {
                        if (frameworkElement != null)
                            propertyReferenceStep1.SetResourceReference((object)frameworkElement, resourceExtension.ResourceKey);
                        else if (frameworkContentElement != null)
                            propertyReferenceStep1.SetResourceReference((object)frameworkContentElement, resourceExtension.ResourceKey);
                    }
                    else
                    {
                        if (Microsoft.Expression.DesignModel.Metadata.KnownProperties.SetterValueProperty.Equals((object)referenceStep))
                        {
                            referenceStep.SetValue(target, obj);
                            return true;
                        }
                        bool flag = false;
                        try
                        {
                            obj = markupExtension.ProvideValue((IServiceProvider)new InstanceBuilderOperations.InstanceBuilderServiceProvider(target, property, (IMetadataResolver)referenceStep.DeclaringType.PlatformMetadata));
                            flag = true;
                        }
                        catch (InvalidOperationException ex)
                        {
                        }
                        if (flag)
                        {
                            if (!(obj is MarkupExtension))
                                return InstanceBuilderOperations.SetValue(target, (IProperty)referenceStep, obj);
                            referenceStep.SetValue(target, obj);
                            return true;
                        }
                    }
                }
                else
                {
                    //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
                    //if (obj is Expression)
                    //{
                    //    referenceStep.SetValue(target, obj);
                    //    return true;
                    //}
                    if (obj != null)
                    {
                        bool flag = false;
                        DependencyPropertyReferenceStep propertyReferenceStep = referenceStep as DependencyPropertyReferenceStep;
                        if (propertyReferenceStep != null)
                        {
                            ITypeId type = (ITypeId)propertyReferenceStep.PlatformTypes.GetType(obj.GetType());
                            if (PlatformTypes.Binding.IsAssignableFrom(type))
                                flag = true;
                        }
                        if (flag)
                        {
                            propertyReferenceStep.SetBinding(target, value);
                            return true;
                        }
                        if (!PlatformTypeHelper.GetPropertyType((IProperty)referenceStep).IsInstanceOfType(obj))
                            return false;
                    }
                    referenceStep.SetValue(target, obj);
                    return true;
                }
            }
            return false;
		}

		private static void SetXmlContent(IXmlSerializable serializable, string content)
		{
			if (content == null)
			{
				content = string.Empty;
			}
			serializable.ReadXml(XmlReader.Create(new StringReader(content)));
		}

		public static object TrueValue(IInstanceBuilderContext context)
		{
			if (!context.IsSerializationScope)
			{
				return true;
			}
			return context.DocumentContext.CreateNode(PlatformTypes.Boolean, new DocumentNodeStringValue(bool.TrueString));
		}

		public static ViewNode UpdateChildWithoutApply(IInstanceBuilderContext context, ViewNode viewNode, int childIndex, DocumentNodeChangeAction action, DocumentNode childNode)
		{
			ViewNode viewNode1 = null;
			if (action == DocumentNodeChangeAction.Remove)
			{
				ViewNode item = viewNode.Children[childIndex];
				viewNode.Children.Remove(item);
			}
			if (action == DocumentNodeChangeAction.Add || action == DocumentNodeChangeAction.Replace)
			{
				IInstanceBuilder builder = context.InstanceBuilderFactory.GetBuilder(childNode.TargetType);
				viewNode1 = builder.GetViewNode(context, childNode);
				if (action != DocumentNodeChangeAction.Replace)
				{
					viewNode.Children.Insert(childIndex, viewNode1);
				}
				else
				{
					viewNode.Children[childIndex] = viewNode1;
				}
				context.ViewNodeManager.Instantiate(viewNode1);
			}
			return viewNode1;
		}

		public static ViewNode UpdatePropertyWithoutApply(IInstanceBuilderContext context, ViewNode viewNode, IProperty propertyKey, DocumentNode valueNode)
		{
			ViewNode item = viewNode.Properties[propertyKey];
			ViewNode viewNode1 = null;
			if (item != null)
			{
				viewNode.Properties[propertyKey] = null;
			}
			if (valueNode != null)
			{
				IInstanceBuilder builder = context.InstanceBuilderFactory.GetBuilder(valueNode.TargetType);
				viewNode1 = builder.GetViewNode(context, valueNode);
				viewNode.Properties[propertyKey] = viewNode1;
				context.ViewNodeManager.Instantiate(viewNode1);
			}
			return viewNode1;
		}

		public static bool UseDesignTimeSize(object target, IInstanceBuilderContext context)
		{
			bool documentNode = false;
			if (context != null && context.ContainerRoot != null && context.ContainerRoot.DocumentNode != null && context.ContainerRoot.DocumentNode.DocumentRoot != null && context.ViewNodeManager != null && context.ViewNodeManager.Root != null)
			{
				ViewNode viewNode = context.InstanceDictionary.GetViewNode(target, true);
				if ((viewNode == null ? true : viewNode.Parent != null))
				{
					documentNode = context.ViewNodeManager.Root.DocumentNode != context.ContainerRoot.DocumentNode.DocumentRoot.RootNode;
					IInstantiatedElementViewNode root = context.ViewNodeManager.Root as IInstantiatedElementViewNode;
					if (documentNode && root != null)
					{
						object first = root.InstantiatedElements.First;
						if (first != null && (bool)(context.DocumentContext.TypeResolver.ResolveProperty(DesignTimeProperties.IsEnhancedOutOfPlaceRootProperty) as DependencyPropertyReferenceStep).GetValue(first))
						{
							documentNode = false;
						}
					}
					if (documentNode && viewNode != null)
					{
						DocumentNodePath containerNodePath = context.ViewNodeManager.GetCorrespondingNodePath(viewNode).GetContainerNodePath();
						if (PlatformTypes.ControlTemplate.Equals(containerNodePath.Node.Type) && containerNodePath.Node != context.ViewNodeManager.Root.DocumentNode)
						{
							DocumentNodePath parent = containerNodePath.GetParent();
							if (parent.Node.Type.Equals(PlatformTypes.Setter) && parent.Node is DocumentCompositeNode)
							{
								IMemberId valueAsMember = DocumentNodeHelper.GetValueAsMember((DocumentCompositeNode)parent.Node, Microsoft.Expression.DesignModel.Metadata.KnownProperties.SetterPropertyProperty);
								if (valueAsMember.Equals(Microsoft.Expression.DesignModel.Metadata.KnownProperties.ControlTemplateProperty) || valueAsMember.Equals(Microsoft.Expression.DesignModel.Metadata.KnownProperties.PageTemplateProperty))
								{
									containerNodePath = parent.GetContainerNodePath();
								}
							}
						}
						if (containerNodePath.Node != context.ViewNodeManager.Root.DocumentNode)
						{
							documentNode = false;
						}
					}
				}
				else
				{
					documentNode = true;
				}
			}
			return documentNode;
		}

		private sealed class InstanceBuilderServiceProvider : IServiceProvider
		{
			private InstanceBuilderOperations.InstanceBuilderServiceProvider.ProvideValueTarget provideValueTarget;

			private IMetadataResolver metadataResolver;

			private InstanceBuilderOperations.InstanceBuilderServiceProvider.XamlNameResolverImpl xamlNameResolver;

			private InstanceBuilderOperations.InstanceBuilderServiceProvider.XamlNameResolverImpl XamlNameResolver
			{
				get
				{
					if (this.xamlNameResolver == null)
					{
						this.xamlNameResolver = new InstanceBuilderOperations.InstanceBuilderServiceProvider.XamlNameResolverImpl(this.provideValueTarget.TargetObject, this.metadataResolver);
					}
					return this.xamlNameResolver;
				}
			}

			public InstanceBuilderServiceProvider(object target, object property, IMetadataResolver metadataResolver)
			{
				this.provideValueTarget = new InstanceBuilderOperations.InstanceBuilderServiceProvider.ProvideValueTarget(target, property);
				this.metadataResolver = metadataResolver;
			}

			public object GetService(Type serviceType)
			{
				if (typeof(IProvideValueTarget).IsAssignableFrom(serviceType))
				{
					return this.provideValueTarget;
				}
				if (typeof(IXamlNameResolver).IsAssignableFrom(serviceType))
				{
					return this.XamlNameResolver;
				}
				if (typeof(IXamlSchemaContextProvider).IsAssignableFrom(serviceType))
				{
					return new InstanceBuilderOperations.InstanceBuilderServiceProvider.XamlSchemaContextProvider();
				}
				return null;
			}

			private sealed class ProvideValueTarget : IProvideValueTarget
			{
				private object target;

				private object property;

				public object TargetObject
				{
					get
					{
						return this.target;
					}
				}

				public object TargetProperty
				{
					get
					{
						return this.property;
					}
				}

				public ProvideValueTarget(object target, object property)
				{
					this.target = target;
					this.property = property;
				}
			}

			private sealed class XamlNameResolverImpl : IXamlNameResolver
			{
				private static DependencyProperty DesignTimeNameScopeProperty;

				private object target;

				private System.Windows.Markup.INameScope nameScope;

				private bool isInitialized;

				public bool IsFixupTokenAvailable
				{
					get
					{
						return false;
					}
				}

				public XamlNameResolverImpl(object target, IMetadataResolver metadataResolver)
				{
					if (InstanceBuilderOperations.InstanceBuilderServiceProvider.XamlNameResolverImpl.DesignTimeNameScopeProperty == null)
					{
						DependencyPropertyReferenceStep dependencyPropertyReferenceStep = metadataResolver.ResolveProperty(DesignTimeProperties.DesignTimeNameScopeProperty) as DependencyPropertyReferenceStep;
						if (dependencyPropertyReferenceStep != null)
						{
							InstanceBuilderOperations.InstanceBuilderServiceProvider.XamlNameResolverImpl.DesignTimeNameScopeProperty = (DependencyProperty)dependencyPropertyReferenceStep.DependencyProperty;
						}
					}
					this.target = target;
					this.FindNameScope();
					UIThreadDispatcherHelper.BeginInvoke(DispatcherPriority.Send, new DispatcherOperationCallback((object o) => {
						this.isInitialized = true;
						this.FindNameScope();
						if (this.OnNameScopeInitializationComplete != null)
						{
							this.OnNameScopeInitializationComplete(this, EventArgs.Empty);
						}
						return null;
					}), null);
				}

				private void FindNameScope()
				{
					for (DependencyObject i = this.target as DependencyObject; i != null && this.nameScope == null; i = LogicalTreeHelper.GetParent(i))
					{
						this.nameScope = i.GetValue(System.Windows.NameScope.NameScopeProperty) as System.Windows.Markup.INameScope;
						if (this.nameScope == null)
						{
							this.nameScope = i as System.Windows.Markup.INameScope;
						}
						if (this.nameScope == null)
						{
							this.nameScope = i.GetValue(InstanceBuilderOperations.InstanceBuilderServiceProvider.XamlNameResolverImpl.DesignTimeNameScopeProperty) as System.Windows.Markup.INameScope;
						}
					}
				}

				public IEnumerable<KeyValuePair<string, object>> GetAllNamesAndValuesInScope()
				{
					System.Windows.NameScope nameScopes = this.nameScope as System.Windows.NameScope;
					if (nameScopes != null)
					{
						return nameScopes;
					}
					return Enumerable.Empty<KeyValuePair<string, object>>();
				}

				public object GetFixupToken(IEnumerable<string> names, bool canAssignDirectly)
				{
					throw new NotImplementedException();
				}

				public object GetFixupToken(IEnumerable<string> names)
				{
					throw new NotImplementedException();
				}

				public object Resolve(string name, out bool isFullyInitialized)
				{
					isFullyInitialized = this.isInitialized;
					return this.nameScope.FindName(name);
				}

				public object Resolve(string name)
				{
					return this.nameScope.FindName(name);
				}

				public event EventHandler OnNameScopeInitializationComplete;
			}

			private class XamlSchemaContextProvider : IXamlSchemaContextProvider
			{
				public XamlSchemaContext SchemaContext
				{
					get
					{
						return System.Windows.Markup.XamlReader.GetWpfSchemaContext();
					}
				}

				public XamlSchemaContextProvider()
				{
				}
			}
		}
	}
}