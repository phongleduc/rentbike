using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace TSolution_Service
{
    public partial class TSolutionService : ServiceBase
    {
                private System.Timers.Timer timer;

                public TSolutionService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            this.timer = new System.Timers.Timer(60000);  // 60000 milliseconds = 60 seconds
            this.timer.AutoReset = true;
            this.timer.Elapsed += new System.Timers.ElapsedEventHandler(this.timer_Elapsed);
            this.timer.Start();
        }

        protected override void OnStop()
        {
            this.timer.Stop();
            this.timer = null;
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (DateTime.Now.Hour == 0 && DateTime.Now.Minute == 1)
                {
                    // Create a request using a URL that can receive a post. 
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://rentbike.com/FormAction.aspx");

                    // Set the Method property of the request to POST.
                    request.Method = WebRequestMethods.Http.Get;

                    // Get the response.
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        // Get the stream containing content returned by the server.
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // Read the content.
                            string content = reader.ReadToEnd();
                            TraceService(content);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TraceService(ex.Message);
            }
        }

        private void TraceService(string content)
        {

            //set up a filestream
            using (FileStream fs = new FileStream(string.Format(@"C:\inetpub\websites\RentBike\log\log_{0}.txt", DateTime.Now.ToString("yyyyMMddHHmmss")), FileMode.OpenOrCreate, FileAccess.Write))
            {
                //set up a streamwriter for adding text
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    //find the end of the underlying filestream
                    sw.BaseStream.Seek(0, SeekOrigin.End);

                    //add the text
                    sw.WriteLine(content);
                }
            }
        }
    }
}
