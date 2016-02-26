using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Project;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	internal sealed class PasteCommand : ProjectCommand
	{
		private CutBuffer cutBuffer;

		public override string DisplayName
		{
			get
			{
				return StringTable.CommandPasteName;
			}
		}

		public override bool IsEnabled
		{
			get
			{
				if (!base.IsEnabled)
				{
					return false;
				}
				return this.cutBuffer.CanPaste;
			}
		}

		public PasteCommand(IServiceProvider serviceProvider, CutBuffer cutBuffer) : base(serviceProvider)
		{
			this.cutBuffer = cutBuffer;
		}

		public override void Execute()
		{
			this.HandleBasicExceptions(() => {
				if (this.SelectedProjectOrNull() == null)
				{
					return;
				}
				this.cutBuffer.Paste();
			});
		}
	}
}