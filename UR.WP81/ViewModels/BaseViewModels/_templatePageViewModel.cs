using Caliburn.Micro;

namespace UR.WP81.ViewModels.BaseViewModels
{
    public class _templatePageViewModel : AppBasePageViewModel
    {
        public _templatePageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
        }

        protected override void OnActivate()
        {
            base.OnActivate();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }

        private async void Load()
        {
        }
    }
}