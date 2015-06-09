using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using UR.WP81.ViewModels.BaseViewModels;

namespace UR.WP81.ViewModels
{
    public class SplashPageViewModel : AppBasePageViewModel
    {
        public SplashPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
        }
    }
}
