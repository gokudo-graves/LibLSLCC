﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LSLCCEditor.Converters
{
    [ValueConversion(typeof (int), typeof (Visibility))]
    public class IntegerNotZeroToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members


        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            if (((int) value) != 0)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }



        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }


        #endregion
    }

    [ValueConversion(typeof (int), typeof (bool))]
    public class IntegerNotZeroToBoolConverter : IValueConverter
    {
        #region IValueConverter Members


        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            return ((int) value) != 0;
        }



        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }


        #endregion
    }
}