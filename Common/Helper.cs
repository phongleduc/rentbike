using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RentBike.Common
{
    public class Helper
    {
        public static int parseInt(string str)
        {
            try
            {
                return int.Parse(str);
            }
            catch
            {
                return 0;
            }
        }
    }
}