using Microsoft.Expression.Framework.Data;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;

namespace Shopdrawing.Framework.Shopdrawing.Controls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Shopdrawing.Framework.Shopdrawing.Controls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Shopdrawing.Framework.Shopdrawing.Controls;assembly=Shopdrawing.Framework.Shopdrawing.Controls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:XXX/>
    ///
    /// </summary>
    public class XXX : UserControl
    {
        public readonly static DependencyProperty BuildMessageProperty;

        public readonly static DependencyProperty IsBuildingProperty;

        public readonly static DependencyProperty IsOpenProperty;

        public readonly static DependencyProperty CloseDelayProperty;

        private DispatcherTimer timer;

        public string BuildMessage
        {
            get
            {
                return (string)base.GetValue(XXX.BuildMessageProperty);
            }
            set
            {
                base.SetValue(XXX.BuildMessageProperty, value);
            }
        }

        public int CloseDelay
        {
            get
            {
                return (int)base.GetValue(XXX.CloseDelayProperty);
            }
            set
            {
                base.SetValue(XXX.CloseDelayProperty, value);
            }
        }

        public ICommand DismissCommand
        {
            get
            {
                return new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.Dismiss));
            }
        }
        private void Dismiss()
        {
            if (this.IsOpen)
            {
                this.timer.Stop();
                this.IsOpen = false;
            }
            this.BuildMessage = "";
        }
        public bool IsBuilding
        {
            get
            {
                return (bool)base.GetValue(XXX.IsBuildingProperty);
            }
            set
            {
                base.SetValue(XXX.IsBuildingProperty, value);
            }
        }

        public bool IsOpen
        {
            get
            {
                return (bool)base.GetValue(XXX.IsOpenProperty);
            }
            set
            {
                base.SetValue(XXX.IsOpenProperty, value);
            }
        }
        static XXX()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XXX), new FrameworkPropertyMetadata(typeof(XXX)));

            XXX.BuildMessageProperty = DependencyProperty.Register("BuildMessage", typeof(string), typeof(XXX));
            XXX.IsBuildingProperty = DependencyProperty.Register("IsBuilding", typeof(bool), typeof(XXX), new PropertyMetadata(new PropertyChangedCallback(XXX.IsBuildingChanged)));
            XXX.IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(XXX));
            XXX.CloseDelayProperty = DependencyProperty.Register("CloseDelay", typeof(int), typeof(XXX), new PropertyMetadata(5));
        }
        private static void IsBuildingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            XXX timeSpan = dependencyObject as XXX;
            if (timeSpan != null && args.Property.Name == "IsBuilding")
            {
                bool oldValue = (bool)args.OldValue;
                bool newValue = (bool)args.NewValue;
                if (newValue != oldValue)
                {
                    if (newValue)
                    {
                        timeSpan.timer.Stop();
                        timeSpan.IsOpen = newValue;
                        return;
                    }
                    timeSpan.timer.Interval = new TimeSpan(0, 0, timeSpan.CloseDelay);
                    timeSpan.timer.Start();
                }
            }
        }
        public XXX()
        {
            this.timer = new DispatcherTimer();
            this.timer.Tick += new EventHandler(this.Timer_Tick);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            this.timer.Stop();
            this.IsOpen = false;
        }
    }
}
