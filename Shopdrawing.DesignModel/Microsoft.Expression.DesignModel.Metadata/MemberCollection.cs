using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Expression.DesignModel.Metadata
{
	internal sealed class MemberCollection : IEnumerable<IMember>, IEnumerable
	{
		private const StringComparison MemberNameComparison = StringComparison.Ordinal;

		private static MemberCollection.MemberNameComparer<PropertyInfo> propertyInfoComparer;

		private static MemberCollection.MemberNameComparer<MethodInfo> methodInfoComparer;

		private static MemberCollection.MemberNameComparer<EventInfo> eventInfoComparer;

		private static MemberCollection.MemberNameComparer<FieldInfo> fieldInfoComparer;

		private readonly ITypeResolver typeResolver;

		private readonly IType type;

		private Type referenceType;

		private Dictionary<MemberCollection.MemberKey, IMember> members;

		private List<IProperty> properties;

		private List<IEvent> events;

		private MemberType initializedMembers;

		internal static BindingFlags StaticBindingFlags;

		private static BindingFlags InstanceOrStaticBindingFlags;

		private MemberAccessTypes CachedMemberAccess
		{
			get
			{
				return TypeHelper.GetAllowableMemberAccess(this.typeResolver, this.type) | MemberAccessTypes.Protected;
			}
		}

		public int Count
		{
			get
			{
				return this.members.Count;
			}
		}

		static MemberCollection()
		{
			MemberCollection.propertyInfoComparer = new MemberCollection.MemberNameComparer<PropertyInfo>();
			MemberCollection.methodInfoComparer = new MemberCollection.MemberNameComparer<MethodInfo>();
			MemberCollection.eventInfoComparer = new MemberCollection.MemberNameComparer<EventInfo>();
			MemberCollection.fieldInfoComparer = new MemberCollection.MemberNameComparer<FieldInfo>();
			MemberCollection.StaticBindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			MemberCollection.InstanceOrStaticBindingFlags = MemberCollection.StaticBindingFlags | BindingFlags.Instance;
		}

		public MemberCollection(ITypeResolver typeResolver, IType type)
		{
			this.typeResolver = typeResolver;
			this.type = type;
			this.members = new Dictionary<MemberCollection.MemberKey, IMember>();
		}

		public MemberCollection(ITypeResolver typeResolver, IType type, Type referenceType) : this(typeResolver, type)
		{
			this.referenceType = referenceType;
		}

		private void AddAllEventsIfNecessary()
		{
			if (this.events == null)
			{
				this.type.InitializeClass();
			}
			if (this.events == null)
			{
				this.events = new List<IEvent>();
				this.ApplyToAllEvents(new MemberCollection.ApplyToEventDelegate(this.AddEventImplementation));
			}
		}

		private void AddAllPropertiesIfNecessary()
		{
			if (this.properties == null)
			{
				this.type.InitializeClass();
				this.properties = new List<IProperty>();
				this.ApplyToAllProperties(new MemberCollection.ApplyToPropertyDelegate(this.AddPropertyImplementation));
				MemberCollection memberCollections = this;
				memberCollections.initializedMembers = memberCollections.initializedMembers | MemberType.Property;
			}
		}

		private void AddEvent(MemberCollection.MemberKey key, IEvent eventKey)
		{
			this.AddMember(key, eventKey);
			this.events.Add(eventKey);
		}

		private void AddEventImplementation(MemberCollection.MemberKey key, EventImplementationBase eventImplementation, ITypeId targetType, IType handlerType)
		{
			this.AddEvent(key, new Event(this.type, handlerType, eventImplementation));
		}

		private void AddIncompleteAttachedProperties()
		{
			IType type;
			IType type1;
			MemberCollection memberCollections = this;
			memberCollections.initializedMembers = memberCollections.initializedMembers | MemberType.IncompleteAttachedProperty;
			if (!this.typeResolver.IsCapabilitySet(PlatformCapability.IncompleteAttachedPropertiesInMarkupExtensions))
			{
				return;
			}
			Type reflectionType = PlatformTypeHelper.GetReflectionType(this.type);
			if (reflectionType == null)
			{
				return;
			}
			this.AddAllPropertiesIfNecessary();
			List<FieldInfo> fieldInfos = null;
			BindingFlags staticBindingFlags = MemberCollection.StaticBindingFlags;
			if (this.referenceType != null)
			{
				fieldInfos = new List<FieldInfo>(this.referenceType.GetFields(staticBindingFlags & (BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod | BindingFlags.CreateInstance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty | BindingFlags.ExactBinding | BindingFlags.SuppressChangeType | BindingFlags.OptionalParamBinding | BindingFlags.IgnoreReturn)));
				fieldInfos.Sort(MemberCollection.fieldInfoComparer);
				staticBindingFlags = staticBindingFlags & (BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod | BindingFlags.CreateInstance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty | BindingFlags.ExactBinding | BindingFlags.SuppressChangeType | BindingFlags.OptionalParamBinding | BindingFlags.IgnoreReturn);
				staticBindingFlags = staticBindingFlags | BindingFlags.FlattenHierarchy;
			}
			FieldInfo[] fields = reflectionType.GetFields(staticBindingFlags & (BindingFlags.IgnoreCase | BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod | BindingFlags.CreateInstance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty | BindingFlags.ExactBinding | BindingFlags.SuppressChangeType | BindingFlags.OptionalParamBinding | BindingFlags.IgnoreReturn));
			for (int i = 0; i < (int)fields.Length; i++)
			{
				FieldInfo fieldInfo = fields[i];
				string name = fieldInfo.Name;
				if (name.EndsWith("Property", StringComparison.Ordinal))
				{
					name = name.Substring(0, name.Length - 8);
					if (this.GetMemberInternal(MemberType.LocalProperty | MemberType.AttachedProperty | MemberType.DependencyProperty | MemberType.Property | MemberType.IncompleteAttachedProperty, name) == null && MemberCollection.ReferenceFieldExists(fieldInfos, fieldInfo))
					{
						DependencyPropertyImplementation propertyImplementation = (DependencyPropertyImplementation)((PlatformTypes)this.type.PlatformMetadata).GetPropertyImplementation(reflectionType, name, null, MemberAccessTypes.Public);
						if (propertyImplementation != null && MemberCollection.GetPropertyTypes(this.typeResolver, propertyImplementation, out type, out type1))
						{
							DependencyPropertyReferenceStep dependencyPropertyReferenceStep = new DependencyPropertyReferenceStep(this.type, name, type1, propertyImplementation);
							this.AddMember(new MemberCollection.MemberKey(MemberType.IncompleteAttachedProperty, name), dependencyPropertyReferenceStep);
						}
					}
				}
			}
		}

		public void AddMember(IMember memberId)
		{
			MemberType memberType = memberId.MemberType;
			MemberCollection.MemberKey memberKey = new MemberCollection.MemberKey(memberId.MemberType, memberId.UniqueName);
			this.AddMember(memberKey, memberId);
		}

		private void AddMember(MemberCollection.MemberKey key, IMember memberId)
		{
			this.members.Add(key, memberId);
		}

		private void AddOrReplaceEventImplementation(MemberCollection.MemberKey key, EventImplementationBase eventImplementation, ITypeId targetType, IType handlerType)
		{
			IMember member;
			if (!this.members.TryGetValue(key, out member))
			{
				this.AddEventImplementation(key, eventImplementation, targetType, handlerType);
			}
			else
			{
				IEventImplementation eventImplementation1 = member as IEventImplementation;
				if (eventImplementation1 != null)
				{
					eventImplementation1.Implementation = eventImplementation;
					return;
				}
			}
		}

		private bool AddOrReplacePropertyImplementation(MemberCollection.MemberKey key, PropertyImplementationBase propertyImplementation, ITypeId targetType, IType valueType)
		{
			IMember member;
			if (!this.members.TryGetValue(key, out member))
			{
				this.AddPropertyImplementation(key, propertyImplementation, targetType, valueType);
			}
			else
			{
				Property property = member as Property;
				if (property != null && !property.PropertyType.Equals(valueType))
				{
					this.members.Remove(key);
					this.AddPropertyImplementation(key, propertyImplementation, targetType, valueType);
					return false;
				}
				IPropertyImplementation propertyImplementation1 = member as IPropertyImplementation;
				if (propertyImplementation1 != null)
				{
					propertyImplementation1.Implementation = propertyImplementation;
				}
			}
			return true;
		}

		private void AddProperty(MemberCollection.MemberKey key, IProperty propertyKey)
		{
			this.AddMember(key, propertyKey);
			this.properties.Add(propertyKey);
		}

		private bool AddPropertyImplementation(MemberCollection.MemberKey key, PropertyImplementationBase propertyImplementation, ITypeId targetType, IType valueType)
		{
			IProperty dependencyPropertyReferenceStep = null;
			string name = propertyImplementation.Name;
			ClrPropertyImplementationBase clrPropertyImplementationBase = propertyImplementation as ClrPropertyImplementationBase;
			if (typeof(IDesignTimePropertyImplementor).IsAssignableFrom(propertyImplementation.DeclaringType))
			{
				dependencyPropertyReferenceStep = DesignTimeProperties.FromName(name, (PlatformTypes)this.type.PlatformMetadata, null);
			}
			else if (clrPropertyImplementationBase == null)
			{
				DependencyPropertyImplementationBase dependencyPropertyImplementationBase = propertyImplementation as DependencyPropertyImplementationBase;
				DependencyPropertyImplementationBase dependencyPropertyImplementationBase1 = dependencyPropertyImplementationBase;
				if (dependencyPropertyImplementationBase != null)
				{
					dependencyPropertyReferenceStep = new DependencyPropertyReferenceStep(this.type, name, valueType, dependencyPropertyImplementationBase1);
				}
			}
			else
			{
				dependencyPropertyReferenceStep = new ClrPropertyReferenceStep(this.type, name, valueType, clrPropertyImplementationBase, PropertySortValue.NoValue);
			}
			if (dependencyPropertyReferenceStep != null && !this.members.ContainsKey(key))
			{
				this.AddProperty(key, dependencyPropertyReferenceStep);
			}
			return true;
		}

		private void ApplyToAllEvents(MemberCollection.ApplyToEventDelegate action)
		{
			MethodInfo methodInfo;
			RoutedEventImplementationBase missingClrEventImplementation;
			Type reflectionType = PlatformTypeHelper.GetReflectionType(this.type);
			if (reflectionType != null)
			{
				MemberAccessTypes cachedMemberAccess = this.CachedMemberAccess;
				List<MemberCollection.EventDescription> eventDescriptions = new List<MemberCollection.EventDescription>();
				EventInfo[] events = reflectionType.GetEvents(MemberCollection.InstanceOrStaticBindingFlags);
				List<EventInfo> eventInfos = null;
				if (this.referenceType != null)
				{
					eventInfos = new List<EventInfo>(this.referenceType.GetEvents(MemberCollection.InstanceOrStaticBindingFlags));
					eventInfos.Sort(MemberCollection.eventInfoComparer);
				}
				EventInfo[] eventInfoArray = events;
				for (int i = 0; i < (int)eventInfoArray.Length; i++)
				{
					EventInfo eventInfo = eventInfoArray[i];
					if (MemberCollection.ReferenceEventExists(eventInfos, eventInfo))
					{
						eventDescriptions.Add(new MemberCollection.EventDescription(eventInfo.Name, eventInfo));
					}
				}
				MethodInfo[] methods = reflectionType.GetMethods(MemberCollection.StaticBindingFlags);
				Dictionary<string, MethodInfo> strs = null;
				MethodInfo[] methodInfoArray = methods;
				for (int j = 0; j < (int)methodInfoArray.Length; j++)
				{
					MethodInfo methodInfo1 = methodInfoArray[j];
					string attachedEventName = MemberCollection.GetAttachedEventName(methodInfo1, "Remove", "Handler");
					if (attachedEventName != null && MemberCollection.ReferenceMethodExists(this.referenceType, methodInfo1.Name, MemberCollection.StaticBindingFlags))
					{
						if (strs == null)
						{
							strs = new Dictionary<string, MethodInfo>();
						}
						strs[attachedEventName] = methodInfo1;
					}
				}
				if (strs != null)
				{
					MethodInfo[] methodInfoArray1 = methods;
					for (int k = 0; k < (int)methodInfoArray1.Length; k++)
					{
						MethodInfo methodInfo2 = methodInfoArray1[k];
						string str = MemberCollection.GetAttachedEventName(methodInfo2, "Add", "Handler");
						if (str != null && MemberCollection.ReferenceMethodExists(this.referenceType, methodInfo2.Name, MemberCollection.StaticBindingFlags) && strs.TryGetValue(str, out methodInfo))
						{
							eventDescriptions.Add(new MemberCollection.EventDescription(str, methodInfo2));
						}
					}
				}
				eventDescriptions.Sort();
				RoutedEventDescription[] routedEventDescriptions = ((PlatformTypes)this.type.PlatformMetadata).GetRoutedEventDescriptions(reflectionType);
				int num = 0;
				if (routedEventDescriptions != null)
				{
					Array.Sort<RoutedEventDescription>(routedEventDescriptions, new Comparison<RoutedEventDescription>(MemberCollection.CompareEvents));
					RoutedEventDescription[] routedEventDescriptionArray = routedEventDescriptions;
					for (int l = 0; l < (int)routedEventDescriptionArray.Length; l++)
					{
						RoutedEventDescription routedEventDescription = routedEventDescriptionArray[l];
						MemberCollection.EventDescription item = null;
						while (num < eventDescriptions.Count)
						{
							item = eventDescriptions[num];
							int num1 = MemberCollection.CompareEvents(item, routedEventDescription);
							if (num1 >= 0)
							{
								if (num1 <= 0)
								{
									break;
								}
								item = null;
								break;
							}
							else
							{
								this.ApplyToEventIfNecessary(cachedMemberAccess, MemberCollection.GetEventImplementation(item), action);
								num++;
							}
						}
						if (item == null)
						{
							missingClrEventImplementation = new MissingClrEventImplementation(routedEventDescription);
						}
						else
						{
							missingClrEventImplementation = new RoutedEventImplementation(routedEventDescription, MemberCollection.GetEventImplementation(item));
							num++;
						}
						this.ApplyToEventIfNecessary(cachedMemberAccess, missingClrEventImplementation, action);
					}
				}
				while (num < eventDescriptions.Count)
				{
					this.ApplyToEventIfNecessary(cachedMemberAccess, MemberCollection.GetEventImplementation(eventDescriptions[num]), action);
					num++;
				}
			}
		}

		private bool ApplyToAllProperties(MemberCollection.ApplyToPropertyDelegate action)
		{
			ClrPropertyImplementationBase attachedClrPropertyImplementation;
			IType type;
			IType type1;
			if (!this.type.IsBuilt)
			{
				return true;
			}
			Type reflectionType = PlatformTypeHelper.GetReflectionType(this.type);
			if (reflectionType == null)
			{
				return true;
			}
			List<MemberCollection.PropertyDescription> propertyDescriptions = new List<MemberCollection.PropertyDescription>();
			this.CollectLocalProperties(reflectionType, propertyDescriptions);
			this.CollectAttachedProperties(reflectionType, propertyDescriptions);
			bool flag = true;
			if (propertyDescriptions.Count > 0)
			{
				MemberAccessTypes cachedMemberAccess = this.CachedMemberAccess;
				propertyDescriptions.Sort();
				MemberCollection.UpdatePropertyOrder(reflectionType, propertyDescriptions);
				foreach (MemberCollection.PropertyDescription propertyDescription in propertyDescriptions)
				{
					PropertyInfo property = propertyDescription.Property;
					if (property == null)
					{
						attachedClrPropertyImplementation = new AttachedClrPropertyImplementation(this.type.PlatformMetadata, propertyDescription.Name, propertyDescription.GetMethod, propertyDescription.SetMethod);
					}
					else
					{
						attachedClrPropertyImplementation = new LocalClrPropertyImplementation(this.type.PlatformMetadata, property, reflectionType, null);
					}
					if (!TypeHelper.IsSet(cachedMemberAccess, attachedClrPropertyImplementation.ReadAccess) && !TypeHelper.IsSet(cachedMemberAccess, attachedClrPropertyImplementation.WriteAccess) || !MemberCollection.GetPropertyTypes(this.typeResolver, attachedClrPropertyImplementation, out type, out type1))
					{
						continue;
					}
					PropertyImplementationBase propertyImplementation = ((PlatformTypes)this.type.PlatformMetadata).GetPropertyImplementation(reflectionType, attachedClrPropertyImplementation.Name, attachedClrPropertyImplementation, MemberAccessTypes.All);
					MemberCollection.MemberKey memberKey = new MemberCollection.MemberKey(propertyImplementation.MemberType, propertyDescription.Name);
					flag = flag & action(memberKey, propertyImplementation, type, type1);
				}
			}
			return flag;
		}

		private bool ApplyToEventIfNecessary(MemberAccessTypes requiredAccess, EventImplementationBase implementation, MemberCollection.ApplyToEventDelegate action)
		{
			IType type;
			IType type1;
			if (TypeHelper.IsSet(requiredAccess, implementation.Access) && MemberCollection.GetEventTypes(this.typeResolver, implementation, out type, out type1))
			{
				MemberCollection.MemberKey memberKey = new MemberCollection.MemberKey(implementation.MemberType, implementation.Name);
				action(memberKey, implementation, type, type1);
			}
			return false;
		}

		private void CollectAttachedProperties(Type runtimeType, List<MemberCollection.PropertyDescription> propertyDescriptions)
		{
			List<MethodInfo> methodInfos;
			List<MethodInfo> methodInfos1;
			List<MethodInfo> methodInfos2 = null;
			BindingFlags staticBindingFlags = MemberCollection.StaticBindingFlags;
			if (this.referenceType != null)
			{
				methodInfos2 = new List<MethodInfo>(this.referenceType.GetMethods(staticBindingFlags));
				methodInfos2.Sort(MemberCollection.methodInfoComparer);
				staticBindingFlags = staticBindingFlags & (BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod | BindingFlags.CreateInstance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty | BindingFlags.ExactBinding | BindingFlags.SuppressChangeType | BindingFlags.OptionalParamBinding | BindingFlags.IgnoreReturn);
				staticBindingFlags = staticBindingFlags | BindingFlags.FlattenHierarchy;
			}
			MethodInfo[] methods = runtimeType.GetMethods(staticBindingFlags);
			Dictionary<string, List<MethodInfo>> strs = null;
			MethodInfo[] methodInfoArray = methods;
			for (int i = 0; i < (int)methodInfoArray.Length; i++)
			{
				MethodInfo methodInfo = methodInfoArray[i];
				string name = methodInfo.Name;
				if (name.StartsWith("Set", StringComparison.Ordinal))
				{
					string str = name.Substring(3);
					if (str.Length > 0 && MemberCollection.HasExpectedParameters(methodInfo, 2) && MemberCollection.ReferenceMethodExists(methodInfos2, methodInfo))
					{
						if (strs == null)
						{
							strs = new Dictionary<string, List<MethodInfo>>();
						}
						if (!strs.TryGetValue(str, out methodInfos))
						{
							methodInfos = new List<MethodInfo>();
							strs.Add(str, methodInfos);
						}
						methodInfos.Add(methodInfo);
					}
				}
			}
			MethodInfo[] methodInfoArray1 = methods;
			for (int j = 0; j < (int)methodInfoArray1.Length; j++)
			{
				MethodInfo methodInfo1 = methodInfoArray1[j];
				string name1 = methodInfo1.Name;
				if (name1.StartsWith("Get", StringComparison.Ordinal))
				{
					string str1 = name1.Substring(3);
					if (str1.Length > 0 && MemberCollection.HasExpectedParameters(methodInfo1, 1) && MemberCollection.ReferenceMethodExists(methodInfos2, methodInfo1))
					{
						MethodInfo item = null;
						if (strs != null && strs.TryGetValue(str1, out methodInfos1))
						{
							item = methodInfos1[0];
							strs.Remove(str1);
						}
						propertyDescriptions.Add(new MemberCollection.PropertyDescription(str1, methodInfo1, item));
					}
				}
			}
			if (strs != null)
			{
				foreach (KeyValuePair<string, List<MethodInfo>> keyValuePair in strs)
				{
					string key = keyValuePair.Key;
					MethodInfo item1 = keyValuePair.Value[0];
					propertyDescriptions.Add(new MemberCollection.PropertyDescription(key, null, item1));
				}
			}
		}

		private void CollectLocalProperties(Type runtimeType, List<MemberCollection.PropertyDescription> propertyDescriptions)
		{
            if (typeof(IDesignTimePropertyImplementor).IsAssignableFrom(runtimeType))
                return;
            List<PropertyInfo> sortedReferenceProperties = (List<PropertyInfo>)null;
            System.Reflection.BindingFlags bindingAttr = MemberCollection.InstanceOrStaticBindingFlags;
            if (this.referenceType != (Type)null)
            {
                sortedReferenceProperties = new List<PropertyInfo>((IEnumerable<PropertyInfo>)this.referenceType.GetProperties(bindingAttr));
                sortedReferenceProperties.Sort((IComparer<PropertyInfo>)MemberCollection.propertyInfoComparer);
                bindingAttr = bindingAttr & ~System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.FlattenHierarchy;
            }
            foreach (PropertyInfo propertyInfo in runtimeType.GetProperties(bindingAttr))
            {
                if (propertyInfo.Name == "Item")
                {
                    ParameterInfo[] indexParameters = PlatformTypeHelper.GetIndexParameters(propertyInfo);
                    if (indexParameters != null && indexParameters.Length > 0)
                        continue;
                }
                bool flag = true;
                MethodInfo[] accessors = propertyInfo.GetAccessors(true);
                for (int index = 0; index < accessors.Length && flag; ++index)
                    flag = accessors[index].IsPrivate;
                if (!flag && MemberCollection.ReferencePropertyExists(sortedReferenceProperties, propertyInfo))
                    propertyDescriptions.Add(new MemberCollection.PropertyDescription(propertyInfo.Name, propertyInfo));
            }
		}

		private static int CompareEvents(RoutedEventDescription a, RoutedEventDescription b)
		{
			return string.Compare(a.Name, b.Name, StringComparison.Ordinal);
		}

		private static int CompareEvents(MemberCollection.EventDescription a, RoutedEventDescription b)
		{
			return string.Compare(a.Name, b.Name, StringComparison.Ordinal);
		}

		private static bool CompareParametersShallow(ParameterInfo[] sourceParameters, ParameterInfo[] referenceParameters)
		{
			if (referenceParameters == sourceParameters)
			{
				return true;
			}
			if (referenceParameters == null)
			{
				if (sourceParameters == null)
				{
					return true;
				}
				return (int)sourceParameters.Length == 0;
			}
			if (sourceParameters == null)
			{
				if (referenceParameters == null)
				{
					return true;
				}
				return (int)referenceParameters.Length == 0;
			}
			if ((int)referenceParameters.Length != (int)sourceParameters.Length)
			{
				return false;
			}
			for (int i = 0; i < (int)sourceParameters.Length; i++)
			{
				ParameterInfo parameterInfo = sourceParameters[i];
				ParameterInfo parameterInfo1 = referenceParameters[i];
				if (!MemberCollection.CompareTypesShallow(parameterInfo.ParameterType, parameterInfo1.ParameterType))
				{
					return false;
				}
			}
			return true;
		}

		private static bool CompareTypesShallow(Type sourceType, Type referenceType)
		{
			return MemberCollection.CompareTypesShallow(sourceType, referenceType, null);
		}

		private static bool CompareTypesShallow(Type sourceType, Type referenceType, Dictionary<Type, Type> visitedTypes)
		{
			Type type;
			if (sourceType == referenceType)
			{
				return true;
			}
			if (sourceType == null || referenceType == null)
			{
				return false;
			}
			if (visitedTypes != null && visitedTypes.TryGetValue(sourceType, out type))
			{
				return type == referenceType;
			}
			if (!string.Equals(sourceType.Name, referenceType.Name, StringComparison.Ordinal))
			{
				return false;
			}
			if (!string.Equals(sourceType.Namespace, referenceType.Namespace, StringComparison.Ordinal))
			{
				return false;
			}
			if (sourceType.IsGenericType != referenceType.IsGenericType || sourceType.IsGenericTypeDefinition != referenceType.IsGenericTypeDefinition || sourceType.IsGenericParameter != referenceType.IsGenericParameter)
			{
				return false;
			}
			if (!sourceType.IsGenericType)
			{
				return true;
			}
			Type[] genericArguments = sourceType.GetGenericArguments();
			Type[] typeArray = referenceType.GetGenericArguments();
			if ((int)genericArguments.Length != (int)typeArray.Length)
			{
				return false;
			}
			if (sourceType.IsGenericTypeDefinition)
			{
				return true;
			}
			if (visitedTypes == null)
			{
				visitedTypes = new Dictionary<Type, Type>((int)genericArguments.Length);
			}
			visitedTypes[sourceType] = referenceType;
			for (int i = 0; i < (int)genericArguments.Length; i++)
			{
				if (!MemberCollection.CompareTypesShallow(genericArguments[i], typeArray[i], visitedTypes))
				{
					return false;
				}
			}
			return true;
		}

		private static string GetAttachedEventName(MethodInfo methodInfo, string startsWith, string endsWith)
		{
			string name = methodInfo.Name;
			int length = startsWith.Length + endsWith.Length;
			if (name.Length <= length || !name.StartsWith(startsWith, StringComparison.Ordinal) || !name.EndsWith(endsWith, StringComparison.Ordinal) || !MemberCollection.HasExpectedParameters(methodInfo, 2))
			{
				return null;
			}
			return name.Substring(startsWith.Length, name.Length - length);
		}

		public IEnumerator<IMember> GetEnumerator()
		{
			return this.members.Values.GetEnumerator();
		}

		private static EventImplementationBase GetEventImplementation(MemberCollection.EventDescription eventDescription)
		{
			EventInfo @event = eventDescription.Event;
			if (@event != null)
			{
				return new LocalEventImplementation(@event);
			}
			return new AttachedEventImplementation(eventDescription.Name, eventDescription.AddMethod);
		}

		public IEnumerable<IEvent> GetEvents(MemberAccessTypes access)
		{
			this.AddAllEventsIfNecessary();
			foreach (IEvent @event in this.events)
			{
				if (!TypeHelper.IsSet(access, @event.Access))
				{
					continue;
				}
				yield return @event;
			}
		}

		private static bool GetEventTypes(ITypeResolver typeResolver, EventImplementationBase implementation, out IType targetType, out IType handlerType)
		{
			IType type;
			IType type1;
			Type type2 = implementation.TargetType;
			if (type2 != null)
			{
				type = typeResolver.GetType(type2);
			}
			else
			{
				type = null;
			}
			targetType = type;
			type2 = implementation.HandlerType;
			if (type2 != null)
			{
				type1 = typeResolver.GetType(type2);
			}
			else
			{
				type1 = null;
			}
			handlerType = type1;
			if (!MemberCollection.IsResolvableType(typeResolver, targetType))
			{
				return false;
			}
			return MemberCollection.IsResolvableType(typeResolver, handlerType);
		}

		public IMemberId GetMember(MemberType memberTypes, string memberName, MemberAccessTypes access)
		{
			if (TypeHelper.IsSet(memberTypes, MemberType.Property))
			{
				this.AddAllPropertiesIfNecessary();
			}
			if (TypeHelper.IsSet(memberTypes, MemberType.Event))
			{
				this.AddAllEventsIfNecessary();
			}
			IMember memberRecursively = this.GetMemberRecursively(memberTypes, memberName);
			if (memberRecursively == null && TypeHelper.IsSet(memberTypes, MemberType.IncompleteAttachedProperty) && !TypeHelper.IsSet(this.initializedMembers, MemberType.IncompleteAttachedProperty))
			{
				this.AddIncompleteAttachedProperties();
				memberRecursively = this.GetMemberRecursively(memberTypes, memberName);
			}
			if (memberRecursively == null)
			{
				memberRecursively = this.GetNonPropertyOrEventMember(memberTypes, memberName);
			}
			if (memberRecursively != null && TypeHelper.IsSet(access, memberRecursively.Access))
			{
				return memberRecursively;
			}
			return null;
		}

		public IMember GetMemberByUniqueName(MemberType memberTypes, string uniqueName)
		{
			return this.GetMemberInternal(memberTypes, uniqueName);
		}

		private IMember GetMemberInternal(MemberType memberTypes, string memberName)
		{
			IMember member;
			for (int i = 1; i < 4096; i = i << 1)
			{
				if ((i & (int)memberTypes) != (int)MemberType.None)
				{
					MemberCollection.MemberKey memberKey = new MemberCollection.MemberKey((MemberType)i, memberName);
					if (this.members.TryGetValue(memberKey, out member))
					{
						return member;
					}
				}
			}
			return null;
		}

		private IMember GetMemberRecursively(MemberType memberTypes, string memberName)
		{
			IMember memberInternal = this.GetMemberInternal(memberTypes, memberName);
			if (memberInternal == null)
			{
				MemberType memberType = memberTypes & (MemberType.LocalProperty | MemberType.AttachedProperty | MemberType.LocalEvent | MemberType.AttachedEvent | MemberType.ClrEvent | MemberType.RoutedEvent | MemberType.Event);
				if (memberType != MemberType.None)
				{
					IType baseType = this.type.BaseType;
					if (baseType != null)
					{
						memberInternal = (IMember)baseType.GetMember(memberType, memberName, MemberAccessTypes.All);
					}
				}
			}
			return memberInternal;
		}

		private IMember GetNonPropertyOrEventMember(MemberType memberTypes, string memberName)
		{
			MemberType memberType = memberTypes & (MemberType.Type | MemberType.DesignTimeProperty | MemberType.Field | MemberType.Method | MemberType.Constructor | MemberType.Methods | MemberType.Other);
			if (memberType == MemberType.None)
			{
				return null;
			}
			Type reflectionType = PlatformTypeHelper.GetReflectionType(this.type.NearestResolvedType);
			if (reflectionType == null)
			{
				return null;
			}
			IMember member = PlatformTypeHelper.GetMember(this.typeResolver, reflectionType, memberType, memberName);
			if (member == null || this.referenceType == null)
			{
				return member;
			}
			Type type = PlatformTypeHelper.GetReflectionType(this.type);
			if (TypeHelper.IsSet(memberType, MemberType.Field))
			{
				FieldInfo fieldInfo = PlatformTypeHelper.GetFieldInfo(this.referenceType, memberName);
				if (fieldInfo == null)
				{
					return null;
				}
				FieldInfo fieldInfo1 = PlatformTypeHelper.GetFieldInfo(type, memberName);
				if (fieldInfo1 == null)
				{
					return null;
				}
				if (!MemberCollection.CompareTypesShallow(fieldInfo1.FieldType, fieldInfo.FieldType))
				{
					return null;
				}
			}
			else if (TypeHelper.IsSet(memberType, MemberType.Method))
			{
				MethodInfo method = PlatformTypeHelper.GetMethod(this.referenceType, memberName);
				if (method == null)
				{
					return null;
				}
				MethodInfo methodInfo = PlatformTypeHelper.GetMethod(type, memberName);
				if (methodInfo == null)
				{
					return null;
				}
				if (!MemberCollection.CompareParametersShallow(methodInfo.GetParameters(), method.GetParameters()) || !MemberCollection.CompareTypesShallow(methodInfo.ReturnType, method.ReturnType))
				{
					return null;
				}
			}
			return member;
		}

		public IEnumerable<IProperty> GetProperties(MemberAccessTypes access)
		{
			this.AddAllPropertiesIfNecessary();
			foreach (IProperty property in this.properties)
			{
				if (!TypeHelper.IsSet(access, property.Access))
				{
					continue;
				}
				yield return property;
			}
		}

		private static bool GetPropertyTypes(ITypeResolver typeResolver, PropertyImplementationBase implementation, out IType targetType, out IType valueType)
		{
			IType type;
			IType type1;
			Type type2 = implementation.TargetType;
			if (type2 != null)
			{
				type = typeResolver.GetType(type2);
			}
			else
			{
				type = null;
			}
			targetType = type;
			type2 = implementation.ValueType;
			if (type2 != null)
			{
				type1 = typeResolver.GetType(type2);
			}
			else
			{
				type1 = null;
			}
			valueType = type1;
			if (!MemberCollection.IsResolvableType(typeResolver, targetType))
			{
				return false;
			}
			return MemberCollection.IsResolvableType(typeResolver, valueType);
		}

		private static bool HasExpectedParameters(MethodInfo methodInfo, int expectedNumberOfParameters)
		{
			if (methodInfo.ContainsGenericParameters)
			{
				return false;
			}
			ParameterInfo[] parameters = PlatformTypeHelper.GetParameters(methodInfo);
			return (parameters != null ? (int)parameters.Length : 0) == expectedNumberOfParameters;
		}

		private static int IndexOf(List<MemberCollection.PropertyDescription> propertyDescriptions, string propertyName)
		{
			return propertyDescriptions.BinarySearch(new MemberCollection.PropertyDescription(propertyName, null, null));
		}

		private static bool IsResolvableType(ITypeResolver typeResolver, IType typeId)
		{
			if (typeId != null && typeId.IsResolvable)
			{
				IAssembly runtimeAssembly = typeId.RuntimeAssembly;
				if (runtimeAssembly != null)
				{
					if (typeId is PlatformType && typeResolver == ((IPlatformTypes)typeId.PlatformMetadata).DefaultTypeResolver || typeResolver.AssemblyReferences.Contains(runtimeAssembly) || typeId.PlatformMetadata.IsDesignToolType(typeId.RuntimeType))
					{
						if (typeId.IsGenericType)
						{
							IList<IType> genericTypeArguments = typeId.GetGenericTypeArguments();
							for (int i = 0; i < genericTypeArguments.Count; i++)
							{
								if (!MemberCollection.IsResolvableType(typeResolver, genericTypeArguments[i]))
								{
									return false;
								}
							}
						}
						return true;
					}
					Type runtimeType = typeId.RuntimeType;
					if (runtimeType != null && PlatformTypes.IsExpressionInteractiveType(runtimeType))
					{
						return true;
					}
				}
			}
			return false;
		}

		private static bool ReferenceEventExists(List<EventInfo> sortedReferenceEvents, EventInfo sourceEvent)
		{
			if (sortedReferenceEvents == null)
			{
				return true;
			}
			int num = sortedReferenceEvents.BinarySearch(sourceEvent, MemberCollection.eventInfoComparer);
			if (num < 0)
			{
				return false;
			}
			EventInfo item = sortedReferenceEvents[num];
			if (!MemberCollection.CompareTypesShallow(sourceEvent.EventHandlerType, item.EventHandlerType))
			{
				return false;
			}
			return true;
		}

		private static bool ReferenceFieldExists(List<FieldInfo> sortedReferenceFields, FieldInfo sourceField)
		{
			if (sortedReferenceFields == null)
			{
				return true;
			}
			int num = sortedReferenceFields.BinarySearch(sourceField, MemberCollection.fieldInfoComparer);
			if (num < 0)
			{
				return false;
			}
			FieldInfo item = sortedReferenceFields[num];
			if (!MemberCollection.CompareTypesShallow(sourceField.FieldType, item.FieldType))
			{
				return false;
			}
			return true;
		}

		public static bool ReferenceMethodExists(Type referenceType, string methodName, BindingFlags bindingFlags)
		{
			bool method;
			if (referenceType == null)
			{
				return true;
			}
			try
			{
				method = referenceType.GetMethod(methodName, bindingFlags) != null;
			}
			catch (AmbiguousMatchException ambiguousMatchException)
			{
				method = true;
			}
			return method;
		}

		private static bool ReferenceMethodExists(List<MethodInfo> sortedReferenceMethods, MethodInfo sourceMethod)
		{
			if (sortedReferenceMethods == null)
			{
				return true;
			}
			int num = sortedReferenceMethods.BinarySearch(sourceMethod, MemberCollection.methodInfoComparer);
			if (num < 0)
			{
				return false;
			}
			for (int i = num; i < sortedReferenceMethods.Count; i++)
			{
				MethodInfo item = sortedReferenceMethods[i];
				if (i != num && item.Name != sourceMethod.Name)
				{
					break;
				}
				if (MemberCollection.CompareTypesShallow(sourceMethod.ReturnType, item.ReturnType) && MemberCollection.CompareParametersShallow(sourceMethod.GetParameters(), item.GetParameters()))
				{
					return true;
				}
			}
			return false;
		}

		private static bool ReferencePropertyExists(List<PropertyInfo> sortedReferenceProperties, PropertyInfo sourceProperty)
		{
			if (sortedReferenceProperties == null)
			{
				return true;
			}
			int num = sortedReferenceProperties.BinarySearch(sourceProperty, MemberCollection.propertyInfoComparer);
			if (num < 0)
			{
				return false;
			}
			for (int i = num; i < sortedReferenceProperties.Count; i++)
			{
				PropertyInfo item = sortedReferenceProperties[num];
				if (i > num && item.Name != sourceProperty.Name)
				{
					break;
				}
				if (sourceProperty.CanWrite == item.CanWrite && MemberCollection.CompareTypesShallow(sourceProperty.PropertyType, item.PropertyType) && MemberCollection.CompareParametersShallow(sourceProperty.GetIndexParameters(), item.GetIndexParameters()))
				{
					return true;
				}
			}
			return false;
		}

		public bool Refresh()
		{
			bool flag;
			foreach (IMemberId value in this.members.Values)
			{
				if ((value.MemberType & (MemberType.Type | MemberType.DependencyProperty | MemberType.DesignTimeProperty | MemberType.Field | MemberType.Method | MemberType.Constructor | MemberType.Methods | MemberType.IncompleteAttachedProperty | MemberType.Other)) == MemberType.None)
				{
					continue;
				}
				ICachedMemberInfo cachedMemberInfo = value as ICachedMemberInfo;
				if (cachedMemberInfo == null)
				{
					continue;
				}
				cachedMemberInfo.Refresh();
			}
			bool flag1 = this.RefreshAllProperties();
			this.RefreshAllEvents();
			if (!flag1)
			{
				return false;
			}
			Dictionary<MemberCollection.MemberKey, IMember>.ValueCollection.Enumerator enumerator = this.members.Values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsResolvable)
					{
						continue;
					}
					flag = false;
					return flag;
				}
				return true;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		private void RefreshAllEvents()
		{
			if (this.events == null)
			{
				return;
			}
			this.type.InitializeClass();
			foreach (IEvent @event in this.events)
			{
				IEventImplementation eventImplementation = @event as IEventImplementation;
				if (eventImplementation == null)
				{
					continue;
				}
				eventImplementation.Invalidate();
			}
			this.ApplyToAllEvents(new MemberCollection.ApplyToEventDelegate(this.AddOrReplaceEventImplementation));
		}

		private bool RefreshAllProperties()
		{
			if (this.properties == null)
			{
				return true;
			}
			this.type.InitializeClass();
			foreach (IPropertyId property in this.properties)
			{
				IPropertyImplementation propertyImplementation = property as IPropertyImplementation;
				if (propertyImplementation == null)
				{
					continue;
				}
				propertyImplementation.Invalidate();
			}
			return this.ApplyToAllProperties(new MemberCollection.ApplyToPropertyDelegate(this.AddOrReplacePropertyImplementation));
		}

		private static MemberCollection.PropertyDescription Remove(List<MemberCollection.PropertyDescription> propertyDescriptions, string propertyName)
		{
			int num = MemberCollection.IndexOf(propertyDescriptions, propertyName);
			MemberCollection.PropertyDescription item = propertyDescriptions[num];
			propertyDescriptions.RemoveAt(num);
			return item;
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public override string ToString()
		{
			return this.type.ToString();
		}

		private static void UpdatePropertyOrder(Type type, List<MemberCollection.PropertyDescription> propertyDescriptions)
		{
			if (type != typeof(FrameworkElement))
			{
				if (type == typeof(Binding))
				{
					propertyDescriptions.Insert(0, MemberCollection.Remove(propertyDescriptions, "Path"));
				}
				return;
			}
			MemberCollection.PropertyDescription propertyDescription = MemberCollection.Remove(propertyDescriptions, "Height");
			int num = MemberCollection.IndexOf(propertyDescriptions, "Width");
			propertyDescriptions.Insert(num + 1, propertyDescription);
		}

		private delegate void ApplyToEventDelegate(MemberCollection.MemberKey key, EventImplementationBase eventKeyImplementation, ITypeId targetType, IType handlerType);

		private delegate bool ApplyToPropertyDelegate(MemberCollection.MemberKey key, PropertyImplementationBase propertyImplementation, ITypeId targetType, IType valueType);

		private sealed class EventDescription : IComparable<MemberCollection.EventDescription>
		{
			private string name;

			private MemberInfo @add;

			public MethodInfo AddMethod
			{
				get
				{
					return this.@add as MethodInfo;
				}
			}

			public EventInfo Event
			{
				get
				{
					return this.@add as EventInfo;
				}
			}

			public string Name
			{
				get
				{
					return this.name;
				}
			}

			public EventDescription(string name, EventInfo eventInfo)
			{
				this.name = name;
				this.@add = eventInfo;
			}

			public EventDescription(string name, MethodInfo add)
			{
				this.name = name;
				this.@add = add;
			}

			public int CompareTo(MemberCollection.EventDescription other)
			{
				return string.Compare(this.Name, other.Name, StringComparison.Ordinal);
			}
		}

		private struct MemberKey
		{
			private MemberType memberType;

			private string memberName;

			private int hashCode;

			public MemberKey(MemberType memberType, string memberName)
			{
				this.memberType = memberType;
				this.memberName = memberName;
				this.hashCode = (int)this.memberType ^ this.memberName.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				return (MemberCollection.MemberKey)obj == this;
			}

			public override int GetHashCode()
			{
				return this.hashCode;
			}

			public static bool operator ==(MemberCollection.MemberKey a, MemberCollection.MemberKey b)
			{
				if (a.memberType != b.memberType)
				{
					return false;
				}
				return a.memberName == b.memberName;
			}

			public static bool operator !=(MemberCollection.MemberKey a, MemberCollection.MemberKey b)
			{
				return !(a == b);
			}

			public override string ToString()
			{
				return this.memberName;
			}
		}

		private class MemberNameComparer<T> : IComparer<T>
		where T : MemberInfo
		{
			public MemberNameComparer()
			{
			}

			public int Compare(T x, T y)
			{
				return string.CompareOrdinal(x.Name, y.Name);
			}
		}

		private struct PropertyDescription : IComparable<MemberCollection.PropertyDescription>
		{
			private string name;

			private MemberInfo getter;

			private MemberInfo setter;

			public MethodInfo GetMethod
			{
				get
				{
					return this.getter as MethodInfo;
				}
			}

			public string Name
			{
				get
				{
					return this.name;
				}
			}

			public PropertyInfo Property
			{
				get
				{
					return this.getter as PropertyInfo;
				}
			}

			public MethodInfo SetMethod
			{
				get
				{
					return this.setter as MethodInfo;
				}
			}

			public PropertyDescription(string name, PropertyInfo propertyInfo)
			{
				this.name = name;
				this.getter = propertyInfo;
				this.setter = null;
			}

			public PropertyDescription(string name, MethodInfo getter, MethodInfo setter)
			{
				this.name = name;
				this.getter = getter;
				this.setter = setter;
			}

			public int CompareTo(MemberCollection.PropertyDescription other)
			{
				return string.Compare(this.Name, other.Name, StringComparison.Ordinal);
			}
		}
	}
}