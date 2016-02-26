using Microsoft.Expression.Extensibility;
using Microsoft.Expression.Framework;
using System;

namespace Microsoft.Expression.Project.Licensing
{
	[CLSCompliant(false)]
	public class ItemizedLicensingDialogCreator : CommonDialogCreator
	{
		private ItemizedLicensingDialog dialog;

		private ItemizedLicensingComponent[] skus;

		[CLSCompliant(false)]
		public override ILicensingDialogQuery GetInstance
		{
			get
			{
				if (this.dialog == null)
				{
					this.dialog = new ItemizedLicensingDialog(base.Services, this.skus);
					base.MergeLicensingResources(this.dialog);
					this.dialog.InitializeDialog();
				}
				return this.dialog;
			}
		}

		public ItemizedLicensingDialogCreator(IServices services) : base(services)
		{
			ItemizedLicensingComponent[] itemizedLicensingComponent = new ItemizedLicensingComponent[] { new ItemizedLicensingComponent(ProjectLicenseGroup.WpfSilverlight, base.Services), new ItemizedLicensingComponent(ProjectLicenseGroup.SketchFlow, base.Services) };
			this.skus = itemizedLicensingComponent;
		}
	}
}