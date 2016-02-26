using System.Reflection;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace DynamicGeometry
{
    public class MethodCallerButton : Button
    {
        public MethodCallerButton()
        {
            this.Click += OnClick;
            //this.Margin = new System.Windows.Thickness(0, 4, 0, 4);
        }

        private void OnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Target != null && OperationDescription != null)
            {
                try
                {
                    object[] arguments = new object[0];
                    if (ParameterGrid != null)
                    {
                        IEnumerable<IValueProvider> parameterValues = ParameterGrid.CurrentProperties;
                        arguments = parameterValues.Select(v => v.GetValue<object>()).ToArray();
                    }
                    OperationDescription.Invoke(Target, arguments);
                }
                catch
                {
                    // whatever happens, we can't allow it bubble up
                    // back to the CLR - we don't trust our method
                }
            }
        }

        public object Target { get; set; }
        public PropertyGrid ParameterGrid { get; set; }

        private IOperationDescription operationDescription;
        public IOperationDescription OperationDescription
        {
            get
            {
                return operationDescription;
            }
            set
            {
                operationDescription = value;
                if (operationDescription != null)
                {
                    string name = operationDescription.DisplayName;

                    var parameters = operationDescription.Parameters;
                    if (parameters.IsEmpty())
                    {
                        this.Content = name;
                    }
                    else
                    {
                        ParameterGrid = new PropertyGrid();
                        ParameterGrid.Title = name;
                        this.Content = ParameterGrid;
                        //this.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
                        //this.VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch;
                        ParameterGrid.Show(operationDescription, null);
                    }
                }
            }
        }

        // Thêm bởi cá cơm
        /// <summary>
        /// Button Icon
        /// </summary>
        public Brush Icon
        {
            get { return (Brush)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        /// <summary>
        /// Button Icon
        /// </summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(Brush), typeof(MethodCallerButton), new PropertyMetadata(Brushes.Transparent));

        /// <summary>Access key to be displayed for the button</summary>
        public string AccessKey
        {
            get { return (string)GetValue(AccessKeyProperty); }
            set { SetValue(AccessKeyProperty, value); }
        }
        /// <summary>Access key to be displayed for the button</summary>
        public static readonly DependencyProperty AccessKeyProperty = DependencyProperty.Register("AccessKey", typeof(string), typeof(MethodCallerButton), new PropertyMetadata(string.Empty, OnAccessKeyChanged));

        /// <summary>Fires when the access key changes</summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnAccessKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var button = d as MethodCallerButton;
            if (button == null) return;
            button.AccessKeySet = !string.IsNullOrEmpty(args.NewValue.ToString());
        }

        /// <summary>Indicates whether a access key has been set</summary>
        /// <value><c>true</c> if [access key set]; otherwise, <c>false</c>.</value>
        public bool AccessKeySet
        {
            get { return (bool)GetValue(AccessKeySetProperty); }
            set { SetValue(AccessKeySetProperty, value); }
        }
        /// <summary>Indicates whether a page access key has been set</summary>
        public static readonly DependencyProperty AccessKeySetProperty = DependencyProperty.Register("AccessKeySet", typeof(bool), typeof(MethodCallerButton), new PropertyMetadata(false));

        public string GroupTitle
        {
            get { return (string)GetValue(GroupTitleProperty); }
            set { SetValue(GroupTitleProperty, value); }
        }
        public static readonly DependencyProperty GroupTitleProperty = DependencyProperty.Register("GroupTitle", typeof(string), typeof(MethodCallerButton), new PropertyMetadata(""));
        /// <summary>
        /// Indicates whether the first File page is to be handled differently as a file menu
        /// </summary>
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(MethodCallerButton), new PropertyMetadata(""));
        /// <summary>
        /// Tên hiển thị của nút bấm
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register("Header", typeof(string), typeof(MethodCallerButton), new PropertyMetadata("New Project"));
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
    }
}
