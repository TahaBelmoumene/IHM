using System;
using System.Globalization;
using System.Windows.Data;

namespace IHM.Converters
{
    /// <summary>
    /// Convertit une valeur null en False et une valeur non-null en True.
    /// Utilisé pour activer/désactiver les combobox.
    /// </summary>
    public class NullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Si l'objet (ex: Marque sélectionnée) n'est pas null, on renvoie Vrai (Activé)
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}