using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Shopdrawing.Controls
{
    /// <summary>
    /// Defines a toolbar button
    /// </summary>
    public class SdrToolbarToggleButton : ToggleButton
    {
       
        /// <summary>Image dependency property</summary>
        public static readonly DependencyProperty TextProperty;

        public static readonly DependencyProperty CheckTooltipProperty;
        public static readonly DependencyProperty UnCheckTooltipProperty;



        public static readonly DependencyProperty IconProperty;
        public static readonly DependencyProperty UncheckedIconProperty;


        /// <summary>
        /// Class ctor
        /// </summary>
        static SdrToolbarToggleButton()
        {
            IconProperty = DependencyProperty.Register("Icon",
                                       typeof(string),
                                       typeof(SdrToolbarToggleButton),
                                       new FrameworkPropertyMetadata(string.Empty,
                                                                     FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure
                                                                     ));

            UncheckedIconProperty = DependencyProperty.Register("UncheckedIcon",
                           typeof(string),
                           typeof(SdrToolbarToggleButton),
                           new FrameworkPropertyMetadata(string.Empty,
                                                         FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure
                                                         ));

            
            TextProperty = DependencyProperty.Register("Text",
                                                                        typeof(String),
                                                                        typeof(SdrToolbarToggleButton),
                                                                        new FrameworkPropertyMetadata("",
                                                                                                      FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits
                                                                                                      ));
            CheckTooltipProperty = DependencyProperty.Register("CheckTooltip",
                                                            typeof(String),
                                                            typeof(SdrToolbarToggleButton),
                                                            new FrameworkPropertyMetadata(""));

                                                                                                     
            UnCheckTooltipProperty = DependencyProperty.Register("UnCheckTooltip",
                                                            typeof(String),
                                                            typeof(SdrToolbarToggleButton),
                                                            new FrameworkPropertyMetadata(""));


           
            //IsEnabledProperty.OverrideMetadata(typeof(SdrToolbarToggleButton), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(IsEnabledChanged)));
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public SdrToolbarToggleButton()
            : base()
        {
            //Style = new Style(typeof(SdrToolbarToggleButton), (Style)FindResource(ToolBar.ToggleButtonStyleKey));
            if ((bool)this.IsChecked)
            {
                if (!string.IsNullOrEmpty(UnCheckTooltip))
                    this.ToolTip = UnCheckTooltip;
            }
            else
            {
                if (!string.IsNullOrEmpty(CheckTooltip))
                    this.ToolTip = CheckTooltip;
            }
        }

        
        public string Icon
        {
            get
            {
                return ((string)GetValue(IconProperty));
            }
            set
            {
                SetValue(IconProperty, value);
            }
        }
        public string UncheckedIcon
        {
            get
            {
                return ((string)GetValue(UncheckedIconProperty));
            }
            set
            {
                SetValue(UncheckedIconProperty, value);
            }
        }

        /// <summary>
        /// Text displayed in button
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        [Bindable(true)]
        [Category("Layout")]
        [Description("Text displayed in button")]
        public String Text
        {
            get
            {
                return ((String)GetValue(TextProperty));
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        public string CheckTooltip
        {
            get
            {
                return ((String)GetValue(CheckTooltipProperty));
            }
            set
            {
                SetValue(CheckTooltipProperty, value);
            }
        }
        public string UnCheckTooltip
        {
            get
            {
                return ((String)GetValue(UnCheckTooltipProperty));
            }
            set
            {
                SetValue(UnCheckTooltipProperty, value);
            }
        }


       
        protected override void OnChecked(RoutedEventArgs e)
        {
            base.OnChecked(e);
            if (!string.IsNullOrEmpty(UnCheckTooltip))
            this.ToolTip = UnCheckTooltip;
        }
        protected override void OnUnchecked(RoutedEventArgs e)
        {
            base.OnUnchecked(e);
            if (!string.IsNullOrEmpty(CheckTooltip))
            this.ToolTip = CheckTooltip;
        }
    }
}
