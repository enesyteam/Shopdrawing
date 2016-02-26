using Microsoft.Expression.DesignModel.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading;

namespace Microsoft.Expression.DesignModel.Core
{
	public abstract class PlatformCreatorBase : IPlatformCreator
	{
		private IDictionary<IPlatform, int> platformTable = new Dictionary<IPlatform, int>();

		private IPlatformReferenceContext defaultPlatformReferenceContext;

		protected abstract FrameworkName RuntimeFramework
		{
			get;
		}

		protected PlatformCreatorBase()
		{
		}

		public IPlatform CreatePlatform(IPlatformRuntimeContext platformRuntimeContext, IPlatformReferenceContext platformReferenceContext, IPlatformService platformService, IPlatformHost platformHost)
		{
			IPlatform platform;
			if (platformReferenceContext == null)
			{
				if (this.defaultPlatformReferenceContext == null)
				{
					this.defaultPlatformReferenceContext = new DefaultPlatformReferenceContext(this.RuntimeFramework);
				}
				platformReferenceContext = this.defaultPlatformReferenceContext;
			}
			using (IEnumerator<IPlatform> enumerator = this.platformTable.Keys.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IPlatform current = enumerator.Current;
					if (!platformReferenceContext.Equals(current.Metadata.ReferenceContext))
					{
						continue;
					}
					this.platformTable[current] = this.platformTable[current] + 1;
					platform = current;
					return platform;
				}
				PlatformBase platformBase = this.CreatePlatformInternal(platformRuntimeContext, platformReferenceContext, platformService, platformHost);
				platformBase.Initialize();
				platformBase.PlatformHost = platformHost;
				RuntimeGeneratedTypesHelper.RegisterRuntimeAssemblies(platformBase.Metadata);
				this.OnPlatformCreated(new PlatformEventArgs(platformBase));
				this.platformTable.Add(platformBase, 1);
				return platformBase;
			}
			return platform;
		}

		protected abstract PlatformBase CreatePlatformInternal(IPlatformRuntimeContext platformRuntimeContext, IPlatformReferenceContext platformReferenceContext, IPlatformService platformService, IPlatformHost platformHost);

		public abstract object GetProperty(string propertyName);

		public virtual void Initialize()
		{
		}

		private void OnPlatformCreated(PlatformEventArgs e)
		{
			if (this.PlatformCreated != null)
			{
				this.PlatformCreated(this, e);
			}
		}

		private void OnPlatformDisposing(PlatformEventArgs e)
		{
			if (this.PlatformDisposing != null)
			{
				this.PlatformDisposing(this, e);
			}
		}

		public abstract void RegisterAssembly(Assembly assembly);

		public void ReleasePlatform(IPlatform platform)
		{
			this.platformTable[platform] = this.platformTable[platform] - 1;
			if (this.platformTable[platform] == 0 && !platform.Metadata.ReferenceContext.KeepAlive)
			{
				this.OnPlatformDisposing(new PlatformEventArgs(platform));
				platform.Dispose();
				this.platformTable.Remove(platform);
			}
		}

		public abstract void SetProperty(string propertyName, object propertyValue);

		public virtual void Shutdown()
		{
		}

		public override string ToString()
		{
			string str;
			using (StringWriter stringWriter = new StringWriter(CultureInfo.CurrentCulture))
			{
				stringWriter.WriteLine(this.GetType().FullName);
				foreach (KeyValuePair<IPlatform, int> keyValuePair in this.platformTable)
				{
					stringWriter.WriteLine(string.Concat("\tPlatformContext: ", keyValuePair.Key.Metadata.ReferenceContext));
					stringWriter.WriteLine(string.Concat("\t\tTargetFramework: ", keyValuePair.Key.Metadata.TargetFramework));
					stringWriter.WriteLine(string.Concat("\t\tCount: ", keyValuePair.Value));
				}
				str = stringWriter.ToString();
			}
			return str;
		}

		public event EventHandler<PlatformEventArgs> PlatformCreated;

		public event EventHandler<PlatformEventArgs> PlatformDisposing;
	}
}