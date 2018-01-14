using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CommonLib.Behavior
{
    public static class NumberOnlyBehavior
    {
        private static string Expression = "[0-9]+";

        public static readonly DependencyProperty IsNumberOnlyProperty =
            DependencyProperty.RegisterAttached("IsNumberOnly",
                                                typeof(bool),
                                                typeof(NumberOnlyBehavior),
                                                new PropertyMetadata(false, OnChanged));

        public static bool GetIsNumberOnly(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsNumberOnlyProperty);
        }

        public static void SetIsNumberOnly(DependencyObject obj, bool value)
        {
            obj.SetValue(IsNumberOnlyProperty, value);
        }

        private static void OnChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as Control;
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

        /// <summary>
        /// [GM70A-23] TextInput Event에서는 Space를 Detecting 하지 못하기 때문에 KeyDown을 통해서 막아야 함
        /// http://wpf.2000things.com/2012/09/03/638-previewtextinput-is-not-fired-in-many-cases/
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = (e.Key == Key.Space || e.Key == Key.ImeProcessed);
        }

        private static void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !ValidateText(e.Text);
        }

        private static bool ValidateText(string text)
        {
            return new Regex(Expression).IsMatch(text);
        }
    }
}
