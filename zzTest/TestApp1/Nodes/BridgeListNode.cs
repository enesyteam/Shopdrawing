using BridgeProject.Items;
using ICSharpCode.ILSpy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Shopdrawing.TreeNode
{
    public class BridgeListNode : ShopdrawingTreeNode
    {
        public BridgesList BrigesList { get; set; }

        public BridgeListNode(BridgesList l)
        {
            BrigesList = l;
            DataObject = l;
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
        public override bool CanDrop(DragEventArgs e, int index)
        {
            e.Effects = DragDropEffects.Move;
            if (e.Data.GetDataPresent(BridgeNode.DataFormat))
                return true;
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
                return true;
            else
            {
                e.Effects = DragDropEffects.None;
                return false;
            }
        }
        public override void Drop(DragEventArgs e, int index)
        {
            string[] files = e.Data.GetData(BridgeNode.DataFormat) as string[];
            if (files == null)
            {
                MessageBox.Show("Null");
                files = e.Data.GetData(DataFormats.FileDrop) as string[];
            }
            if (files != null)
            {
                
                //lock (assemblyList.assemblies)
                //{
                //    var assemblies = (from file in files
                //                      where file != null
                //                      select assemblyList.OpenAssembly(file) into node
                //                      where node != null
                //                      select node).Distinct().ToList();
                //    foreach (LoadedAssembly asm in assemblies)
                //    {
                //        int nodeIndex = assemblyList.assemblies.IndexOf(asm);
                //        if (nodeIndex < index)
                //            index--;
                //        assemblyList.assemblies.RemoveAt(nodeIndex);
                //    }
                //    assemblies.Reverse();
                //    foreach (LoadedAssembly asm in assemblies)
                //    {
                //        assemblyList.assemblies.Insert(index, asm);
                //    }
                //}
            }
        }
        public override object Icon
        {
            get
            {
                return Images.Namespace;
            }
        }
    }
}
