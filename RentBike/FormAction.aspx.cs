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
    public partial class FormAction : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                TraceService("Another entry at " + DateTime.Now); // my separate static method for do work
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
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message + Environment.NewLine + ex.StackTrace;
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