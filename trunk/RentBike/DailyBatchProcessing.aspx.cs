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
                Logger.TraceService("Backup database start");
                CommonList.BackUp();
                Logger.TraceService("Backup database end");

                Logger.TraceService("Save summary fee daily start");
                CommonList.SaveSummaryPayFeeDaily();
                Logger.TraceService("Save summary fee daily end");

                Logger.TraceService("Auto extend contract start");
                CommonList.AutoExtendContract();
                Logger.TraceService("Auto extend contract end");

            }
            catch (Exception ex)
            {
                Logger.TraceService("Error Entry at: " + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
    }
}