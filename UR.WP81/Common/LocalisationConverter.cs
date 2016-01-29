using System;
using Windows.UI.Xaml.Data;
using UR.Core.WP81.Common;

namespace UR.WP81.Common
{
    public class LocConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is ETrackStatus)
            {
                var tStatus = (ETrackStatus)value;

                switch (tStatus)
                {
                    case ETrackStatus.Recording:
                        return "Йде запис";
                    case ETrackStatus.Recorded:
                        return "Записано";
                    case ETrackStatus.RecordPaused:
                        return "Запис призупинено";
                    case ETrackStatus.Processed:
                        return "Опрацьовано";
                    case ETrackStatus.Processing:
                        return "Обробка";
                    case ETrackStatus.Sending:
                        return "Відправка";
                    case ETrackStatus.Sent:
                        return "Відправлено";
                    case ETrackStatus.Error:
                        return "Помилка";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}