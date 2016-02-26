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

namespace Microsoft.Expression.DesignSurface.View
{
    public sealed partial class NotificationBar : UserControl
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
                return (string)base.GetValue(NotificationBar.BuildMessageProperty);
            }
            set
            {
                base.SetValue(NotificationBar.BuildMessageProperty, value);
            }
        }

        public int CloseDelay
        {
            get
            {
                return (int)base.GetValue(NotificationBar.CloseDelayProperty);
            }
            set
            {
                base.SetValue(NotificationBar.CloseDelayProperty, value);
            }
        }

        public ICommand DismissCommand
        {
            get
            {
                return new DelegateCommand(new DelegateCommand.SimpleEventHandler(this.Dismiss));
            }
        }

        public bool IsBuilding
        {
            get
            {
                return (bool)base.GetValue(NotificationBar.IsBuildingProperty);
            }
            set
            {
                base.SetValue(NotificationBar.IsBuildingProperty, value);
            }
        }

        public bool IsOpen
        {
            get
            {
                return (bool)base.GetValue(NotificationBar.IsOpenProperty);
            }
            set
            {
                base.SetValue(NotificationBar.IsOpenProperty, value);
            }
        }

        static NotificationBar()
        {
            NotificationBar.BuildMessageProperty = DependencyProperty.Register("BuildMessage", typeof(string), typeof(NotificationBar));
            NotificationBar.IsBuildingProperty = DependencyProperty.Register("IsBuilding", typeof(bool), typeof(NotificationBar), new PropertyMetadata(new PropertyChangedCallback(NotificationBar.IsBuildingChanged)));
            NotificationBar.IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(NotificationBar));
            NotificationBar.CloseDelayProperty = DependencyProperty.Register("CloseDelay", typeof(int), typeof(NotificationBar), new PropertyMetadata((object)5));
        }

        public NotificationBar()
        {
            this.InitializeComponent();
            this.timer = new DispatcherTimer();
            this.timer.Tick += new EventHandler(this.Timer_Tick);
        }

        private void Dismiss()
        {
            if (this.IsOpen)
            {
                this.timer.Stop();
                this.IsOpen = false;
            }
        }

        private static void IsBuildingChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            NotificationBar timeSpan = dependencyObject as NotificationBar;
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


        private void Timer_Tick(object sender, EventArgs e)
        {
            this.timer.Stop();
            this.IsOpen = false;
        }
    }
}