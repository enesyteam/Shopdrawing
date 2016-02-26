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
    public class SubstructuresNode : ShopdrawingTreeNode
    {
        public SubstructuresNode(BridgeProject.Items.Substructures s)
        {
            this.DataObject = s;
        }
        public override object Icon
        {
            get
            {
                return Images.Library;
            }
        }
    }
}