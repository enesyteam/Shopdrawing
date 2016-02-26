using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Expression.DesignModel.Core
{
	public class PlatformService : IPlatformService
	{
		private IDictionary<string, PlatformService.PlatformEntry> platforms = new Dictionary<string, PlatformService.PlatformEntry>();

		public IEnumerable<string> RegisteredPlatformCreators
		{
			get
			{
				return this.platforms.Keys;
			}
		}

		public PlatformService()
		{
		}

		public IPlatformCreator GetPlatformCreator(string identifier)
		{
			return this.platforms[identifier].PlatformCreator;
		}

		public object GetProperty(string platform, string propertyName)
		{
			return this.platforms[platform].GetProperty(propertyName);
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

		public void RegisterPlatformCreator(string identifier, IPlatformCreator platformCreator)
		{
			this.platforms[identifier] = new PlatformService.PlatformEntry(this, platformCreator);
		}

		public void RegisterPlatformCreator(string identifier, PlatformCreatorCallback callback)
		{
			this.platforms[identifier] = new PlatformService.PlatformEntry(this, callback);
		}

		public void SetProperty(string platform, string propertyName, object propertyValue)
		{
			this.platforms[platform].SetProperty(propertyName, propertyValue);
		}

		public void UnregisterPlatformCreator(string identifier)
		{
			PlatformService.PlatformEntry platformEntry = null;
			if (this.platforms.TryGetValue(identifier, out platformEntry))
			{
				platformEntry.Shutdown();
				this.platforms.Remove(identifier);
			}
		}

		public event EventHandler<PlatformEventArgs> PlatformCreated;

		public event EventHandler<PlatformEventArgs> PlatformDisposing;

		private sealed class PlatformEntry
		{
			private PlatformService platformService;

			private PlatformCreatorCallback callback;

			private IPlatformCreator platformCreator;

			private IDictionary<string, object> propertyTable;

			public IPlatformCreator PlatformCreator
			{
				get
				{
					if (this.platformCreator == null)
					{
						this.platformCreator = this.callback();
						this.platformCreator.PlatformCreated += new EventHandler<PlatformEventArgs>(this.PlatformCreator_PlatformCreated);
						this.platformCreator.PlatformDisposing += new EventHandler<PlatformEventArgs>(this.PlatformCreator_PlatformDisposing);
						foreach (KeyValuePair<string, object> keyValuePair in this.propertyTable)
						{
							this.platformCreator.SetProperty(keyValuePair.Key, keyValuePair.Value);
						}
						this.propertyTable.Clear();
					}
					return this.platformCreator;
				}
			}

			public PlatformEntry(PlatformService platformService, IPlatformCreator platformCreator)
			{
				this.platformService = platformService;
				this.platformCreator = platformCreator;
			}

			public PlatformEntry(PlatformService platformService, PlatformCreatorCallback callback)
			{
				this.platformService = platformService;
				this.callback = callback;
			}

			public object GetProperty(string propertyName)
			{
				if (this.platformCreator == null && this.propertyTable.ContainsKey(propertyName))
				{
					return this.propertyTable[propertyName];
				}
				return this.PlatformCreator.GetProperty(propertyName);
			}

			private void PlatformCreator_PlatformCreated(object sender, PlatformEventArgs e)
			{
				this.platformService.OnPlatformCreated(e);
			}

			private void PlatformCreator_PlatformDisposing(object sender, PlatformEventArgs e)
			{
				this.platformService.OnPlatformDisposing(e);
			}

			public void SetProperty(string propertyName, object propertyValue)
			{
				if (this.platformCreator == null)
				{
					this.propertyTable[propertyName] = propertyValue;
					return;
				}
				this.platformCreator.SetProperty(propertyName, propertyValue);
			}

			public void Shutdown()
			{
				if (this.platformCreator != null)
				{
					this.platformCreator.Shutdown();
					this.platformCreator.PlatformCreated -= new EventHandler<PlatformEventArgs>(this.PlatformCreator_PlatformCreated);
					this.platformCreator.PlatformDisposing -= new EventHandler<PlatformEventArgs>(this.PlatformCreator_PlatformDisposing);
					this.platformCreator = null;
				}
			}
		}
	}
}