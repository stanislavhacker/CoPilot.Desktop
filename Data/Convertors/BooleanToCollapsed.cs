using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace CoPilot.Desktop.Data.Convertors
{
    public class BooleanToCollapsed : IValueConverter
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
            return val ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            Visibility vis = (Visibility)value;
            return vis == Visibility.Collapsed;
        }
    }
}
