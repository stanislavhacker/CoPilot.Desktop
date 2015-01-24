using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace CoPilot.Desktop.Data.Convertors
{
    public class TrueToFalse : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            Boolean val = (Boolean)value;
            return !val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            return null;
        }
    }
}
