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
    /// Interaction logic for _2.xaml
    /// </summary>
    public partial class _2 : UserControl
    {
        //public _2()
        //{
        //    InitializeComponent();
        //}
        public readonly static DependencyProperty BuildMessageProperty;

        public readonly static DependencyProperty IsBuildingProperty;

        public readonly static DependencyProperty IsOpenProperty;

        public readonly static DependencyProperty CloseDelayProperty;

        private DispatcherTimer timer;

        public string BuildMessage
        {
            get
            {
                return (string)base.GetValue(_2.BuildMessageProperty);
            }
            set
            {
                base.SetValue(_2.BuildMessageProperty, value);
            }
        }

        public int CloseDelay
        {
            get
            {
                return (int)base.GetValue(_2.CloseDelayProperty);
            }
            set
            {
                base.SetValue(_2.CloseDelayProperty, value);
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
                return (bool)base.GetValue(_2.IsBuildingProperty);
            }
            set
            {
                base.SetValue(_2.IsBuildingProperty, value);
            }
        }

        public bool IsOpen
        {
            get
            {
                return (bool)base.GetValue(_2.IsOpenProperty);
            }
            set
            {
                base.SetValue(_2.IsOpenProperty, value);
            }
        }

        static _2()
        {
            _2.BuildMessageProperty = DependencyProperty.Register("BuildMessage", typeof(string), typeof(_2));
            _2.IsBuildingProperty = DependencyProperty.Register("IsBuilding", typeof(bool), typeof(_2), new PropertyMetadata(new PropertyChangedCallback(_2.IsBuildingChanged)));
            _2.IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(_2));
            _2.CloseDelayProperty = DependencyProperty.Register("CloseDelay", typeof(int), typeof(_2), new PropertyMetadata((object)5));
        }

        public _2()
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
            _2 timeSpan = dependencyObject as _2;
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
