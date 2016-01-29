using Caliburn.Micro;
using Eve.Caliburn;
using UR.WP81.ViewModels;
using UR.WP81.Views;

namespace UR.WP81.Common
{
    public static class AppNavigationExtensions
    {
        public static void ToLoginPage(this Caliburn.Micro.INavigationService ns, bool backOnSucsseful = false)
        {
            ns.UriFor<LoginPageViewModel>().WithParam(x => x.BackOnSuccessfulLogin, backOnSucsseful).Navigate();
        }

        public static void ToSettingsPage(this Caliburn.Micro.INavigationService ns, bool backOnSucsseful = false)
        {
            ns.UriFor<SettingsPageViewModel>().Navigate();
        }

        public static void ToMainPage(this Caliburn.Micro.INavigationService ns)
        {
            ns.CleanNavigationStackAfter().UriFor<MainPageViewModel>().Navigate();
        }

        //public static void ToSearchPage(this INavigationService ns, string searchString = "")
        //{
        //    ns.UriFor<SearchPageViewModel>().WithParam(x => x.SearchText, searchString).Navigate();
        //}

        //public static void ToMainPage(this INavigationService ns)
        //{
        //    if (ns.CurrentPageIs(typeof(SplashScreenPage)))
        //    {
        //        if (!Megogo.Services.SettingsService.IsWelcomeScreenShown)
        //        {
        //            //SettingsService.IsWelcomeScreenShown = true;
        //            ns.CleanNavigationStackAfter().ToWelcomePage();
        //            return;
        //        }
        //    }

        //    //ns.CleanNavigationStackAfter().ToWelcomePage();
        //    //return;

        //    ns.CleanNavigationStackAfter().UriFor<MainPageViewModel>().Navigate();
        //}

        //public static void ToVideoPlayerPage(this INavigationService ns, AVideoFullData video, int selectedEpisode = -1)
        //{
        //    ns.UriFor<VideoPlayerPageViewModel>()
        //        .WithParam(m => m.NavVideoId, video.Id)
        //        .WithParam(m => m.NavSelectedEpisodeId, selectedEpisode)
        //        .Navigate();
        //}

        ////public static void ToVideoPage(this INavigationService ns, int videoId, EVideoType type = EVideoType.None)
        ////{
        ////    ns.UriFor<VideoPageViewModel>()
        ////        .WithParam(m => m.VideoId, videoId)
        ////        .WithParam(m => m.VideoType, type)
        ////        .Navigate();
        ////}


        //public static void ToVideoPage(this INavigationService ns, AVideoShortData data)
        //{
        //    if (data != null)
        //    {
        //        ToVideoPage(ns, data.Id, 0, data.Categories);
        //    }
        //}

        //public static void ToVideoPage(this INavigationService ns, AVideoFullData data)
        //{
        //    if (data != null)
        //    {
        //        ToVideoPage(ns, data.Id, data.CategoryId, data.Categories);
        //    }
        //}

        //private static void ToVideoPage(INavigationService ns, int objectId, int categoryId, int[] categories)
        //{
        //    EVideoType cat = EVideoType.None;

        //    //int objectId = 0;

        //    if (categoryId != 0)
        //    {
        //        Enum.TryParse(categoryId.ToString(), out cat);
        //    }

        //    if (cat == EVideoType.None)
        //    {
        //        if (categories != null && categories.Any())
        //        {
        //            var catId = categories.First();

        //            Enum.TryParse(catId.ToString(), out cat);
        //        }
        //    }

        //    ns.UriFor<VideoPageViewModel>()
        //        .WithParam(m => m.VideoId, objectId)
        //        .WithParam(m => m.VideoType, cat)
        //        .Navigate();
        //}

        //public static void ToWelcomePage(this INavigationService ns)
        //{
        //    ns.UriFor<WelcomePageViewModel>().Navigate();
        //}

        //public static void ToCategoryDetailsPage(this INavigationService ns, int categoryId)
        //{
        //    ns.UriFor<CategoryDetailsPageViewModel>()
        //        .WithParam(x => x.CategoryId, categoryId)
        //        //.WithParam(x => x.Header, header)
        //        .Navigate();
        //}


        //public static void ToCollectionsPage(this INavigationService ns)
        //{
        //    ns.UriFor<CollectionsPageViewModel>()
        //        //.WithParam(x => x.CategoryId, categoryId)
        //        .Navigate();
        //}

        //public static void ToCollectionDetailsPage(this INavigationService ns, int collectionId)
        //{
        //    ns.UriFor<CollectionDetailsPageViewModel>()
        //        .WithParam(x => x.CollectionId, collectionId)
        //        .Navigate();
        //}


        //public static void ToPlusPage(this INavigationService ns)
        //{
        //    //if (ns.CurrentPageIs(typeof(WelcomePage)))
        //    //{
        //    //    var bsMainPage = new PageStackEntry(typeof(MainPage), "caliburn://megogo/Views/MainPage.xaml", null);

        //    //    ns.CleanNavigationStackAfter()
        //    //        .AddToBackStackAfter(bsMainPage)
        //    //        .UriFor<PlusPageViewModel>()
        //    //        .Navigate();
        //    //    return;
        //    //}

        //    ns.UriFor<PlusPageViewModel>()
        //        .Navigate();
        //}

        //public static void ToPurchasedPage(this INavigationService ns)
        //{
        //    ns.UriFor<PurchasedPageViewModel>()
        //        .Navigate();
        //}

        //public static void ToPremierePage(this INavigationService ns)
        //{
        //    //if (ns.CurrentPageIs(typeof(WelcomePage)))
        //    //{
        //    //    var bsMainPage = new PageStackEntry(typeof(MainPage), "caliburn://megogo/Views/MainPage.xaml", null);

        //    //    ns.CleanNavigationStackAfter()
        //    //        .AddToBackStackAfter(bsMainPage)
        //    //        .UriFor<PremierePageViewModel>()
        //    //        .Navigate();
        //    //    return;
        //    //}

        //    ns.UriFor<PremierePageViewModel>()
        //        .Navigate();
        //}


        //public static void ToAccountPage(this INavigationService ns)
        //{
        //    ns.UriFor<AccountPageViewModel>()
        //        .Navigate();
        //}

        //public static void ToFavoritesPage(this INavigationService ns)
        //{
        //    ns.UriFor<FavoritesPageViewModel>()
        //        .Navigate();

        //}

        //public static void ToRegisterPage(this INavigationService ns)
        //{
        //    ns.UriFor<RegisterPageViewModel>()
        //        .Navigate();
        //}

        //public static void ToSubscriptionInfoPage(this INavigationService ns)
        //{
        //    ns.UriFor<SubscriptionInfoPageViewModel>()
        //        .Navigate();
        //}

        //public static void ToPlusDetailsPage(this INavigationService ns, int categoryId, string header)
        //{
        //    ns.UriFor<PlusDetailsPageViewModel>()
        //        .WithParam(x => x.CategoryId, categoryId)
        //        .WithParam(x => x.Header, header)
        //        .Navigate();
        //}

        //public static void ToTvPage(this INavigationService ns)
        //{
        //    ns.UriFor<TvPageViewModel>()
        //        .Navigate();
        //}

        //public static void ToSettingsPage(this INavigationService ns)
        //{
        //    //todo fix later
        //    ns.UriFor<AboutPageViewModel>()
        //        .Navigate();
        //}



        //public static void ToAdminPage(this INavigationService ns)
        //{
        //    ns.UriFor<AdminPageViewModel>().Navigate();
        //}



        //public static void ToPlayTvChannel(this INavigationService ns, int channelId)
        //{
        //    ns.UriFor<VideoPlayerPageViewModel>()
        //          .WithParam(x => x.NavVideoId, channelId)
        //          .WithParam(x => x.NavSelectedEpisodeId, -1)
        //          .WithParam(x => x.NavIsTv, true)
        //          .Navigate();
        //}
    }

}
