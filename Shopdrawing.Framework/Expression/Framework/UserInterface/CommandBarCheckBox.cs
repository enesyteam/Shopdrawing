using Microsoft.Expression.Framework.Commands;
using Microsoft.Expression.Framework.Diagnostics;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Expression.Framework.UserInterface
{
    internal class CommandBarCheckBox : CommandBarButtonBase, ICommandBarCheckBox, ICommandBarItem
    {
        public string Text
        {
            get
            {
                return (string)base.Header;
            }
            set
            {
                base.Header = value;
            }
        }

        public CommandBarCheckBox(ICommandService commandService, string command)
            : base(commandService, command)
        {
            base.IsCheckable = true;
            base.Click += new RoutedEventHandler(this.Me_Click);
        }

        private void Me_Click(object sender, RoutedEventArgs e)
        {
            PerformanceUtility.MeasurePerformanceUntilRender(PerformanceEvent.CommandBarCheckBoxClicked);
            bool flag = true;
            object commandProperty = this.commandService.GetCommandProperty(this.command, "IsChecked");
            if (commandProperty != null)
            {
                flag = !(bool)commandProperty;
            }
            this.commandService.SetCommandProperty(this.command, "IsChecked", flag);
        }

        public override void Update()
        {
            base.Update();
            base.UpdateChecked();
        }
    }
}