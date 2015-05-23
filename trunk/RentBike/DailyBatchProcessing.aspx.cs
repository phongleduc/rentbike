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
                Logger.Log("Backup database start");
                CommonList.BackUp();
                Logger.Log("Backup database end");

                Logger.Log("Save summary fee daily start");
                CommonList.SaveSummaryPayFeeDaily();
                Logger.Log("Save summary fee daily end");

                Logger.Log("Auto extend contract start");
                CommonList.AutoExtendContract();
                Logger.Log("Auto extend contract end");

            }
            catch (Exception ex)
            {
                Logger.Log("Error Entry at: " + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
    }
}