using System.Windows;
using System.Windows.Controls;


namespace DynamicGeometry
{
    public class StringEditorFactory 
        : BaseValueEditorFactory<StringEditor, string> { }

    public class StringEditor : LabeledValueEditor, IValueEditor
    {
        public TextBoxEx TextBox { get; set; }

        protected override UIElement CreateEditor()
        {
            TextBox = new TextBoxEx();
            TextBox.TextChanged += StringPropertyEditor_TextChanged;
            TextBox.AcceptsReturn = false;
            return TextBox;
        }

        protected override void Focus()
        {
            TextBox.Focus();
            if (!string.IsNullOrEmpty(TextBox.Text))
            {
                TextBox.SelectAll();
            }
        }
        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            //TextBox.Focus();
            //if (!string.IsNullOrEmpty(TextBox.Text))
            //{
            //    TextBox.SelectAll();
            //}
            //base.OnMouseDown(e);
        }

        void StringPropertyEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            SetValue(TextBox.Text);
        }

        public override void UpdateEditor()
        {
            TextBox.Text = (GetValue() ?? "").ToString();
            TextBox.IsReadOnly = !Value.CanSetValue;
        }
    }
}
