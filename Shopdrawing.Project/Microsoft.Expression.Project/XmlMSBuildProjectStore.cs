using Microsoft.Expression.Framework.Documents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Microsoft.Expression.Project
{
	internal sealed class XmlMSBuildProjectStore : ProjectStoreBase
	{
		private XDocument document;

		private static XNamespace msbuild;

		private IEnumerable<XAttribute> ProjectImportAttributes
		{
			get
			{
				return 
					from project in this.document.Elements(XmlMSBuildProjectStore.msbuild + "Project")
					from import in project.Elements(XmlMSBuildProjectStore.msbuild + "Import")
					from projectAttribute in import.Attributes("Project")
					select projectAttribute;
			}
		}

		public override IEnumerable<string> ProjectImports
		{
			get
			{
				return 
					from import in this.ProjectImportAttributes
					select import.Value;
			}
		}

		private IEnumerable<XElement> ProjectPropertyElements
		{
			get
			{
				return 
					from propertyGroup in this.UnconditionedPropertyGroups
					from property in propertyGroup.Elements()
					select property;
			}
		}

		public override Version StoreVersion
		{
			get
			{
				Version version;
				XAttribute toolsVersionAttribute = this.ToolsVersionAttribute;
				if (toolsVersionAttribute == null)
				{
					return null;
				}
				if (Version.TryParse(toolsVersionAttribute.Value, out version))
				{
					return version;
				}
				return null;
			}
		}

		private XAttribute ToolsVersionAttribute
		{
			get
			{
				return (
					from project in this.document.Elements(XmlMSBuildProjectStore.msbuild + "Project")
					from attribute in project.Attributes("ToolsVersion")
					select attribute).FirstOrDefault<XAttribute>();
			}
		}

		private IEnumerable<XElement> UnconditionedPropertyGroups
		{
			get
			{
				return 
					from project in this.document.Elements(XmlMSBuildProjectStore.msbuild + "Project")
					from propertyGroup in project.Elements(XmlMSBuildProjectStore.msbuild + "PropertyGroup")
					where propertyGroup.Attribute("Condition") == null
					select propertyGroup;
			}
		}

		static XmlMSBuildProjectStore()
		{
			XmlMSBuildProjectStore.msbuild = "http://schemas.microsoft.com/developer/msbuild/2003";
		}

		private XmlMSBuildProjectStore(Microsoft.Expression.Framework.Documents.DocumentReference documentReference) : base(documentReference)
		{
		}

		public override bool ChangeImport(string oldImportValue, string newImportValue)
		{
			XAttribute[] array = (
				from import in this.ProjectImportAttributes
				where import.Value == oldImportValue
				select import).ToArray<XAttribute>();
			if ((int)array.Length == 0)
			{
				return false;
			}
			XAttribute[] xAttributeArray = array;
			for (int i = 0; i < (int)xAttributeArray.Length; i++)
			{
				XAttribute xAttribute = xAttributeArray[i];
				string path = base.DocumentReference.Path;
				string updateImportAction = StringTable.UpdateImportAction;
				object[] value = new object[] { xAttribute.Value, newImportValue };
				ProjectLog.LogSuccess(path, updateImportAction, value);
				xAttribute.Value = newImportValue;
			}
			return true;
		}

		public static XmlMSBuildProjectStore CreateInstance(Microsoft.Expression.Framework.Documents.DocumentReference documentReference, IServiceProvider serviceProvider)
		{
			if (documentReference == null)
			{
				throw new ArgumentNullException("documentReference");
			}
			if (serviceProvider == null)
			{
				throw new ArgumentNullException("serviceProvider");
			}
			if (!documentReference.IsValidPathFormat)
			{
				throw new ArgumentException("Document reference must be a valid path.", "documentReference");
			}
			if (!File.Exists(documentReference.Path))
			{
				throw new FileNotFoundException("File not found.", documentReference.Path);
			}
			XmlMSBuildProjectStore xmlMSBuildProjectStore = new XmlMSBuildProjectStore(documentReference)
			{
				document = XDocument.Load(documentReference.Path)
			};
			if (xmlMSBuildProjectStore.document == null)
			{
				return null;
			}
			return xmlMSBuildProjectStore;
		}

		private XElement GetOrCreateProjectElement()
		{
			XElement xElement = this.document.Element(XmlMSBuildProjectStore.msbuild + "Project");
			if (xElement != null)
			{
				return xElement;
			}
			xElement = new XElement(XmlMSBuildProjectStore.msbuild + "Project");
			this.document.Add(xElement);
			return xElement;
		}

		private XElement GetOrCreatePropertyGroup()
		{
			XElement xElement = this.UnconditionedPropertyGroups.FirstOrDefault<XElement>();
			if (xElement != null)
			{
				return xElement;
			}
			xElement = new XElement(XmlMSBuildProjectStore.msbuild + "PropertyGroup");
			this.GetOrCreateProjectElement().Add(xElement);
			return xElement;
		}

		private IEnumerable<XElement> GetProperties(string name)
		{
			return 
				from property in this.ProjectPropertyElements
				where property.Name.LocalName == name
				select property;
		}

		public override string GetProperty(string name)
		{
			return (
				from property in this.GetProperties(name)
				select property.Value).LastOrDefault<string>();
		}

		public override void Save()
		{
			this.document.Save(base.DocumentReference.Path);
		}

		protected override bool SetProperty(string name, string value, bool persisted)
		{
			bool flag;
			if (!persisted)
			{
				return false;
			}
			if (value == null)
			{
				return false;
			}
			using (IEnumerator<XElement> enumerator = this.GetProperties(name).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					XElement current = enumerator.Current;
					string path = base.DocumentReference.Path;
					CultureInfo currentCulture = CultureInfo.CurrentCulture;
					string updatePropertyAction = StringTable.UpdatePropertyAction;
					object[] objArray = new object[] { name, current.Value, value };
					ProjectLog.LogSuccess(path, string.Format(currentCulture, updatePropertyAction, objArray), new object[0]);
					current.Value = value;
					flag = true;
				}
				else
				{
					XElement orCreatePropertyGroup = this.GetOrCreatePropertyGroup();
					XElement xElement = new XElement(XmlMSBuildProjectStore.msbuild + name);
					string str = base.DocumentReference.Path;
					CultureInfo cultureInfo = CultureInfo.CurrentCulture;
					string setPropertyAction = StringTable.SetPropertyAction;
					object[] objArray1 = new object[] { name, xElement.Value, value };
					ProjectLog.LogSuccess(str, string.Format(cultureInfo, setPropertyAction, objArray1), new object[0]);
					xElement.Value = value;
					orCreatePropertyGroup.Add(xElement);
					return true;
				}
			}
			return flag;
		}
	}
}