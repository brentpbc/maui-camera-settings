using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace MauiCameraSettings.Converters;

class InverseBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
        if (value == null) return false;
        bool b = (bool)value;
        return !b;
    }

    public object ConvertBack(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}