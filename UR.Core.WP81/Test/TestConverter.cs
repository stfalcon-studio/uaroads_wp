using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Newtonsoft.Json;
using UR.Core.WP81.Common;
using UR.Core.WP81.Models;
using UR.Core.WP81.Services;

namespace UR.Core.WP81.Test
{
    public class TestConverter
    {
        public async Task RunAsync()
        {
            var folder = Windows.Storage.ApplicationData.Current.LocalFolder;
            var wf = await folder.GetFolderAsync("Test");

            if (wf == null)
                return;

            var list = new List<Container>();

            var files = await wf.GetFilesAsync();


            var track = await IoC.Get<ITracksProvider>().GetTrackAsync(Guid.NewGuid());
            track.StartedDateTime = DateTime.Now;
            track.Status = ETrackStatus.Recorded;
            track.Comment = DateTime.Now.ToString("U");
            await IoC.Get<ITracksProvider>().SaveTrackAsync(track);


            List<DataGeo> geos = new List<DataGeo>();

            foreach (var storageFile in files)
            {
                //!storageFile.Name.EndsWith("ojson")
                if (!storageFile.Name.EndsWith("cjson"))
                {
                    continue;
                }

                using (var fr = new StreamReader(await storageFile.OpenStreamForReadAsync()))
                {
                    var listPoints = JsonConvert.DeserializeObject<List<DataGeo>>(await fr.ReadToEndAsync());

                    listPoints = listPoints.Select(x => new DataGeo()
                    {
                        Accuracy = x.Accuracy,
                        Latitude = x.Latitude,
                        Longitude = x.Longitude,
                        TimeOffset = x.TimeOffset,
                        Speed = !double.IsNaN(x.Speed) ? (x.Speed * 100 / 60 / 60) * 3.6 : Double.NaN
                    }).ToList();

                    geos.AddRange(listPoints);
                }
            }
            await IoC.Get<ITracksProvider>().WriteToTrackAsync(geos.OrderBy(x => x.TimeOffset).ToList(), track.Id);


            List<DataAccelerometer> acc = new List<DataAccelerometer>();
            foreach (var storageFile in files)
            {
                if (!storageFile.Name.EndsWith("ojson"))
                {
                    continue;
                }

                using (var fr = new StreamReader(await storageFile.OpenStreamForReadAsync()))
                {
                    var listPoints = JsonConvert.DeserializeObject<List<DataAccelerometer>>(await fr.ReadToEndAsync());

                    acc.AddRange(listPoints);
                }
            }
            await IoC.Get<ITracksProvider>().WriteToTrackAsync(acc.OrderBy(x => x.TimeOffset).ToList(), track.Id);
        }

        public class Container
        {
            public DateTimeOffset Time { get; set; }

            public object Value { get; set; }
        }
    }
}
