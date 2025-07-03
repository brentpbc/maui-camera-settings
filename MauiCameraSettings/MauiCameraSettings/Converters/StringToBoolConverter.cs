using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace MauiCameraSettings.Converters;

public class StringToBoolConverter : BindableObject,IValueConverter
{        

    public object Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
    {            
            
        if (value == null) return false;
        string s = (string)value;
        if (string.IsNullOrEmpty(s))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public object ConvertBack(object value, Type targetType,
        object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

}