using BridgeProject;
using BridgeProject.Items;
using ICSharpCode.ILSpy;
using ICSharpCode.TreeView;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Shopdrawing.TreeNode
{
    public class BridgeNode : ShopdrawingTreeNode
    {
        //public override object DataObject { get; set; }
        internal const string DataFormat = "BridgeNode";

        readonly BridgeProject.Items.Bridge bridge;
        public BridgeNode(BridgeProject.Items.Bridge b)
        {
            this.bridge = b;
            DataObject = b;
        }
        public override IDataObject Copy(SharpTreeNode[] nodes)
        {
            DataObject dataObject = new DataObject();
            dataObject.SetData(DataFormat, nodes.OfType<BridgeNode>().Select(n => n.DataObject).ToArray());
            return dataObject;
        }
        //public override FilterResult Filter(FilterSettings settings)
        //{
        //    //if (settings.SearchTermMatches(BridgeProject.Items.Bridge.Name))
        //    //    return FilterResult.Match;
        //    //else
        //    //    return FilterResult.Recurse;
        //}
        public override object Icon
        {
            get
            {
                return Images.Bridge;
            }
        }
        string _text = "Bridge No.";
        public override string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                RaisePropertyChanged("Text");
            }
        }
        TextBlock tooltip;

        public override object ToolTip
        {
            get
            {
                if (DataObject == null)
                    return "Assembly could not be loaded. Click here for details.";

                //if (tooltip == null)
                //{
                    tooltip = new TextBlock();
                    tooltip.Inlines.Add(new Bold(new Run("Name: ")));
                    tooltip.Inlines.Add(new Run(DataObject.ToString()));
                    tooltip.Inlines.Add(new LineBreak());
                    tooltip.Inlines.Add(new Bold(new Run("Location: ")));
                    tooltip.Inlines.Add(new Run(((BridgeProject.Items.Bridge)DataObject).MileStone.ToString()));
                    tooltip.Inlines.Add(new LineBreak());
                    tooltip.Inlines.Add(new Bold(new Run("Substructures: ")));
                    tooltip.Inlines.Add(new Run(((BridgeProject.Items.Bridge)DataObject).Substructures.ToString()));
                    //tooltip.Inlines.Add(new Run(CSharpLanguage.GetPlatformDisplayName(assembly.AssemblyDefinition.MainModule)));
                    //string runtimeName = CSharpLanguage.GetRuntimeDisplayName(assembly.AssemblyDefinition.MainModule);
                    //if (runtimeName != null)
                    //{
                    //    tooltip.Inlines.Add(new LineBreak());
                    //    tooltip.Inlines.Add(new Bold(new Run("Runtime: ")));
                    //    tooltip.Inlines.Add(new Run(runtimeName));
                    //}
                //}

                return tooltip;
            }
        }
        public override bool CanDrag(SharpTreeNode[] nodes)
        {
            return nodes.All(n => n is BridgeNode);
        }

        public override void StartDrag(DependencyObject dragSource, SharpTreeNode[] nodes)
        {
            DragDrop.DoDragDrop(dragSource, Copy(nodes), DragDropEffects.All);
        }

        public override bool CanDelete()
        {
            return true;
        }

        public override void Delete()
        {
            DeleteCore();
        }

        public override void DeleteCore()
        {
            MessageBox.Show("Do you want to delete?");

        }
    }

    [ExportContextMenuEntryAttribute(Header = "_Remove")] //, Icon = "images/Delete.png"
    sealed class RemoveAssembly : IContextMenuEntry
    {
        public bool IsVisible(TextViewContext context)
        {
            if (context.SelectedTreeNodes == null)
                return false;
            return context.SelectedTreeNodes.All(n => n is BridgeNode);
        }

        public bool IsEnabled(TextViewContext context)
        {
            return true;
        }

        public void Execute(TextViewContext context)
        {
            if (context.SelectedTreeNodes == null)
                return;
            foreach (var node in context.SelectedTreeNodes)
            {
                node.Delete();
            }
        }
    }
}
