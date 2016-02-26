using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Shopdrawing.Controls
{
    /// <summary>
    /// Defines a toolbar button
    /// </summary>
    public class ToolBarButton : Button
    {

        /// <summary>Image dependency property</summary>
        public static readonly DependencyProperty TextProperty;

        public static readonly DependencyProperty IconProperty;


        /// <summary>Display Style applied to the Toolbarbutton</summary>
        public enum DisplayStyleE
        {
            /// <summary>Image only displayed</summary>
            Image,
            /// <summary>Text only displayed</summary>
            Text,
            /// <summary>Image and Text displayed</summary>
            ImageAndText
        }

        /// <summary>
        /// Class ctor
        /// </summary>
        static ToolBarButton()
        {
            IconProperty = DependencyProperty.Register("Icon",
                                                   typeof(string),
                                                   typeof(ToolBarButton),
                                                   new FrameworkPropertyMetadata(string.Empty,
                                                                                 FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure
                                                                                 ));

            
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public ToolBarButton()
            : base()
        {
            Style = new Style(typeof(ToolBarButton), (Style)FindResource(ToolBar.ButtonStyleKey));
            
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
    
    }
}
