using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RemoteService.Converters
{
    public class StateColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case "Start Pending":
                    return "#5447C8";
                case "Running":
                    return "#01D275";
                case "Stop Pending":
                    return "#FF9057";
                case "Stopped":
                    return "#D63031";//D91818
                default:
                    return "#838C97";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
