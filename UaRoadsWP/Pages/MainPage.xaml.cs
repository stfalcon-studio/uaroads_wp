﻿using System;
using System.Diagnostics;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using UaRoadsWP.Models.Db;
using UaRoadsWP.Services;
using UaRoadsWpApi;

namespace UaRoadsWP.Pages
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);


            //await new DbStorageService().TrackInsertUpdate(new DbTrack()
            //{
            //    TrackComment = "comment " + DateTime.Now.ToString("g"),
            //    TrackStatus = ETrackStatus.Started
            //});

            //var tracks = await new DbStorageService().TracksGet();

            //foreach (var dbTrack in tracks)
            //{
            //    Debug.WriteLine("{0} {1} {2} {3}", dbTrack.Id, dbTrack.TrackId, dbTrack.TrackComment, dbTrack.TrackStatus);
            //}


            //var res = await new ApiClient().Login("test@test.com", Environment.OSVersion.Platform.ToString(), Microsoft.Phone.Info.DeviceStatus.DeviceName, Environment.OSVersion.Version.ToString(), Windows.Phone.System.Analytics.HostInformation.PublisherHostId);

        }

        private void NavigateToAllTacksPageClick(object sender, EventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Pages/TracksPage.xaml", UriKind.Relative));
        }
    }
}