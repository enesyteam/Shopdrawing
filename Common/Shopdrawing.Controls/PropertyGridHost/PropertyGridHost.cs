using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Shopdrawing.Controls
{
    public class PropertyGridHost : Grid
    {
        /// <summary>
        /// Tên hiển thị
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(PropertyGridHost), new PropertyMetadata("Untitled"));
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
    }
}
