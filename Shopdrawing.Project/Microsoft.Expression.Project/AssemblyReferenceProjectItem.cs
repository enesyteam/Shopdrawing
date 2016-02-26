using Microsoft.Expression.Framework.Documents;
using Microsoft.Expression.Framework.UserInterface;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Expression.Project
{
	internal abstract class AssemblyReferenceProjectItem : ProjectItemBase
	{
		private static ICommandBar contextMenu;

		internal string AssemblyLocation
		{
			get;
			set;
		}

		public override bool CanAddChildren
		{
			get
			{
				return false;
			}
		}

		public override IDocument Document
		{
			get
			{
				return null;
			}
		}

		public override bool FileExists
		{
			get
			{
				return PathHelper.FileExists(base.DocumentReference.Path);
			}
		}

		public override bool IsDirty
		{
			get
			{
				return false;
			}
		}

		public override bool IsLinkedFile
		{
			get
			{
				return true;
			}
		}

		public override bool IsOpen
		{
			get
			{
				return false;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public override bool IsReference
		{
			get
			{
				return true;
			}
		}

		static AssemblyReferenceProjectItem()
		{
		}

		internal AssemblyReferenceProjectItem(IProject project, Microsoft.Expression.Framework.Documents.DocumentReference documentReference, IDocumentType documentType, IServiceProvider serviceProvider) : base(project, documentReference, documentType, serviceProvider)
		{
		}

		public override ICommandBar GetContextMenu(ICommandBarCollection commandBarCollection)
		{
			if (AssemblyReferenceProjectItem.contextMenu == null)
			{
				AssemblyReferenceProjectItem.contextMenu = commandBarCollection.AddContextMenu("Project_FolderStandinContextMenu");
				AssemblyReferenceProjectItem.contextMenu.Items.AddButton("Project_RemoveProjectItem", StringTable.ProjectItemContextMenuRemove);
			}
			return AssemblyReferenceProjectItem.contextMenu;
		}
	}
}