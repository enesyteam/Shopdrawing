using Microsoft.Expression.DesignModel.DocumentModel;
using Microsoft.Expression.DesignModel.Markup;
using Microsoft.Expression.DesignModel.Metadata;
using Microsoft.Expression.DesignModel.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.Expression.DesignModel.Core
{
	public abstract class ThemeManager
	{
		public virtual bool AllowFallbackToPlatform
		{
			get
			{
				return false;
			}
		}

		public abstract string CurrentTheme
		{
			get;
		}

		protected IPlatform Platform
		{
			get;
			private set;
		}

		public virtual string ThemeResourcesFile
		{
			get
			{
				return null;
			}
		}

		protected ThemeManager(IPlatform platform)
		{
			this.Platform = platform;
		}

		public XamlDocument GetTheme(IDocumentLocator documentLocator, bool isFromAssemblyTheme, IDocumentContext userContext, ITextBuffer textBuffer, Encoding encoding)
		{
			XamlDocument xamlDocument = null;
			if (textBuffer != null && (isFromAssemblyTheme || File.Exists(documentLocator.Path)))
			{
				IDocumentContext documentContext = (userContext != null ? userContext : this.ProvideDocumentContext(documentLocator));
				XamlParserResults.Parse(documentContext, PlatformTypes.ResourceDictionary, textBuffer);
				xamlDocument = new XamlDocument(documentContext, PlatformTypes.Object, textBuffer, encoding, new DefaultXamlSerializerFilter());
			}
			return xamlDocument;
		}

		public static ITextBuffer LoadResource(IAssembly assembly, string path, ITextBufferService textBufferService, out Encoding encoding)
		{
			ITextBuffer textBuffer;
			encoding = null;
			string manifestModule = assembly.ManifestModule;
			if (manifestModule.EndsWith(".DLL", StringComparison.OrdinalIgnoreCase))
			{
				manifestModule = manifestModule.Substring(0, manifestModule.Length - 4);
			}
			manifestModule = string.Concat(manifestModule, ".g");
			try
			{
				bool flag = false;
				Assembly reflectionAssembly = AssemblyHelper.GetReflectionAssembly(assembly);
				if (reflectionAssembly != null)
				{
					foreach (CustomAttributeData customAttributesDatum in reflectionAssembly.GetCustomAttributesData())
					{
						if (!(customAttributesDatum.Constructor.DeclaringType.FullName == "System.Resources.NeutralResourcesLanguageAttribute") || !(customAttributesDatum.Constructor.DeclaringType.Assembly.GetName().Name == "mscorlib") || customAttributesDatum.ConstructorArguments.Count != 2 || !(customAttributesDatum.ConstructorArguments[1].ArgumentType.FullName == "System.Resources.UltimateResourceFallbackLocation") || customAttributesDatum.ConstructorArguments[1].Value.Equals(0))
						{
							continue;
						}
						flag = true;
						break;
					}
				}
				if (flag)
				{
					ResourceManager resourceManager = new ResourceManager(manifestModule, reflectionAssembly);
					using (Stream stream = resourceManager.GetStream(path, CultureInfo.InvariantCulture))
					{
						if (stream != null)
						{
							textBuffer = ThemeManager.LoadResourceFromStream(textBufferService, stream, out encoding);
							return textBuffer;
						}
					}
				}
				using (Stream manifestResourceStream = assembly.GetManifestResourceStream(string.Concat(manifestModule, ".resources")))
				{
					if (manifestResourceStream != null)
					{
						using (ResourceReader resourceReaders = new ResourceReader(manifestResourceStream))
						{
							foreach (DictionaryEntry resourceReader in resourceReaders)
							{
								if (string.Compare((string)resourceReader.Key, path, StringComparison.Ordinal) != 0)
								{
									continue;
								}
								textBuffer = ThemeManager.LoadResourceFromStream(textBufferService, (Stream)resourceReader.Value, out encoding);
								return textBuffer;
							}
						}
					}
				}
				return null;
			}
			catch
			{
				return null;
			}
			return textBuffer;
		}

		private static ITextBuffer LoadResourceFromStream(ITextBufferService textBufferService, Stream stream, out Encoding encoding)
		{
			ITextBuffer textBuffer;
			using (StreamReader streamReader = new StreamReader(stream))
			{
				string end = streamReader.ReadToEnd();
				encoding = streamReader.CurrentEncoding;
				ITextBuffer textBuffer1 = textBufferService.CreateTextBuffer();
				textBuffer1.SetText(0, textBuffer1.Length, end);
				textBuffer = textBuffer1;
			}
			return textBuffer;
		}

		protected virtual IDocumentContext ProvideDocumentContext(IDocumentLocator documentLocator)
		{
			return this.Platform.PlatformHost.CreateDefaultContext(documentLocator);
		}
	}
}