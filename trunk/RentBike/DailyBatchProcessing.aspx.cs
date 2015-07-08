using RentBike.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RentBike
{
    public partial class DailyBatchProcessing : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Lock on the readonly object.
                if (!Singleton.IsRunningBatch())
                {
                    Singleton.CreateSingletonFile();

                    Logger.Log("Backup database start");
                    CommonList.BackUp();
                    Logger.Log("Backup database end");

                    Logger.Log("Save summary fee daily start");
                    CommonList.SaveSummaryPayFeeDaily();
                    Logger.Log("Save summary fee daily end");

                    Logger.Log("Auto extend contract start");
                    CommonList.AutoExtendContract();
                    Logger.Log("Auto extend contract end");

                    Logger.Log("Auto create dummy inout start");
                    CommonList.CreateDummyInout();
                    Logger.Log("Auto create dummy inout end");

                    Logger.Log("Backup database into dropbox start");
                    DropboxHelper.BackUp();
                    Logger.Log("Backup database into dropbox end");

                    Singleton.DeleteSingletonFile();
                }
            }
            catch (Exception ex)
            {
                Singleton.DeleteSingletonFile();
                Logger.Log("Error Entry at: " + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
    }
}