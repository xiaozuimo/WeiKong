using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace cqmu.MVVM.Extension
{
    public static class StorageFolderExtension
    {
        public static async Task<bool> FolderExists(this StorageFolder folder,string folderName)
        {
            foreach(var item in await folder.GetFoldersAsync())
            {
                if (item.Name == folderName)
                    return true;
            }
            return false;
        }

        public static async Task<bool> FileExists(this StorageFolder folder, string fileName)
        {
            foreach(var item in await folder.GetFilesAsync())
            {
                if (item.Name == fileName)
                    return true;
            }
            return false;
        }

    }
}
