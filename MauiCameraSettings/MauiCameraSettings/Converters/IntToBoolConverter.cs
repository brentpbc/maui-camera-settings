using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace MauiCameraSettings.Converters;

public class IntToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
        if (value is long) //Handle long
        {
            if ((long)value > 0) // number greater than  0 ?
                return true;    // Contains some data
            else
                return false;   // Contains no data
        }

        if ((int)value > 0) // number greater than  0 ?
            return true;    // Contains some data
        else
            return false;   // Contains no data
    }

    public object ConvertBack(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}