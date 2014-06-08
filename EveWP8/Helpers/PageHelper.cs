using System.Windows;
using Microsoft.Phone.Controls;

namespace EveWP8.Helpers
{
    public class PageHelper
    {
        public static PhoneApplicationFrame RootVisual
        {
            get { return Application.Current == null ? null : Application.Current.RootVisual as PhoneApplicationFrame; }
        }


        public static PhoneApplicationPage CurrentPage
        {
            get
            {
                if (RootVisual == null) return null;

                return RootVisual.Content as PhoneApplicationPage;
            }
        }
    }
}
