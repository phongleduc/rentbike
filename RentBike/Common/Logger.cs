using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace RentBike.Common
{
    public class Logger
    {
        public static void TraceService(string content)
        {

            //set up a filestream
            using (FileStream fs = new FileStream(WebConfigurationManager.AppSettings["RentBike.LogFolder"] + string.Format("log_{0}.txt", DateTime.Now.ToString("yyyyMMdd")), FileMode.OpenOrCreate, FileAccess.Write))
            {
                //set up a streamwriter for adding text
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    //find the end of the underlying filestream
                    sw.BaseStream.Seek(0, SeekOrigin.End);

                    //add the text
                    sw.WriteLine(DateTime.Now + ": " + content);
                }
            }
        }
    }
}