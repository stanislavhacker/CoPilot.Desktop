using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace CoPilot.Desktop.Data.Convertors
{
    public class BooleanToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            Boolean val;
            if (value == null)
            {
                val = false;
            }
            else if (value.GetType() == typeof(Boolean))
            {
                val = (Boolean)value;
            }
            else
            {
                val = true;
            }
            return val ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            Visibility vis = (Visibility)value;
            return vis == Visibility.Visible;
        }
    }
}
