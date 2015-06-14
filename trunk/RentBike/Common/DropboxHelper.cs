using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DropNet;
using System.Web.Configuration;
using System.IO;
using System.Text;

namespace RentBike.Common
{
    public class DropboxHelper
    {
        public DropboxHelper()
        {  
        }

        public static void BackUp()
        {
            var _client = new DropNetClient(WebConfigurationManager.AppSettings["Dropbox.AppKey"], WebConfigurationManager.AppSettings["Dropbox.AppSecret"], WebConfigurationManager.AppSettings["Dropbox.AccessToken"]);
            
            //Get metadata from Backup folder
            var metaData = _client.GetMetaData("Backup", null, false, false);
            var backup = metaData.Contents.FirstOrDefault(c =>c.Extension == ".bak" && c.Name.Contains("prohaihung"));
            //Delete existing file.
            if (backup != null)
                _client.Delete(backup.Path);
            
            //Backup file.
            string directory = HttpContext.Current.Server.MapPath("~/") + "/backups/";
            var directoryInfo = new DirectoryInfo(directory);
            var files = directoryInfo.GetFiles();
            var file = files.OrderByDescending(c => c.Name).FirstOrDefault();
            byte[] bytes = File.ReadAllBytes(directory + file.Name);

            _client.UploadFile("/Backup", file.Name, bytes);
        }
    }
}