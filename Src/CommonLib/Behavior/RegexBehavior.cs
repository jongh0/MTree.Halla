using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CommonLib.Behavior
{
    public static class RegexBehavior
    {
        public static readonly DependencyProperty IsRegexEnabledProperty =
                DependencyProperty.RegisterAttached("IsRegexEnabled",
                                                    typeof(bool),
                                                    typeof(RegexBehavior),
                                                    new PropertyMetadata(false, OnChanged));

        public static bool GetIsRegexEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsRegexEnabledProperty);
        }

        public static void SetIsRegexEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(IsRegexEnabledProperty, value);
        }

        private static void OnChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as TextBox;
            if (control == null) return;

            if ((bool)e.NewValue == true)
            {
                control.PreviewTextInput += OnPreviewTextInput;
                control.PreviewKeyDown += OnPreviewKeyDown;
            }
            else
            {
                control.PreviewTextInput -= OnPreviewTextInput;
                control.PreviewKeyDown -= OnPreviewKeyDown;
            }
        }

        public static readonly DependencyProperty AllowWhiteSpaceProperty =
                DependencyProperty.RegisterAttached("AllowWhiteSpace",
                                                    typeof(bool),
                                                    typeof(RegexBehavior),
                                                    new PropertyMetadata(false));

        public static bool GetAllowWhiteSpace(DependencyObject obj)
        {
            return (bool)obj.GetValue(AllowWhiteSpaceProperty);
        }

        public static void SetAllowWhiteSpace(DependencyObject obj, bool value)
        {
            obj.SetValue(AllowWhiteSpaceProperty, value);
        }

        /// <summary>
        /// TextInput Event에서는 Space를 Detecting 하지 못하기 때문에 KeyDown을 통해서 막아야 함
        /// http://wpf.2000things.com/2012/09/03/638-previewtextinput-is-not-fired-in-many-cases/
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var control = sender as TextBox;
            if (control == null)
                return;

            if (GetAllowWhiteSpace(control) == true) return;

            string expression = (string)control.Tag;
            if (string.IsNullOrEmpty(expression) == true)
                return;

            /// Expression에 띄어쓰기가 포함되어 있는 경우에는 무시한다
            if (expression.Contains(" "))
                return;

            e.Handled = (e.Key == Key.Space);
        }

        private static void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var control = sender as TextBox;
            if (control == null)
                return;

            string expression = (string)control.Tag;
            if (string.IsNullOrEmpty(expression) == true)
                return;

            string text = control.Text;
            text = text.Remove(control.SelectionStart, control.SelectionLength);
            text = text.Insert(control.SelectionStart, e.Text);
            e.Handled = !ValidateText(expression, text);
        }

        private static bool ValidateText(string expression, string text)
        {
            return new Regex(expression).IsMatch(text);
        }
    }
}
