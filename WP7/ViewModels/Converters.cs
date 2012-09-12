using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Diagnostics;

using MonkeySpace.Core;

namespace MonkeySpace
{
    public class NewsMonthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime dt;
            if (value is DateTime)
               dt  = (DateTime)value;
            else
            {
                DateTime.TryParse(value.ToString(), out dt);
            }
            return dt.ToString("MMM", culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class NewsDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTimeOffset dt = (DateTimeOffset)value; return dt.ToString("dd", culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // http://www.eggheadcafe.com/sample-code/SilverlightWPFandXAML/6e55e7c5-fa65-40bd-8af1-9583c666c1b9/ivalueconverter-for-bool-to-visibility.aspx
    public class EmptyListVisibilityConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return System.Windows.Visibility.Collapsed;
            try
            {
                var i = (ObservableCollection<Session>)value;
                if (i.Count == 0) return System.Windows.Visibility.Visible;
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
            return System.Windows.Visibility.Collapsed;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
    // http://www.eggheadcafe.com/sample-code/SilverlightWPFandXAML/6e55e7c5-fa65-40bd-8af1-9583c666c1b9/ivalueconverter-for-bool-to-visibility.aspx
    public class NonEmptyListVisibilityConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return System.Windows.Visibility.Collapsed;
            try
            {
                var i = (ObservableCollection<Session>)value;
                if (i.Count > 0) return System.Windows.Visibility.Visible;
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
            return System.Windows.Visibility.Collapsed;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
