using System;
using System.Globalization;
using System.Reflection;
using Xamarin.Forms;

namespace MDDevDaysApp.Converters
{
    public class LocalImagePathConverter : IValueConverter
    {
        private static readonly Assembly Assembly;
        private static readonly string AssemblyPraefix;

        static LocalImagePathConverter()
        {
            Assembly = typeof(LocalImagePathConverter).GetTypeInfo().Assembly;
            AssemblyPraefix = typeof(LocalImagePathConverter).AssemblyQualifiedName.Split(',')[1].Trim() + '.';
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var source = AssemblyPraefix + ((string)value).Replace('/', '.');
            // Angabe der Assembly ist notwendig, damit UWP im Release Mode das Bild auch anzeigt
            return ImageSource.FromResource(source, Assembly);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
