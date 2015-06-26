using RentBike.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RentBike
{
    public partial class FormTest : FormBase
    {
        static readonly object _object = new object();
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            //string del = Request.QueryString["del"];
            //if (!string.IsNullOrEmpty(del))
            //{
            //    using (var db = new RentBikeEntities())
            //    {
            //        switch (del)
            //        {
            //            //Truncate all table
            //            case "all":
            //                db.Database.ExecuteSqlCommand("TRUNCATE TABLE Contract");
            //                db.Database.ExecuteSqlCommand("TRUNCATE TABLE Account");
            //                db.Database.ExecuteSqlCommand("TRUNCATE TABLE AccountPermission");
            //                db.Database.ExecuteSqlCommand("TRUNCATE TABLE City");
            //                db.Database.ExecuteSqlCommand("TRUNCATE TABLE ContractHistory");
            //                db.Database.ExecuteSqlCommand("TRUNCATE TABLE Customer");
            //                db.Database.ExecuteSqlCommand("TRUNCATE TABLE InOut");
            //                db.Database.ExecuteSqlCommand("TRUNCATE TABLE Log");
            //                db.Database.ExecuteSqlCommand("TRUNCATE TABLE PayPeriod");
            //                db.Database.ExecuteSqlCommand("TRUNCATE TABLE ReferencePerson");
            //                db.Database.ExecuteSqlCommand("TRUNCATE TABLE RentType");
            //                db.Database.ExecuteSqlCommand("TRUNCATE TABLE Store");
            //                db.Database.ExecuteSqlCommand("TRUNCATE TABLE StoreFee");
            //                break;
            //            //Truncate by table name
            //            default:
            //                db.Database.ExecuteSqlCommand("TRUNCATE TABLE "+ del);
            //                break;
            //        }
            //        db.SaveChanges();
            //        lblMessage.Text = "Delete data successfully!!!";
            //    }
            //}
        }
        protected void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Singleton.IsRunningBatch())
                {
                    lock (_object)
                    {
                        Singleton.CreateSingletonFile();

                        Logger.Log("Backup database into dropbox start");
                        DropboxHelper.BackUp();
                        Logger.Log("Backup database into dropbox end");

                        Singleton.DeleteSingletonFile();
                    }
                }
                lblTest.Text = "Backup successful!";
            }
            catch (Exception ex)
            {
                Singleton.DeleteSingletonFile();
                Logger.Log(ex.Message + Environment.NewLine + ex.StackTrace);
                lblTest.Text = ex.Message;
            }
        }
    }
}