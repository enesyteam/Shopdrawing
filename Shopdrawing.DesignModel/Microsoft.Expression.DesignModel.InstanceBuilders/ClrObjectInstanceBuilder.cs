using Microsoft.Expression.DesignModel.Core;
using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.ViewObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Microsoft.Expression.DesignModel.InstanceBuilders
{
	public class ClrObjectInstanceBuilder : IInstanceBuilder
	{
		internal readonly static object InvalidObjectSentinel;

		public virtual Type BaseType
		{
			get
			{
				return typeof(object);
			}
		}

		public virtual Type ReplacementType
		{
			get
			{
				return null;
			}
		}

		static ClrObjectInstanceBuilder()
		{
			ClrObjectInstanceBuilder.InvalidObjectSentinel = new object();
		}

		public ClrObjectInstanceBuilder()
		{
		}

		public virtual bool AllowPostponedResourceUpdate(IInstanceBuilderContext context, ViewNode viewNode, IProperty propertyKey, DocumentNodePath evaluatedResource)
		{
			return false;
		}

		protected virtual void BeginInit(IInstanceBuilderContext context, ViewNode viewNode, DocumentCompositeNode compositeNode, ISupportInitialize supportInitialize)
		{
			supportInitialize.BeginInit();
		}

		protected static bool DoesPropertyApply(Type replacementType, IPropertyId propertyKey)
		{
			ClrPropertyReferenceStep clrPropertyReferenceStep = propertyKey as ClrPropertyReferenceStep;
			if (clrPropertyReferenceStep == null)
			{
				return true;
			}
			return clrPropertyReferenceStep.TargetType.IsAssignableFrom(replacementType);
		}

		protected virtual void EndInit(IInstanceBuilderContext context, ViewNode viewNode, DocumentCompositeNode compositeNode, ISupportInitialize supportInitialize)
		{
			supportInitialize.EndInit();
		}

		private object FindInstantiatedResource(IInstanceBuilderContext context, ViewNode viewNode, DocumentNode documentNode, DocumentNode keyNode)
		{
			object obj;
			IInstanceBuilder builder = context.InstanceBuilderFactory.GetBuilder(keyNode.TargetType);
			ViewNode viewNode1 = builder.GetViewNode(context, keyNode);
			object obj1 = null;
			using (IDisposable disposable = context.ChangeSerializationContext(null))
			{
				using (IDisposable disposable1 = context.DisablePostponedResourceEvaluation())
				{
					obj1 = context.ViewNodeManager.Instantiate(viewNode1);
					if (obj1 == null)
					{
						obj = null;
						return obj;
					}
				}
			}
			object obj2 = null;
			ViewNode parent = viewNode;
			while (parent != null && documentNode != null && parent.DocumentNode == documentNode)
			{
				obj2 = this.LookupInstantiatedResource(context, parent, obj1);
				if (obj2 != null)
				{
					return obj2;
				}
				parent = parent.Parent;
				documentNode = documentNode.Parent;
			}
			IDocumentRoot applicationRoot = context.DocumentRootResolver.ApplicationRoot;
			if (applicationRoot != null)
			{
				IInstanceBuilderContext viewContext = context.GetViewContext(applicationRoot);
				if (viewContext != null)
				{
					obj2 = this.LookupInstantiatedResource(viewContext, viewContext.ViewNodeManager.Root, obj1);
					if (obj2 != null)
					{
						return obj2;
					}
				}
			}
			using (IEnumerator<IDocumentRoot> enumerator = viewNode.DocumentNode.DocumentRoot.DesignTimeResources.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IInstanceBuilderContext instanceBuilderContext = context.GetViewContext(enumerator.Current);
					if (instanceBuilderContext == null)
					{
						continue;
					}
					obj2 = this.LookupInstantiatedResource(instanceBuilderContext, instanceBuilderContext.ViewNodeManager.Root, obj1);
					if (obj2 == null)
					{
						continue;
					}
					obj = obj2;
					return obj;
				}
				IViewResourceDictionary themeResources = context.Platform.ThemeResources;
				if (themeResources == null)
				{
					return null;
				}
				obj2 = themeResources.FindResource(obj1);
				return obj2;
			}
			return obj;
		}

		public virtual AttachmentOrder GetAttachmentOrder(IInstanceBuilderContext context, ViewNode viewNode)
		{
			return AttachmentOrder.PostInitialization;
		}

		public virtual ViewNode GetViewNode(IInstanceBuilderContext context, DocumentNode documentNode)
		{
			return new ViewNode(context.ViewNodeManager, documentNode);
		}

		public virtual void Initialize(IInstanceBuilderContext context, ViewNode viewNode, bool isNewInstance)
		{
			IDisposable disposable = null;
			try
			{
				if (viewNode.Instance != null && viewNode.InstanceState == InstanceState.Uninitialized)
				{
					DocumentCompositeNode documentNode = viewNode.DocumentNode as DocumentCompositeNode;
					DocumentCompositeNode documentCompositeNode = documentNode;
					if (documentNode != null)
					{
						if (context.ViewNodeManager.IsContainerRoot(viewNode))
						{
							disposable = context.ChangeContainerRoot(viewNode);
						}
						this.InstantiateProperties(context, viewNode, documentCompositeNode);
						if (documentCompositeNode.SupportsChildren)
						{
							this.InstantiateChildren(context, viewNode, documentCompositeNode, isNewInstance);
						}
					}
				}
				viewNode.InstanceState = InstanceState.Valid;
				if (!context.IsSerializationScope)
				{
					this.OnInitialized(context, viewNode, viewNode.Instance);
				}
			}
			finally
			{
				if (disposable != null)
				{
					disposable.Dispose();
					disposable = null;
				}
			}
		}

		public virtual bool Instantiate(IInstanceBuilderContext context, ViewNode viewNode)
		{
			IType type;
			object obj;
			bool flag = false;
			if (viewNode.Instance == null)
			{
				if (!context.IsSerializationScope)
				{
					viewNode.Instance = this.InstantiateTargetType(context, viewNode);
					flag = true;
				}
				else
				{
					DocumentNode documentNode = viewNode.DocumentNode;
					DocumentPrimitiveNode documentPrimitiveNode = documentNode as DocumentPrimitiveNode;
					if (documentPrimitiveNode == null)
					{
						type = (this.ReplacementType == null ? (IType)documentNode.Type.NearestResolvedType.Clone(context.DocumentContext.TypeResolver) : context.DocumentContext.TypeResolver.GetType(this.ReplacementType));
						DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode)documentNode;
						DocumentCompositeNode documentCompositeNode1 = new DocumentCompositeNode(context.DocumentContext, type, type.ItemType)
						{
							IsExplicitCollection = documentCompositeNode.IsExplicitCollection
						};
						if (viewNode is IInstantiatedElementViewNode && (PlatformTypes.UIElement.IsAssignableFrom(documentCompositeNode.Type) || PlatformTypes.FrameworkTemplate.IsAssignableFrom(documentCompositeNode.Type) || PlatformTypes.Style.IsAssignableFrom(documentCompositeNode.Type) || ClrObjectInstanceBuilder.IsTypeSupported(documentCompositeNode, PlatformTypes.FrameworkContentElement) || ClrObjectInstanceBuilder.IsTypeSupported(documentCompositeNode, PlatformTypes.Inline) || ClrObjectInstanceBuilder.IsTypeSupported(documentCompositeNode, PlatformTypes.Visual3D) || ClrObjectInstanceBuilder.IsTypeSupported(documentCompositeNode, PlatformTypes.Camera) || ClrObjectInstanceBuilder.IsTypeSupported(documentCompositeNode, PlatformTypes.Model3D) || ClrObjectInstanceBuilder.IsTypeSupported(documentCompositeNode, ProjectNeutralTypes.VisualTransition) || ClrObjectInstanceBuilder.IsTypeSupported(documentCompositeNode, ProjectNeutralTypes.Behavior) || ClrObjectInstanceBuilder.IsTypeSupported(documentCompositeNode, ProjectNeutralTypes.BehaviorTriggerBase) || ClrObjectInstanceBuilder.IsTypeSupported(documentCompositeNode, ProjectNeutralTypes.BehaviorTriggerAction) || ClrObjectInstanceBuilder.IsTypeSupported(documentCompositeNode, ProjectNeutralTypes.ComparisonCondition) || !documentCompositeNode.TypeResolver.IsCapabilitySet(PlatformCapability.IsWpf) && (PlatformTypes.RowDefinition.IsAssignableFrom(documentCompositeNode.Type) || PlatformTypes.ColumnDefinition.IsAssignableFrom(documentCompositeNode.Type))))
						{
							ViewNodeId id = context.SerializationContext.GetId(viewNode);
							documentCompositeNode1.Properties[DesignTimeProperties.ViewNodeIdProperty] = context.DocumentContext.CreateNode(typeof(string), ViewNodeManager.ViewNodeIdConverter.ConvertToInvariantString(id));
						}
						viewNode.Instance = documentCompositeNode1;
					}
					else
					{
						DependencyPropertyReferenceStep valueAsMember = DocumentPrimitiveNode.GetValueAsMember(documentPrimitiveNode) as DependencyPropertyReferenceStep;
						if (valueAsMember == null)
						{
							DocumentNode documentNode1 = documentPrimitiveNode.Clone(context.DocumentContext);
							documentNode1.SourceContext = null;
							viewNode.Instance = documentNode1;
						}
						else
						{
							valueAsMember = (DependencyPropertyReferenceStep)valueAsMember.Clone(context.DocumentContext.TypeResolver);
							DependencyPropertyReferenceStep shadowProperty = null;
							if (context.UseShadowProperties && (documentNode.Parent == null || !DocumentNodeUtilities.IsTemplateBinding(documentNode.Parent) || !context.DocumentContext.TypeResolver.IsCapabilitySet(PlatformCapability.WorkaroundSL20339)))
							{
								shadowProperty = DesignTimeProperties.GetShadowProperty(valueAsMember, valueAsMember.DeclaringTypeId);
								if (shadowProperty != null && !DesignTimeProperties.UseShadowPropertyForInstanceBuilding(context.DocumentContext.TypeResolver, shadowProperty))
								{
									shadowProperty = null;
								}
							}
							ViewNode viewNode1 = viewNode;
							if (shadowProperty == null)
							{
								obj = documentPrimitiveNode.Clone(context.DocumentContext);
							}
							else
							{
								obj = context.DocumentContext.CreateNode(PlatformTypes.DependencyProperty, new DocumentNodeMemberValue(shadowProperty));
							}
							viewNode1.Instance = obj;
						}
						viewNode.InstanceState = InstanceState.Valid;
					}
				}
			}
			return flag;
		}

		protected object InstantiateAndInitializeChild(IInstanceBuilderContext context, ViewNode viewNode, int childIndex)
		{
			IInstanceBuilder instanceBuilder;
			ViewNode viewNode1;
			bool flag;
			DocumentNode item = ((DocumentCompositeNode)viewNode.DocumentNode).Children[childIndex];
			object obj = this.UpdatePropertyOrChildValue(context, viewNode, null, childIndex, DocumentNodeChangeAction.Add, item, out viewNode1, out instanceBuilder, out flag);
			if (viewNode1 != null && viewNode1.InstanceState == InstanceState.Uninitialized)
			{
				context.ViewNodeManager.InitializeInstance(instanceBuilder, viewNode1, flag);
			}
			return obj;
		}

		protected object InstantiateChild(IInstanceBuilderContext context, ViewNode viewNode, int childIndex, out ViewNode childViewNode, out IInstanceBuilder childBuilder, out bool isNewInstance)
		{
			DocumentNode item = ((DocumentCompositeNode)viewNode.DocumentNode).Children[childIndex];
			return this.UpdatePropertyOrChildValue(context, viewNode, null, childIndex, DocumentNodeChangeAction.Add, item, out childViewNode, out childBuilder, out isNewInstance);
		}

		protected virtual void InstantiateChildren(IInstanceBuilderContext context, ViewNode viewNode, DocumentCompositeNode compositeNode, bool isNewInstance)
		{
			IInstanceBuilder instanceBuilder;
			ViewNode viewNode1;
			bool flag;
			IInstanceBuilder instanceBuilder1;
			ViewNode viewNode2;
			bool flag1;
			if (context.IsSerializationScope)
			{
				DocumentCompositeNode instance = (DocumentCompositeNode)viewNode.Instance;
				instance.Children.Clear();
				for (int i = 0; i < compositeNode.Children.Count; i++)
				{
					DocumentNode documentNode = (DocumentNode)this.InstantiateChild(context, viewNode, i, out viewNode1, out instanceBuilder, out flag);
					instance.Children.Add(documentNode);
					if (viewNode1 != null && viewNode1.InstanceState == InstanceState.Uninitialized)
					{
						context.ViewNodeManager.InitializeInstance(instanceBuilder, viewNode1, flag);
					}
				}
				return;
			}
			IList listAdapter = InstanceBuilderOperations.GetListAdapter(viewNode.Instance);
			if (listAdapter == null)
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string instanceBuilderTypeNotAList = ExceptionStringTable.InstanceBuilderTypeNotAList;
				object[] name = new object[] { viewNode.Instance.GetType().Name };
				throw new NotSupportedException(string.Format(currentCulture, instanceBuilderTypeNotAList, name));
			}
			if (!isNewInstance || compositeNode.Children.Count > 0)
			{
				listAdapter.Clear();
			}
			for (int j = 0; j < compositeNode.Children.Count; j++)
			{
				object obj = this.InstantiateChild(context, viewNode, j, out viewNode2, out instanceBuilder1, out flag1);
				if (obj != ClrObjectInstanceBuilder.InvalidObjectSentinel)
				{
					try
					{
						if (!listAdapter.IsFixedSize)
						{
							listAdapter.Add(obj);
						}
						else
						{
							listAdapter[j] = obj;
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						throw new InstanceBuilderException(exception.Message, exception, compositeNode.Children[j], viewNode);
					}
					if (viewNode2 != null && viewNode2.InstanceState == InstanceState.Uninitialized)
					{
						context.ViewNodeManager.InitializeInstance(instanceBuilder1, viewNode2, flag1);
					}
				}
			}
		}

		protected virtual void InstantiateProperties(IInstanceBuilderContext context, ViewNode viewNode, DocumentCompositeNode compositeNode)
		{
			ISupportInitialize supportInitialize = context.Platform.Metadata.GetISupportInitialize(viewNode.Instance);
			if (supportInitialize != null)
			{
				this.BeginInit(context, viewNode, compositeNode, supportInitialize);
			}
			try
			{
				IProperty property = null;
				IPropertyId resourcesProperty = viewNode.Type.Metadata.ResourcesProperty;
				if (resourcesProperty != null)
				{
					property = context.DocumentContext.TypeResolver.ResolveProperty(resourcesProperty);
					if (property != null)
					{
						DocumentNode propertyValue = context.GetPropertyValue(viewNode, property);
						if (propertyValue != null)
						{
							this.UpdateProperty(context, viewNode, property, propertyValue);
						}
					}
				}
				foreach (IProperty property1 in context.GetProperties(viewNode))
				{
					if (property == property1)
					{
						continue;
					}
					DocumentNode documentNode = context.GetPropertyValue(viewNode, property1);
					this.UpdateProperty(context, viewNode, property1, documentNode);
				}
			}
			finally
			{
				if (supportInitialize != null)
				{
					this.EndInit(context, viewNode, compositeNode, supportInitialize);
				}
			}
		}

		protected virtual object InstantiateTargetType(IInstanceBuilderContext context, ViewNode viewNode)
		{
			Type runtimeType;
			DocumentNode documentNode = viewNode.DocumentNode;
			object routedEvent = null;
			DocumentPrimitiveNode documentPrimitiveNode = documentNode as DocumentPrimitiveNode;
			if (documentPrimitiveNode == null)
			{
				bool flag = false;
				Type replacementType = this.ReplacementType;
				if (viewNode.Parent == null && context.RootTargetTypeReplacement != null && viewNode.Type.Equals(context.RootTargetTypeReplacement.SourceType))
				{
					runtimeType = context.RootTargetTypeReplacement.ReplacementType.RuntimeType;
				}
				else if (replacementType == null)
				{
					runtimeType = documentNode.TargetType;
					flag = documentNode.TypeResolver.InTargetAssembly(documentNode.Type);
				}
				else
				{
					runtimeType = replacementType;
				}
				routedEvent = InstanceBuilderOperations.InstantiateType(runtimeType, flag);
			}
			else
			{
				IDocumentNodeValue value = documentPrimitiveNode.Value;
				if (value != null)
				{
					DocumentNodeStringValue documentNodeStringValue = value as DocumentNodeStringValue;
					DocumentNodeStringValue documentNodeStringValue1 = documentNodeStringValue;
					if (documentNodeStringValue == null)
					{
						DocumentNodeCloneableValue documentNodeCloneableValue = value as DocumentNodeCloneableValue;
						DocumentNodeCloneableValue documentNodeCloneableValue1 = documentNodeCloneableValue;
						if (documentNodeCloneableValue == null)
						{
							DocumentNodeMemberValue documentNodeMemberValue = value as DocumentNodeMemberValue;
							DocumentNodeMemberValue documentNodeMemberValue1 = documentNodeMemberValue;
							if (documentNodeMemberValue == null)
							{
								throw new InstanceBuilderException(ExceptionStringTable.InstanceBuilderUnexpectedValue, documentNode);
							}
							IMember member = documentNodeMemberValue1.Member;
							IType type = member as IType;
							IType type1 = type;
							if (type == null)
							{
								DependencyPropertyReferenceStep dependencyPropertyReferenceStep = member as DependencyPropertyReferenceStep;
								DependencyPropertyReferenceStep dependencyPropertyReferenceStep1 = dependencyPropertyReferenceStep;
								if (dependencyPropertyReferenceStep == null)
								{
									ClrPropertyReferenceStep clrPropertyReferenceStep = member as ClrPropertyReferenceStep;
									ClrPropertyReferenceStep clrPropertyReferenceStep1 = clrPropertyReferenceStep;
									if (clrPropertyReferenceStep == null)
									{
										FieldReferenceStep fieldReferenceStep = member as FieldReferenceStep;
										FieldReferenceStep fieldReferenceStep1 = fieldReferenceStep;
										if (fieldReferenceStep == null)
										{
											Event @event = member as Event;
											Event event1 = @event;
											if (@event != null)
											{
												routedEvent = event1.RoutedEvent;
												if (routedEvent == null)
												{
													routedEvent = event1.EventInfo;
												}
											}
										}
										else
										{
											FieldInfo fieldInfo = fieldReferenceStep1.FieldInfo;
											if (fieldInfo.IsStatic)
											{
												routedEvent = fieldInfo.GetValue(null);
											}
										}
									}
									else
									{
										MethodInfo getMethod = clrPropertyReferenceStep1.GetMethod;
										if (getMethod.IsStatic)
										{
											routedEvent = getMethod.Invoke(null, null);
										}
									}
								}
								else
								{
									DependencyPropertyReferenceStep shadowProperty = null;
									if (context.UseShadowProperties)
									{
										shadowProperty = DesignTimeProperties.GetShadowProperty(dependencyPropertyReferenceStep1, dependencyPropertyReferenceStep1.DeclaringTypeId);
										if (shadowProperty != null && !DesignTimeProperties.UseShadowPropertyForInstanceBuilding(context.DocumentContext.TypeResolver, shadowProperty))
										{
											shadowProperty = null;
										}
									}
									routedEvent = (shadowProperty != null ? shadowProperty.DependencyProperty : dependencyPropertyReferenceStep1.DependencyProperty);
								}
							}
							else
							{
								routedEvent = type1.NearestResolvedType.RuntimeType;
							}
						}
						else
						{
							routedEvent = documentNodeCloneableValue1.CloneValue();
						}
					}
					else
					{
						string str = documentNodeStringValue1.Value;
						if (str != null)
						{
							Type targetType = documentPrimitiveNode.TargetType;
							if (!targetType.IsAssignableFrom(typeof(string)))
							{
								if (typeof(Delegate).IsAssignableFrom(targetType))
								{
									return null;
								}
								TypeConverter valueConverter = documentPrimitiveNode.ValueConverter;
								if (valueConverter != null && valueConverter.CanConvertFrom(typeof(string)))
								{
									try
									{
										routedEvent = valueConverter.ConvertFromInvariantString(str);
									}
									catch (Exception exception1)
									{
										Exception exception = exception1;
										CultureInfo currentCulture = CultureInfo.CurrentCulture;
										string instanceBuilderCannotConvertValue = ExceptionStringTable.InstanceBuilderCannotConvertValue;
										object[] objArray = new object[] { str };
										throw new InstanceBuilderException(string.Format(currentCulture, instanceBuilderCannotConvertValue, objArray), exception, documentPrimitiveNode);
									}
								}
							}
							else
							{
								routedEvent = str;
							}
						}
					}
				}
				else
				{
					routedEvent = null;
				}
			}
			return routedEvent;
		}

		protected static bool IsPropertyWritable(DocumentNode node, IProperty property)
		{
			return TypeHelper.IsPropertyWritable(node.TypeResolver, property, node.IsSubclassDefinition);
		}

		private static bool IsTypeSupported(DocumentNode node, ITypeId type)
		{
			if (!node.TypeResolver.PlatformMetadata.IsSupported(node.TypeResolver, type))
			{
				return false;
			}
			return type.IsAssignableFrom(node.Type);
		}

		private object LookupInstantiatedResource(IInstanceBuilderContext context, ViewNode viewNode, object key)
		{
			if (viewNode == null || viewNode.Instance == null || viewNode.InstanceState != InstanceState.Valid && viewNode.InstanceState != InstanceState.Uninitialized)
			{
				return null;
			}
			ViewNode item = null;
			if (!PlatformTypes.ResourceDictionary.IsAssignableFrom(viewNode.Type))
			{
				IType type = viewNode.Type;
				if (type.Metadata.ResourcesProperty == null)
				{
					return null;
				}
				IProperty property = viewNode.DocumentNode.TypeResolver.ResolveProperty(type.Metadata.ResourcesProperty);
				item = viewNode.Properties[property];
			}
			else
			{
				item = viewNode;
			}
			if (item == null)
			{
				return null;
			}
			if (item.Instance != null && (item.InstanceState == InstanceState.Valid || item.InstanceState == InstanceState.Uninitialized))
			{
				IViewObjectFactory viewObjectFactory = context.Platform.ViewObjectFactory;
				IViewResourceDictionary viewResourceDictionary = viewObjectFactory.Instantiate(item.Instance) as IViewResourceDictionary;
				if (viewResourceDictionary == null)
				{
					return null;
				}
				return viewResourceDictionary.FindResource(key);
			}
			IProperty property1 = item.TypeResolver.ResolveProperty(Microsoft.Expression.DesignModel.Metadata.KnownProperties.ResourceDictionaryMergedDictionariesProperty);
			ViewNode item1 = item.Properties[property1];
			object obj = null;
			if (item1 != null)
			{
				using (IEnumerator<ViewNode> enumerator = item1.Children.GetEnumerator())
				{
					do
					{
						if (!enumerator.MoveNext())
						{
							break;
						}
						obj = this.LookupInstantiatedResource(context, enumerator.Current, key);
					}
					while (obj == null);
				}
			}
			return obj;
		}

		public virtual void ModifyValue(IInstanceBuilderContext context, ViewNode target, object onlyThisInstance, IProperty propertyKey, object value, PropertyModification modification)
		{
			IProperty shadowProperty;
			if (context.UseShadowProperties)
			{
				shadowProperty = DesignTimeProperties.GetShadowProperty(propertyKey, target.DocumentNode.Type);
			}
			else
			{
				shadowProperty = null;
			}
			IProperty property = shadowProperty;
			if (property != null && DesignTimeProperties.UseShadowPropertyForInstanceBuilding(target.TypeResolver, property))
			{
				propertyKey = property;
			}
			object obj = onlyThisInstance ?? target.Instance;
			if (context.RootTargetTypeReplacement != null && !context.IsSerializationScope && obj != null && obj.GetType() == context.RootTargetTypeReplacement.ReplacementType.RuntimeType)
			{
				propertyKey = context.RootTargetTypeReplacement.GetReplacementProperty(propertyKey);
			}
			if (modification != PropertyModification.Set)
			{
				InstanceBuilderOperations.ClearValue(obj, propertyKey);
			}
			else if (ClrObjectInstanceBuilder.ShouldSetPropertyExplicitly(context, target, propertyKey))
			{
				InstanceBuilderOperations.SetValue(obj, propertyKey, value);
				if (DesignTimeProperties.XNameProperty.Equals(propertyKey) && context.NameScope != null && value != null)
				{
					string str = value as string;
					if (str != null)
					{
						context.NameScope.RegisterName(str, obj);
						return;
					}
				}
			}
		}

		public virtual void OnChildRemoving(IInstanceBuilderContext context, ViewNode parent, ViewNode child)
		{
			context.EffectManager.UnregisterEffect(parent, child);
		}

		public virtual void OnDescendantUpdated(IInstanceBuilderContext context, ViewNode target, ViewNode child, InstanceState childState)
		{
		}

		public virtual void OnInitialized(IInstanceBuilderContext context, ViewNode target, object instance)
		{
		}

		public virtual void OnViewNodeInvalidating(IInstanceBuilderContext context, ViewNode target, ViewNode child, ref bool doesInvalidRootsContainTarget, List<ViewNode> invalidRoots)
		{
			InstanceState instanceState = target.InstanceState;
			if (target.Instance is DocumentNode && instanceState.IsPropertyOrChildInvalid && instanceState != InstanceState.Invalid)
			{
				InstanceBuilderOperations.SetInvalid(context, target, ref doesInvalidRootsContainTarget, invalidRoots);
			}
			bool flag = false;
			if (instanceState.IsPropertyOrChildInvalid && target.TargetType.IsValueType)
			{
				flag = true;
			}
			if (instanceState.IsPropertyInvalid)
			{
				foreach (IProperty invalidProperty in instanceState.InvalidProperties)
				{
					if (ClrObjectInstanceBuilder.ShouldSetPropertyExplicitly(context, target, invalidProperty) || context.GetPropertyValue(target, invalidProperty) != null)
					{
						continue;
					}
					flag = true;
					break;
				}
			}
			if (instanceState.IsChildInvalid)
			{
				IList listAdapter = InstanceBuilderOperations.GetListAdapter(target.Instance);
				if (listAdapter != null && listAdapter.Count != target.Children.Count)
				{
					flag = true;
				}
			}
			if (target.Type.IsResource)
			{
				flag = true;
			}
			if (flag)
			{
				InstanceBuilderOperations.SetInvalid(context, target, ref doesInvalidRootsContainTarget, invalidRoots);
			}
		}

		protected virtual void ReplaceChild(IList list, int childIndex, object childInstance)
		{
			try
			{
				list[childIndex] = childInstance;
			}
			catch (NotSupportedException notSupportedException)
			{
				list.RemoveAt(childIndex);
				list.Insert(childIndex, childInstance);
			}
		}

		public virtual bool ShouldReturnInvalidObjectSentinel(DocumentNode valueNode)
		{
			return DocumentNodeUtilities.IsStaticResource(valueNode);
		}

		protected static bool ShouldSetPropertyExplicitly(IInstanceBuilderContext context, ViewNode viewNode, IProperty propertyKey)
		{
			if (context.IsSerializationScope)
			{
				return true;
			}
			if (!ClrObjectInstanceBuilder.IsPropertyWritable(viewNode.DocumentNode, propertyKey))
			{
				return false;
			}
			IType propertyType = propertyKey.PropertyType;
			if (propertyType.ItemType == null)
			{
				return true;
			}
			if (propertyType.IsArray)
			{
				return true;
			}
			DocumentNode propertyValue = context.GetPropertyValue(viewNode, propertyKey);
			if (propertyValue == null)
			{
				return false;
			}
			DocumentCompositeNode documentCompositeNode = propertyValue as DocumentCompositeNode;
			if (documentCompositeNode == null)
			{
				return true;
			}
			if (propertyValue.Type.IsExpression)
			{
				return true;
			}
			return documentCompositeNode.IsExplicitCollection;
		}

		public virtual bool ShouldTryExpandExpression(IInstanceBuilderContext context, ViewNode viewNode, IPropertyId propertyKey, DocumentNode expressionNode)
		{
			ViewNode i;
			if (DocumentNodeUtilities.IsStaticResource(expressionNode))
			{
				return true;
			}
			if (!DocumentNodeUtilities.IsDynamicResource(expressionNode))
			{
				return false;
			}
			for (i = viewNode; i != null; i = i.Parent)
			{
				if (i.TargetType == typeof(Style))
				{
					DocumentCompositeNode documentNode = i.DocumentNode as DocumentCompositeNode;
					if (documentNode != null && documentNode.GetValue<bool>(DesignTimeProperties.IsDefaultStyleProperty))
					{
						return false;
					}
				}
			}
			i = viewNode.Parent;
			ViewNode viewNode1 = viewNode;
			while (i != null)
			{
				if (i.TargetType == typeof(DictionaryEntry) && (PlatformTypes.Style.IsAssignableFrom(viewNode1.Type) || PlatformTypes.FrameworkTemplate.IsAssignableFrom(viewNode1.Type)))
				{
					return false;
				}
				viewNode1 = i;
				i = i.Parent;
			}
			return true;
		}

		public virtual void UpdateChild(IInstanceBuilderContext context, ViewNode viewNode, int childIndex, DocumentNodeChangeAction action, DocumentNode childNode)
		{
			ViewNode viewNode1;
			IInstanceBuilder instanceBuilder;
			bool flag;
			IList listAdapter = InstanceBuilderOperations.GetListAdapter(viewNode.Instance);
			if (listAdapter == null)
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string instanceBuilderTypeNotAList = ExceptionStringTable.InstanceBuilderTypeNotAList;
				object[] name = new object[] { viewNode.Instance.GetType().Name };
				throw new NotSupportedException(string.Format(currentCulture, instanceBuilderTypeNotAList, name));
			}
			if (action == DocumentNodeChangeAction.Add || action == DocumentNodeChangeAction.Replace)
			{
				object obj = this.UpdatePropertyOrChildValue(context, viewNode, null, childIndex, action, childNode, out viewNode1, out instanceBuilder, out flag);
				if (action == DocumentNodeChangeAction.Replace)
				{
					this.ReplaceChild(listAdapter, childIndex, obj);
				}
				else if (listAdapter.Count != 0)
				{
					listAdapter.Insert(childIndex, obj);
				}
				else
				{
					listAdapter.Add(obj);
				}
				if (viewNode1 != null && viewNode1.InstanceState == InstanceState.Uninitialized)
				{
					context.ViewNodeManager.InitializeInstance(instanceBuilder, viewNode1, flag);
					return;
				}
			}
			else if (action == DocumentNodeChangeAction.Remove)
			{
				ViewNode item = viewNode.Children[childIndex];
				viewNode.Children.Remove(item);
				listAdapter.RemoveAt(childIndex);
				return;
			}
		}

		public virtual void UpdateInstance(IInstanceBuilderContext context, ViewNode viewNode)
		{
			InstanceState instanceState = viewNode.InstanceState;
			if (instanceState.IsChildInvalid)
			{
				DocumentNode item = null;
				if (instanceState.InvalidChildIndices == null)
				{
					if (instanceState.ChildAction == DocumentNodeChangeAction.Add || instanceState.ChildAction == DocumentNodeChangeAction.Replace)
					{
						item = ((DocumentCompositeNode)viewNode.DocumentNode).Children[instanceState.ChildIndex];
					}
					this.UpdateChild(context, viewNode, instanceState.ChildIndex, instanceState.ChildAction, item);
				}
				else
				{
					DocumentCompositeNode documentNode = (DocumentCompositeNode)viewNode.DocumentNode;
					foreach (int invalidChildIndex in instanceState.InvalidChildIndices)
					{
						item = documentNode.Children[invalidChildIndex];
						this.UpdateChild(context, viewNode, invalidChildIndex, instanceState.ChildAction, item);
					}
				}
			}
			if (instanceState.IsPropertyInvalid)
			{
				this.UpdateProperties(context, viewNode, instanceState.InvalidProperties);
			}
			viewNode.InstanceState = InstanceState.Valid;
		}

		public virtual void UpdateProperties(IInstanceBuilderContext context, ViewNode viewNode, IList<IProperty> properties)
		{
			foreach (IProperty property in properties)
			{
				DocumentNode item = ((DocumentCompositeNode)viewNode.DocumentNode).Properties[property];
				this.UpdateProperty(context, viewNode, property, item);
			}
		}

		public virtual void UpdateProperty(IInstanceBuilderContext context, ViewNode viewNode, IProperty propertyKey, DocumentNode valueNode)
		{
			ViewNode item;
			ViewNode viewNode1;
			IInstanceBuilder instanceBuilder;
			bool flag;
			if (viewNode.Properties[propertyKey] != null)
			{
				viewNode.Properties.Remove(propertyKey);
			}
			if (DesignTimeProperties.XNameProperty.Equals(propertyKey) && viewNode.DocumentNode.TypeResolver.IsCapabilitySet(PlatformCapability.SetInstanceNameInResourceDictionary) && !PlatformTypes.DependencyObject.IsAssignableFrom(viewNode.Type) && viewNode.IsProperty && Microsoft.Expression.DesignModel.Metadata.KnownProperties.DictionaryEntryValueProperty.Equals(viewNode.SitePropertyKey))
			{
				InstanceBuilderOperations.UpdatePropertyWithoutApply(context, viewNode, propertyKey, valueNode);
				return;
			}
			if (valueNode == null)
			{
				this.ModifyValue(context, viewNode, null, propertyKey, null, PropertyModification.Clear);
			}
			else
			{
				object obj = this.UpdatePropertyOrChildValue(context, viewNode, propertyKey, -1, DocumentNodeChangeAction.Add, valueNode, out viewNode1, out instanceBuilder, out flag);
				if (obj != ClrObjectInstanceBuilder.InvalidObjectSentinel)
				{
					try
					{
						this.ModifyValue(context, viewNode, null, propertyKey, obj, PropertyModification.Set);
					}
					catch (TargetInvocationException targetInvocationException1)
					{
						TargetInvocationException targetInvocationException = targetInvocationException1;
						Exception innerException = targetInvocationException;
						if (targetInvocationException.InnerException != null && viewNode.DocumentNode.TypeResolver.IsCapabilitySet(PlatformCapability.PreferInnerExceptions))
						{
							innerException = targetInvocationException.InnerException;
						}
						item = viewNode.Properties[propertyKey];
						viewNode.ViewNodeManager.OnException(item ?? viewNode, new InstanceBuilderException(innerException.Message, innerException, valueNode), false);
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						item = viewNode.Properties[propertyKey];
						viewNode.ViewNodeManager.OnException(item ?? viewNode, new InstanceBuilderException(exception.Message, exception, valueNode), false);
					}
					if (viewNode1 != null && viewNode1.InstanceState == InstanceState.Uninitialized)
					{
						context.ViewNodeManager.InitializeInstance(instanceBuilder, viewNode1, flag);
						return;
					}
				}
			}
		}

		protected object UpdatePropertyOrChildValue(IInstanceBuilderContext context, ViewNode viewNode, IProperty propertyKey, int childIndex, DocumentNodeChangeAction action, DocumentNode valueNode, out ViewNode childViewNode, out IInstanceBuilder valueBuilder, out bool isNewInstance)
		{
			bool flag;
			List<IDocumentRoot> documentRoots = null;
			IDocumentRoot documentRoot = valueNode.DocumentRoot;
			bool flag1 = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			object obj = null;
			DocumentNodePath documentNodePath = null;
			childViewNode = null;
			valueBuilder = null;
			isNewInstance = false;
			if (!valueNode.Type.IsExpression || !this.ShouldTryExpandExpression(context, viewNode, propertyKey, valueNode))
			{
				DocumentPrimitiveNode documentPrimitiveNode = valueNode as DocumentPrimitiveNode;
				if (documentPrimitiveNode == null || !(documentPrimitiveNode.Value is DocumentNodeReferenceValue))
				{
					goto Label0;
				}
			}
			ExpressionEvaluator expressionEvaluator = new ExpressionEvaluator(context.DocumentRootResolver);
			DocumentNode documentNode = null;
			if (DocumentNodeUtilities.IsDynamicResource(valueNode) || DocumentNodeUtilities.IsStaticResource(valueNode))
			{
				DocumentNode resourceKey = ResourceNodeHelper.GetResourceKey((DocumentCompositeNode)valueNode);
				if (resourceKey != null)
				{
					if (DocumentNodeUtilities.IsStaticExtension(resourceKey))
					{
						DocumentCompositeNode documentCompositeNode = (DocumentCompositeNode)resourceKey;
						DocumentNode item = documentCompositeNode.Properties[resourceKey.TypeResolver.PlatformMetadata.KnownProperties.StaticExtensionMember];
						if (item != null)
						{
							IMember valueAsMember = DocumentPrimitiveNode.GetValueAsMember(item);
							if (valueAsMember != null)
							{
								Type runtimeType = valueAsMember.DeclaringType.RuntimeType;
								flag2 = (runtimeType == typeof(SystemColors) || runtimeType == typeof(SystemFonts) ? true : runtimeType == typeof(SystemParameters));
								IProperty property = valueAsMember as IProperty;
								if (property != null)
								{
									flag3 = typeof(ResourceKey).IsAssignableFrom(PlatformTypeHelper.GetPropertyType(property));
								}
							}
						}
					}
					if (typeof(ResourceKey).IsAssignableFrom(resourceKey.TargetType))
					{
						flag3 = true;
					}
					if (PlatformTypes.Type.IsAssignableFrom(resourceKey.Type))
					{
						flag4 = true;
					}
					if (!flag2)
					{
						documentRoots = new List<IDocumentRoot>();
						List<string> strs = new List<string>();
						DocumentNodePath correspondingNodePath = context.ViewNodeManager.GetCorrespondingNodePath(viewNode);
						documentNode = expressionEvaluator.EvaluateResourceAndCollectionPath(correspondingNodePath, ResourceNodeHelper.GetResourceType(valueNode), resourceKey, null, documentRoots, strs, out flag);
						if (documentNode == null && strs.Count > 0)
						{
							context.WarningDictionary.SetWarning(viewNode, valueNode, strs[0]);
						}
						if (documentNode == null && !flag)
						{
							obj = this.FindInstantiatedResource(context, viewNode, correspondingNodePath.Node, resourceKey);
						}
					}
				}
			}
			else
			{
				documentNode = expressionEvaluator.EvaluateExpression(context.ViewNodeManager.GetCorrespondingNodePath(viewNode), valueNode);
			}
			for (ViewNode i = viewNode; i != null && documentNode != null; i = i.Parent)
			{
				if (i.DocumentNode == documentNode)
				{
					throw new InstanceBuilderException(ExceptionStringTable.InstanceBuilderExpressionCycle, null, valueNode, viewNode);
				}
			}
			if (documentNode != null)
			{
				if (propertyKey == null)
				{
					IType itemType = viewNode.DocumentNode.Type.ItemType;
					if (itemType != null)
					{
						Type type = itemType.RuntimeType;
						if (type != null && !type.IsAssignableFrom(documentNode.TargetType))
						{
							CultureInfo currentUICulture = CultureInfo.CurrentUICulture;
							string instanceBuilderUnexpectedChildType = ExceptionStringTable.InstanceBuilderUnexpectedChildType;
							object[] name = new object[] { viewNode.TargetType.Name, documentNode.TargetType.Name };
							throw new InstanceBuilderException(string.Format(currentUICulture, instanceBuilderUnexpectedChildType, name), null, valueNode, viewNode);
						}
					}
				}
				else if (!PlatformTypeHelper.GetPropertyType(propertyKey).IsAssignableFrom(documentNode.TargetType))
				{
					if (!context.DocumentContext.TypeResolver.IsCapabilitySet(PlatformCapability.SupportsImplicitResourceTypeConversion) || !Microsoft.Expression.DesignModel.Metadata.KnownProperties.PathDataProperty.Equals(propertyKey) || !(documentNode.TargetType == typeof(string)))
					{
						CultureInfo currentCulture = CultureInfo.CurrentCulture;
						string instanceBuilderCannotApplyExpressionValue = ExceptionStringTable.InstanceBuilderCannotApplyExpressionValue;
						object[] targetType = new object[] { documentNode.TargetType, PlatformTypeHelper.GetPropertyType(propertyKey) };
						throw new InstanceBuilderException(string.Format(currentCulture, instanceBuilderCannotApplyExpressionValue, targetType), null, valueNode, viewNode);
					}
					object obj1 = propertyKey.TypeConverter.ConvertFromString(DocumentPrimitiveNode.GetValueAsString(documentNode));
					documentNode = documentNode.Context.CreateNode(typeof(PathGeometry), obj1);
				}
				if (!flag1 && propertyKey != null && valueNode != documentNode && documentNode != null && documentNode.DocumentRoot != null && context.AllowPostponingResourceEvaluation && (DocumentNodeUtilities.IsStaticResource(valueNode) || DocumentNodeUtilities.IsDynamicResource(valueNode)))
				{
					documentNodePath = new DocumentNodePath(documentNode.DocumentRoot.RootNode, documentNode);
					if (!this.AllowPostponedResourceUpdate(context, viewNode, propertyKey, documentNodePath))
					{
						documentNodePath = null;
					}
				}
				valueNode = documentNode;
			}
			else if (obj == null && !flag2 && (DocumentNodeUtilities.IsStaticResource(valueNode) || DocumentNodeUtilities.IsDynamicResource(valueNode)))
			{
				flag1 = true;
			}
		Label0:
			object value = null;
			valueBuilder = context.InstanceBuilderFactory.GetBuilder(valueNode.TargetType);
			childViewNode = valueBuilder.GetViewNode(context, valueNode);
			if (propertyKey != null)
			{
				ReferenceStep referenceStep = propertyKey as ReferenceStep;
				if (referenceStep != null && InstanceBuilderOperations.IsSupported(referenceStep) && !ClrObjectInstanceBuilder.ShouldSetPropertyExplicitly(context, viewNode, propertyKey))
				{
					value = referenceStep.GetValue(viewNode.Instance);
				}
				viewNode.Properties[propertyKey] = childViewNode;
				if (value != null)
				{
					childViewNode.Instance = value;
				}
			}
			else if (action != DocumentNodeChangeAction.Replace)
			{
				viewNode.Children.Insert(childIndex, childViewNode);
			}
			else
			{
				viewNode.Children[childIndex] = childViewNode;
			}
			if (documentNodePath != null)
			{
				context.ViewNodeManager.AddPostponedReference(childViewNode, documentNodePath);
				return ClrObjectInstanceBuilder.InvalidObjectSentinel;
			}
			if (obj != null && !context.IsSerializationScope)
			{
				childViewNode.Instance = obj;
				childViewNode.InstanceState = InstanceState.Valid;
				value = obj;
			}
			else if (valueBuilder.GetAttachmentOrder(context, childViewNode) != AttachmentOrder.PreInitialization)
			{
				value = context.ViewNodeManager.Instantiate(childViewNode);
			}
			else
			{
				isNewInstance = context.ViewNodeManager.CreateInstance(valueBuilder, childViewNode);
				value = childViewNode.Instance;
			}
			if (documentRoots != null && documentRoots.Count > 0)
			{
				foreach (IDocumentRoot documentRoot1 in documentRoots)
				{
					if (documentRoot1 == documentRoot)
					{
						continue;
					}
					context.ViewNodeManager.AddRelatedDocumentRoot(childViewNode, documentRoot1);
				}
			}
			if (!context.IsSerializationScope && (DocumentNodeUtilities.IsStaticResource(valueNode) && flag2 || flag1 && (flag4 || flag3)))
			{
				StaticResourceExtension staticResourceExtension = value as StaticResourceExtension;
				DynamicResourceExtension dynamicResourceExtension = value as DynamicResourceExtension;
				object obj2 = null;
				if (staticResourceExtension != null)
				{
					obj2 = context.EvaluateSystemResource(staticResourceExtension.ResourceKey);
					value = obj2;
				}
				if (dynamicResourceExtension != null)
				{
					obj2 = context.EvaluateSystemResource(dynamicResourceExtension.ResourceKey);
				}
				if (obj2 != null && (flag4 || flag3))
				{
					flag1 = false;
				}
			}
			else if (context.IsSerializationScope && flag1 && flag3)
			{
				flag1 = false;
			}
			if (flag1)
			{
				string empty = string.Empty;
				if (DocumentNodeUtilities.IsDynamicResource(valueNode) || DocumentNodeUtilities.IsStaticResource(valueNode))
				{
					DocumentNode resourceKey1 = ResourceNodeHelper.GetResourceKey((DocumentCompositeNode)valueNode);
					if (resourceKey1 != null)
					{
						IDocumentRoot documentRoot2 = resourceKey1.DocumentRoot;
						if (documentRoot2 != null)
						{
							empty = Microsoft.Expression.DesignModel.Markup.XamlSerializer.SerializeValue(documentRoot2, new DefaultXamlSerializerFilter(), resourceKey1, CultureInfo.CurrentCulture);
						}
					}
				}
				IWarningDictionary warningDictionary = context.WarningDictionary;
				CultureInfo cultureInfo = CultureInfo.CurrentCulture;
				string instanceBuilderCannotEvaluateResource = ExceptionStringTable.InstanceBuilderCannotEvaluateResource;
				object[] objArray = new object[] { empty };
				warningDictionary.SetWarning(childViewNode, valueNode, string.Format(cultureInfo, instanceBuilderCannotEvaluateResource, objArray));
				if (this.ShouldReturnInvalidObjectSentinel(valueNode))
				{
					value = ClrObjectInstanceBuilder.InvalidObjectSentinel;
				}
			}
			return value;
		}

		[Conditional("DEBUG")]
		protected virtual void VerifyTargetType(Type targetType)
		{
			if (!this.BaseType.IsAssignableFrom(targetType) && (!this.BaseType.IsGenericTypeDefinition || !PlatformTypes.IsGenericTypeDefinitionOf(this.BaseType, targetType)))
			{
				CultureInfo currentCulture = CultureInfo.CurrentCulture;
				string instanceBuilderTargetTypeDoesNotDeriveFromBuilderBaseType = ExceptionStringTable.InstanceBuilderTargetTypeDoesNotDeriveFromBuilderBaseType;
				object[] name = new object[] { targetType.Name, this.BaseType.Name };
				throw new InvalidOperationException(string.Format(currentCulture, instanceBuilderTargetTypeDoesNotDeriveFromBuilderBaseType, name));
			}
		}
	}
}