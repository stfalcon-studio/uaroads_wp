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
                        return Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetValue("Resources/LocConverter_Recording").ValueAsString;
                    case ETrackStatus.Recorded:
                        return Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetValue("Resources/LocConverter_Recorded").ValueAsString;
                    case ETrackStatus.RecordPaused:
                        return Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetValue("Resources/LocConverter_RecordPaused").ValueAsString;
                    case ETrackStatus.Processed:
                        return Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetValue("Resources/LocConverter_Processed").ValueAsString;
                    case ETrackStatus.Processing:
                        return Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetValue("Resources/LocConverter_Processing").ValueAsString;
                    case ETrackStatus.Sending:
                        return Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetValue("Resources/LocConverter_Sending").ValueAsString;
                    case ETrackStatus.Sent:
                        return Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetValue("Resources/LocConverter_Sent").ValueAsString;
                    case ETrackStatus.Error:
                        return Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetValue("Resources/LocConverter_Error").ValueAsString;
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