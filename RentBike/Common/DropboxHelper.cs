using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DropNet;
using System.Web.Configuration;

namespace RentBike.Common
{
    public class DropboxHelper
    {
        public DropboxHelper()
        { }

        public void Test()
        {
            var client = new DropNetClient(WebConfigurationManager.AppSettings["Dropbox.AppKey"], WebConfigurationManager.AppSettings["Dropbox.AppSecret"]);

            // Sync
            client.GetToken();

            // Async
            client.GetTokenAsync((userLogin) =>
            {
                //Dont really need to do anything with userLogin, DropNet takes care of it for now
            },
                (error) =>
                {
                    //Handle error
                });
        }
    }
}