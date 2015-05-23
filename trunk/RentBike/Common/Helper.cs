using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace RentBike.Common
{
    public class Helper
    {
        public static int parseInt(object o)
        {
            try
            {
                return Convert.ToInt32(o);
            }
            catch
            {
                return 0;
            }
        }

        public static string FormatedAsCurrency(decimal input)
        {
            return String.Format("{0:#,0.#}", input);
        }

        public static object GetPropValue(object source, string property)
        {
            return source.GetType().GetProperty(property).GetValue(source, null);
        }

        public static void SetPropValue(object source, string property, object value)
        {
            source.GetType().GetProperty(property).SetValue(source, value);
        }

        public static string ConvertByteImageToBase64String(byte[] data)
        {
            if (data != null && data.Length > 0)
            {
                StringBuilder base64Buidler = new StringBuilder();
                base64Buidler.Append("data:");

                base64Buidler.Append("image/png");
                base64Buidler.Append(";base64,");
                base64Buidler.Append(Convert.ToBase64String(data));
                return base64Buidler.ToString();
            }
            return string.Empty;
        }

        public static void EmptyCookies()
        {
            HttpCookie aCookie = new HttpCookie("UserName");
            aCookie.Expires = DateTime.Now.AddDays(-1);
            HttpContext.Current.Response.Cookies.Add(aCookie);

            aCookie = new HttpCookie("Password");
            aCookie.Expires = DateTime.Now.AddDays(-1);
            HttpContext.Current.Response.Cookies.Add(aCookie);
        }

        public static string EncryptPassword(string strPassword)
        {
            string encrypted = string.Empty;
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strPassword));

            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }
            encrypted = strBuilder.ToString();

            return encrypted;
        }
    }
}