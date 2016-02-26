using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Shopdrawing.Controls
{
    public class TextBoxEx : TextBox
    {
        public TextBoxEx()
        {
        }

        /// <summary>
        /// Thuộc tính quy định thay đổi giá trị textbox mà k chờ phải nhấn Enter
        /// </summary>
        public static readonly DependencyProperty CommitOnTypingProperty = DependencyProperty.Register("CommitOnTyping", typeof(bool), typeof(TextBoxEx), new PropertyMetadata(false));
        public bool CommitOnTyping
        {
            get { return (bool)GetValue(CommitOnTypingProperty); }
            set { SetValue(CommitOnTypingProperty, value); }
        }

    }
}
