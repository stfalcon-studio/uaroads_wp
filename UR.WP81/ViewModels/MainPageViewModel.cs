using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using UR.WP81.ViewModels.BaseViewModels;

namespace UR.WP81.ViewModels
{
    class MainPageViewModel : AppBasePageViewModel
    {
        public MainPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
        }
    }
}
