using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;

namespace Microsoft.Expression.Framework.Controls
{
    [GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
    public sealed class ProductKeyEditControl : TextBox, INotifyPropertyChanged, IComponentConnector
    {
        private const int MAXLENGTH = 29;

        private bool isInOnTextChanging;

        private string formattedKey = string.Empty;

        public readonly static DependencyProperty IsWellFormedProperty;

        private bool _contentLoaded;

        public bool IsWellFormed
        {
            get
            {
                return (bool)base.GetValue(ProductKeyEditControl.IsWellFormedProperty);
            }
            set
            {
                if (value != this.IsWellFormed)
                {
                    base.SetValue(ProductKeyEditControl.IsWellFormedProperty, value);
                    this.OnPropertyChanged("IsWellFormed");
                }
            }
        }

        static ProductKeyEditControl()
        {
            ProductKeyEditControl.IsWellFormedProperty = DependencyProperty.Register("IsWellFormed", typeof(bool), typeof(ProductKeyEditControl), new PropertyMetadata(false));
        }

        public ProductKeyEditControl()
        {
            this.InitializeComponent();
            InputMethod.SetIsInputMethodEnabled(this, false);
        }

        private ProductKeyEditControl.CanonicalizationInfo CanonicalizeKey(string formattedKey)
        {
            int num = 0;
            StringBuilder stringBuilder = new StringBuilder(formattedKey.Length * 2);
            for (int i = 0; i < formattedKey.Length && stringBuilder.Length <= 29; i++)
            {
                string str = formattedKey[i].ToString();
                if (str != "-")
                {
                    stringBuilder.Append(str.Trim().ToUpperInvariant());
                }
                else
                {
                    num++;
                }
            }
            return new ProductKeyEditControl.CanonicalizationInfo(stringBuilder.ToString(), num);
        }

        private string FormatKey(string newKey)
        {
            StringBuilder stringBuilder = new StringBuilder(newKey.Length * 2);
            if (!string.IsNullOrEmpty(newKey))
            {
                for (int i = 0; i < Math.Min(newKey.Length, 25); i++)
                {
                    string str = newKey[i].ToString();
                    if (Regex.Match(str, "[0-9a-zA-Z]").Success)
                    {
                        if (i > 0 && i % 5 == 0)
                        {
                            stringBuilder.Append('-');
                        }
                        stringBuilder.Append(str);
                    }
                }
            }
            return stringBuilder.ToString();
        }

        [DebuggerNonUserCode]
        public void InitializeComponent()
        {
            if (this._contentLoaded)
            {
                return;
            }
            this._contentLoaded = true;
            Application.LoadComponent(this, new Uri("/Microsoft.Expression.Framework;component/licensing/productkeyeditcontrol.xaml", UriKind.Relative));
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected sealed override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            if (!this.isInOnTextChanging)
            {
                this.isInOnTextChanging = true;
                ProductKeyEditControl.CanonicalizationInfo canonicalizationInfo = this.CanonicalizeKey(this.formattedKey);
                ProductKeyEditControl.CanonicalizationInfo canonicalizationInfo1 = this.CanonicalizeKey(base.Text);
                int caretIndex = base.CaretIndex;
                string str = this.FormatKey(canonicalizationInfo1.CanonicalizedKey);
                canonicalizationInfo1 = this.CanonicalizeKey(str);
                base.Text = str;
                this.formattedKey = str;
                int num = Math.Max(caretIndex + (canonicalizationInfo1.DashCount - canonicalizationInfo.DashCount), 0);
                base.CaretIndex = num;
                this.isInOnTextChanging = false;
            }
            this.IsWellFormed = Regex.Match(base.Text, "([0-9a-zA-Z]{5}-){4}[0-9a-zA-Z]{5}").Success;
        }

        [DebuggerNonUserCode]
        [EditorBrowsable(EditorBrowsableState.Never)]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
        {
            this._contentLoaded = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private struct CanonicalizationInfo
        {
            private string canonicalizedString;

            private int dashCount;

            internal string CanonicalizedKey
            {
                get
                {
                    return this.canonicalizedString;
                }
            }

            internal int DashCount
            {
                get
                {
                    return this.dashCount;
                }
            }

            internal CanonicalizationInfo(string canonicalizedString, int dashCount)
            {
                this.canonicalizedString = canonicalizedString;
                this.dashCount = dashCount;
            }

            public override string ToString()
            {
                CultureInfo invariantCulture = CultureInfo.InvariantCulture;
                object[] objArray = new object[] { this.canonicalizedString, this.dashCount };
                return string.Format(invariantCulture, "Key: {0} Count: {1}", objArray);
            }
        }
    }
}