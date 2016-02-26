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
    public class AbutmentNode : ShopdrawingTreeNode
    {
        internal const string DataFormat = "AbutmentsNode";
        public AbutmentNode(BridgeProject.Items.Abutment a)
        {
            DataObject = a;
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
                tooltip.Inlines.Add(new Run(((BridgeProject.Items.Abutment)DataObject).ToString()));
                tooltip.Inlines.Add(new LineBreak());
                tooltip.Inlines.Add(new Bold(new Run("Substructures: ")));
                tooltip.Inlines.Add(new Run(((BridgeProject.Items.Abutment)DataObject).Center.ToString()));
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
        public override object Icon
        {
            get
            {
                return Images.Interface;
            }
        }

        [ExportContextMenuEntryAttribute(Header = "_Draw", Icon = "images/Delete.png")] //, Icon = "images/Delete.png"
        sealed class RemoveAssembly : IContextMenuEntry
        {
            public bool IsVisible(TextViewContext context)
            {
                if (context.SelectedTreeNodes == null)
                    return false;
                return context.SelectedTreeNodes.All(n => n is AbutmentNode);
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
                    //node.Delete();
                    MessageBox.Show("Create drawing?");
                }
            }
        }
        //[ExportContextMenuEntryAttribute(Header = "_Get Quantities", Icon = "images/Delete.png")] //, Icon = "images/Delete.png"
        //sealed class GetQuantities : IContextMenuEntry
        //{
        //    public bool IsVisible(TextViewContext context)
        //    {
        //        if (context.SelectedTreeNodes == null)
        //            return false;
        //        return context.SelectedTreeNodes.All(n => n is AbutmentNode);
        //    }

        //    public bool IsEnabled(TextViewContext context)
        //    {
        //        return true;
        //    }

        //    public void Execute(TextViewContext context)
        //    {
        //        if (context.SelectedTreeNodes == null)
        //            return;
        //        foreach (var node in context.SelectedTreeNodes)
        //        {
        //            //node.Delete();
        //            MessageBox.Show("Get Quantities?");
        //        }
        //    }
        //}
    }
}