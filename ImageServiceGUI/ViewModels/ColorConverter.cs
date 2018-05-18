using ImageService.Logging.Modal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ImageServiceGUI.ViewModels
{
    class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            MessageTypeEnum type =(MessageTypeEnum)value;
            switch (type)
            {
                case MessageTypeEnum.INFO:
                    return "GREEN";
                case MessageTypeEnum.WARNING:
                    return "YELLOW";
                case MessageTypeEnum.FAIL:
                    return "RED";
                default:
                    return "TRANSPARENT";

            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
