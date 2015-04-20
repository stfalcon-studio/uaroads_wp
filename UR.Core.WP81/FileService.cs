using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace UaRoadsWP.Services
{
    static class FileService
    {
        public async static void CheckFolder()
        {
            if (!System.IO.Directory.Exists(Path.Combine(ApplicationData.Current.LocalFolder.Path, AppConstant.DataFolderName)))
            {
                await ApplicationData.Current.LocalFolder.CreateFolderAsync(AppConstant.DataFolderName);
            }
        }

        public static async Task<StorageFile> GetFileForWrite(string fileName)
        {
            CheckFolder();

            var folder = await ApplicationData.Current.LocalFolder.GetFolderAsync(AppConstant.DataFolderName);

            if (System.IO.File.Exists(Path.Combine(folder.Path, fileName)))
            {
                File.Delete(Path.Combine(folder.Path, fileName));
            }

            var res = await folder.CreateFileAsync(fileName, CreationCollisionOption.FailIfExists);

            return res;
        }

        public static async Task<StorageFile> GetFileForRead(string fileName)
        {
            CheckFolder();

            var folder = await ApplicationData.Current.LocalFolder.GetFolderAsync(AppConstant.DataFolderName);

            var res = await folder.GetFileAsync(fileName);

            return res;
        }


    }
}
