using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace RentBike.Common
{
    public class Singleton
    {
        public static void CreateSingletonFile()
        {
            string fileName = HttpContext.Current.Server.MapPath("~/") + "/logs/" + Constants.SINGLETON_FILE;
            // Create a file to write to. 
            using (StreamWriter sw = File.CreateText(fileName))
            {
                sw.WriteLine("Singleton File.");
            }
        }

        public static bool IsRunningBatch()
        {
            string fileName = HttpContext.Current.Server.MapPath("~/") + "/logs/" + Constants.SINGLETON_FILE;
            return File.Exists(fileName);
        }

        public static void DeleteSingletonFile()
        {
            string fileName = HttpContext.Current.Server.MapPath("~/") + "/logs/" + Constants.SINGLETON_FILE;
            File.Delete(fileName);
        }
    }
}