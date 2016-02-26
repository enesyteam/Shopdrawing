using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Project;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project.Commands
{
	internal sealed class CutCopyCommand : ProjectCommand
	{
		private bool isCut;

		private CutBuffer cutBuffer;

		public override string DisplayName
		{
			get
			{
				if (this.isCut)
				{
					return StringTable.CommandCutName;
				}
				return StringTable.CommandCopyName;
			}
		}

		public override bool IsEnabled
		{
			get
			{
				if (base.IsEnabled && this.isCut)
				{
					return this.cutBuffer.CanCut;
				}
				return this.cutBuffer.CanCopy;
			}
		}

		public CutCopyCommand(IServiceProvider serviceProvider, bool isCut, CutBuffer cutBuffer) : base(serviceProvider)
		{
			this.isCut = isCut;
			this.cutBuffer = cutBuffer;
		}

		public override void Execute()
		{
			this.HandleBasicExceptions(() => {
				if (this.isCut)
				{
					this.cutBuffer.Cut();
					return;
				}
				this.cutBuffer.Copy();
			});
		}
	}
}