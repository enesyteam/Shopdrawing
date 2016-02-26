using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Expression.Project
{
	public class AssemblyLoggingService : IAssemblyLoggingService
	{
		private const string filename = "AssemblyLog.xml";

		private bool isEnabled;

		private bool hasWritten;

		private string directory;

		private Queue<AssemblyLoggingEvent> events = new Queue<AssemblyLoggingEvent>();

		private string FileLocation
		{
			get
			{
				return Path.Combine(this.directory, "AssemblyLog.xml");
			}
		}

		public bool IsEnabled
		{
			get
			{
				return this.isEnabled;
			}
			set
			{
				if (this.isEnabled != value)
				{
					if (value)
					{
						if (!this.hasWritten)
						{
							this.hasWritten = true;
							this.Clear();
						}
						this.Log(new AppDomainListEvent());
						while (this.events.Count > 0)
						{
							this.Write(this.events.Dequeue());
						}
					}
					this.isEnabled = value;
				}
			}
		}

		public AssemblyLoggingService(string directory)
		{
			this.directory = directory;
			AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(this.OnCurrentDomainAssemblyLoad);
		}

		private void Clear()
		{
			try
			{
				File.Delete(this.FileLocation);
			}
			catch (ArgumentException argumentException)
			{
			}
			catch (UnauthorizedAccessException unauthorizedAccessException)
			{
			}
			catch (IOException oException)
			{
			}
			catch (NotSupportedException notSupportedException)
			{
			}
		}

		public void Log(AssemblyLoggingEvent assemblyLoadingEvent)
		{
			if (!this.isEnabled)
			{
				this.events.Enqueue(assemblyLoadingEvent);
				return;
			}
			assemblyLoadingEvent.StackTrace = (new StackTrace()).ToString();
			this.Write(assemblyLoadingEvent);
		}

		private void OnCurrentDomainAssemblyLoad(object sender, AssemblyLoadEventArgs args)
		{
			this.Log(new AssemblyLoadedEvent(args.LoadedAssembly));
		}

		private void RemoveRootClosingTag()
		{
			using (FileStream fileStream = File.Open(this.FileLocation, FileMode.OpenOrCreate, FileAccess.ReadWrite))
			{
				using (StreamReader streamReader = new StreamReader(fileStream))
				{
					long num = fileStream.Seek(Math.Max((long)0, fileStream.Length - (long)128), SeekOrigin.Begin);
					string end = streamReader.ReadToEnd();
					int length = end.Length - 1;
					while (length >= 0)
					{
						if (end[length] != '<' || !end.Substring(length).StartsWith("</Events>", StringComparison.Ordinal))
						{
							length--;
						}
						else
						{
							fileStream.SetLength(num + (long)length);
							return;
						}
					}
				}
			}
		}

		public void Unload()
		{
			AppDomain.CurrentDomain.AssemblyLoad -= new AssemblyLoadEventHandler(this.OnCurrentDomainAssemblyLoad);
		}

		private void Write(AssemblyLoggingEvent assemblyLoadingEvent)
		{
			using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
			{
				using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
				{
					xmlTextWriter.Formatting = Formatting.Indented;
					assemblyLoadingEvent.XmlElement.WriteTo(xmlTextWriter);
					stringWriter.WriteLine();
				}
				this.Write(stringWriter.ToString());
			}
		}

		private void Write(string text)
		{
			try
			{
				this.RemoveRootClosingTag();
				using (FileStream fileStream = File.Open(this.FileLocation, FileMode.Append, FileAccess.Write))
				{
					using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
					{
						if (fileStream.Length == (long)0)
						{
							streamWriter.Write("<Events>\r\n");
						}
						streamWriter.Write(text);
						streamWriter.Write("</Events>\r\n");
					}
				}
			}
			catch (ArgumentException argumentException)
			{
			}
			catch (UnauthorizedAccessException unauthorizedAccessException)
			{
			}
			catch (IOException oException)
			{
			}
			catch (NotSupportedException notSupportedException)
			{
			}
		}
	}
}