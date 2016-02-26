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
    public class PierNode : ShopdrawingTreeNode
    {
        internal const string DataFormat = "PierNode";
        public PierNode(BridgeProject.Items.Pier p)
        {
            DataObject = p;
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
                tooltip.Inlines.Add(new Run(((BridgeProject.Items.Pier)DataObject).ToString()));
                tooltip.Inlines.Add(new LineBreak());
                tooltip.Inlines.Add(new Bold(new Run("Substructures: ")));
                tooltip.Inlines.Add(new Run(((BridgeProject.Items.Pier)DataObject).Center.ToString()));
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
    }
}