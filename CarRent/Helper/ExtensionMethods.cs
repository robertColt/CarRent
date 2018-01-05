using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace CarRent.ExtensionMethods
{
    public static class LabelExtension
    {
        public static void ShowSuccess(this Label label, string message)
        {
            label.Content = message;
            label.Foreground = Brushes.DarkGreen;
        }

        public static void ShowError(this Label label, string message)
        {
            label.Content = message;
            label.Foreground = Brushes.DarkRed;
        }
    }
}
