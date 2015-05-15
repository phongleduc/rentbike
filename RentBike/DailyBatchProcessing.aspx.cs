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
                Logger.TraceService("Auto Extend Contact start at " + DateTime.Now);
                CommonList.AutoExtendContract();
                Logger.TraceService("Auto Extend Contact end at " + DateTime.Now);

                Logger.TraceService("Saving Daily Data start at " + DateTime.Now);
                CommonList.AutoExtendContract();
                Logger.TraceService("Saving Daily Data end at " + DateTime.Now);

            }
            catch (Exception ex)
            {
                Logger.TraceService("Error Entry at: " + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
    }
}