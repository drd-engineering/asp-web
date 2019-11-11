using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Service

{
    /// <summary> This is an generator for application config, you can get config by code. there is several code available
    /// like : MEM_NO, EMAILUSER, etc. </summary>
    public class AppConfigGenerator
    {
        /// constructor
        public AppConfigGenerator() { }

        /// <summary> This is the function you can use to get any of application config available. </summary>
        /// <param name="keyGiven" > you can fill this parameter use a code, available code is : MEMBER_NUMBER
        /// EMAIL_USER, EMAIL_PASSWORD, EMAIL_SMTP, EMAIL_PORT, EMAIL_USER_DISPLAY, COMPANY_CODE, APPLICATION_NAME, 
        /// DEF_GROUP_USER, EXPIRY_DAY_DOCUMENT, MEMBER_TOPUP, PAYMEMTU_NUMBER, and MINIMAL_TOPUP</param>
        public Dictionary<String, String> GetConstant(string keyGiven)
        {
            if (keyGiven.Equals("MEMBER_NUMBER"))
            {
                Dictionary<String, String> dict = new Dictionary<String, String>();
                dict.Add("id", "1");
                dict.Add("code", "MEMBER_NUMBER");
                dict.Add("description", "Format registration member no");
                dict.Add("value", "yyWW#####");
                dict.Add("flag", "1");
                dict.Add("data_type", "string");
                dict.Add("data_length", "15");
                dict.Add("initial_value", "0");
                dict.Add("user_id", "SYST");
                dict.Add("date_created", "2016-01-01 00:00:00.000");
                dict.Add("date_updated", null);
                return dict;
            }
            else if (keyGiven.Equals("EMAIL_USER"))
            {
                Dictionary<String, String> dict = new Dictionary<String, String>();
                dict.Add("id", "2");
                dict.Add("code", "EMAIL_USER");
                dict.Add("description", "email user untuk kirim email");
                dict.Add("value", "indonesia.drd@gmail.com");
                dict.Add("flag", "0");
                dict.Add("data_type", "string");
                dict.Add("data_length", "0");
                dict.Add("initial_value", "0");
                dict.Add("user_id", "SYST");
                dict.Add("date_created", "2017-01-01 00:00:00.000");
                dict.Add("date_updated", null);
                return dict;
            }
            else if (keyGiven.Equals("EMAIL_PASSWORD"))
            {
                Dictionary<String, String> dict = new Dictionary<String, String>();
                dict.Add("id", "3");
                dict.Add("code", "EMAIL_PASSWORD");
                dict.Add("description", "email pwd untuk kirim email");
                dict.Add("value", "t3d1bud1");
                dict.Add("flag", "0");
                dict.Add("data_type", "string");
                dict.Add("data_length", "0");
                dict.Add("initial_value", "0");
                dict.Add("user_id", "SYST0");
                dict.Add("date_created", "2017-01-01 00:00:00.000");
                dict.Add("date_updated", null);
                return dict;
            }
            else if (keyGiven.Equals("EMAIL_SMTP"))
            {
                Dictionary<String, String> dict = new Dictionary<String, String>();
                dict.Add("id", "4");
                dict.Add("code", "EMAIL_SMTP");
                dict.Add("description", "email smtp untuk kirim email");
                dict.Add("value", "t3d1bud1");
                dict.Add("flag", "smtp.gmail.com");
                dict.Add("data_type", "string");
                dict.Add("data_length", "0");
                dict.Add("initial_value", "0");
                dict.Add("user_id", "SYST");
                dict.Add("date_created", "2017-01-01 00:00:00.000");
                dict.Add("date_updated", null);
                return dict;
            }
            else if (keyGiven.Equals("EMAIL_PORT"))
            {
                Dictionary<String, String> dict = new Dictionary<String, String>();
                dict.Add("id", "5");
                dict.Add("code", "EMAIL_PORT");
                dict.Add("description", "email smtp port untuk kirim email");
                dict.Add("value", "587");
                dict.Add("flag", "0");
                dict.Add("data_type", "string");
                dict.Add("data_length", "0");
                dict.Add("initial_value", "0");
                dict.Add("user_id", "SYST");
                dict.Add("date_created", "2017-01-01 00:00:00.000");
                dict.Add("date_updated", null);
                return dict;
            }
            else if (keyGiven.Equals("EMAIL_USER_DISPLAY"))
            {
                Dictionary<String, String> dict = new Dictionary<String, String>();
                dict.Add("id", "6");
                dict.Add("code", "EMAIL_USER_DISPLAY");
                dict.Add("description", "email addres from display name");
                dict.Add("value", "DRD");
                dict.Add("flag", "0");
                dict.Add("data_type", "string");
                dict.Add("data_length", "0");
                dict.Add("initial_value", "0");
                dict.Add("user_id", "SYST");
                dict.Add("date_created", "2017-01-01 00:00:00.000");
                dict.Add("date_updated", null);
                return dict;
            }

            else if (keyGiven.Equals("COMPANY_CODE"))
            {
                Dictionary<String, String> dict = new Dictionary<String, String>();
                dict.Add("id", "7");
                dict.Add("code", "COMPANY_CODE");
                dict.Add("description", "Company code");
                dict.Add("value", "DRDyy#####");
                dict.Add("flag", "1");
                dict.Add("data_type", "string");
                dict.Add("data_length", "15");
                dict.Add("initial_value", "0");
                dict.Add("user_id", "SYST");
                dict.Add("date_created", "2016-01-01 00:00:00.000");
                dict.Add("date_updated", null);
                return dict;
            }
            else if (keyGiven.Equals("APPLICATION_NAME"))
            {
                Dictionary<String, String> dict = new Dictionary<String, String>();
                dict.Add("id", "8");
                dict.Add("code", "APPLICATION_NAME");
                dict.Add("description", "Name of application");
                dict.Add("value", "DRD Indonesia");
                dict.Add("flag", "1");
                dict.Add("data_type", "string");
                dict.Add("data_length", "15");
                dict.Add("initial_value", "0");
                dict.Add("user_id", "SYST");
                dict.Add("date_created", "2016-01-01 00:00:00.000");
                dict.Add("date_updated", null);
                return dict;
            }
            else if (keyGiven.Equals("DEF_GROUP_USER"))
            {
                Dictionary<String, String> dict = new Dictionary<String, String>();
                dict.Add("id", "9");
                dict.Add("code", "DEF_GROUP_USER");
                dict.Add("description", "New user reg user group");
                dict.Add("value", "Recipient");
                dict.Add("flag", "1");
                dict.Add("data_type", "string");
                dict.Add("data_length", "15");
                dict.Add("initial_value", "0");
                dict.Add("user_id", "SYST");
                dict.Add("date_created", "2016-01-01 00:00:00.000");
                dict.Add("date_updated", null);
                return dict;
            }
            else if (keyGiven.Equals("EXPIRY_DAY_DOCUMENT"))
            {
                Dictionary<String, String> dict = new Dictionary<String, String>();
                dict.Add("id", "10");
                dict.Add("code", "EXPIRY_DAY_DOCUMENT");
                dict.Add("description", "Expiry day doc after completed");
                dict.Add("value", "3");
                dict.Add("flag", "1");
                dict.Add("data_type", "int");
                dict.Add("data_length", "15");
                dict.Add("initial_value", "0");
                dict.Add("user_id", "SYST");
                dict.Add("date_created", "2016-01-01 00:00:00.000");
                dict.Add("date_updated", null);
                return dict;
            }
            else if (keyGiven.Equals("MEMBER_TOPUP"))
            {
                Dictionary<String, String> dict = new Dictionary<String, String>();
                dict.Add("id", "11");
                dict.Add("code", "MEMBER_TOPUP");
                dict.Add("description", "member topup");
                dict.Add("value", "MTUyyMM######");
                dict.Add("flag", "1");
                dict.Add("data_type", "string");
                dict.Add("data_length", "15");
                dict.Add("initial_value", "0");
                dict.Add("user_id", "SYST");
                dict.Add("date_created", "2016-01-01 00:00:00.000");
                dict.Add("date_updated", null);
                return dict;
            }
            else if (keyGiven.Equals("PAYMEMTU_NUMBER"))
            {
                Dictionary<String, String> dict = new Dictionary<String, String>();
                dict.Add("id", "12");
                dict.Add("code", "PAYMEMTU_NO");
                dict.Add("description", "numbering payment order member topup");
                dict.Add("value", "PMTUyyWW######");
                dict.Add("flag", "1");
                dict.Add("data_type", "string");
                dict.Add("data_length", "15");
                dict.Add("initial_value", "0");
                dict.Add("user_id", "SYST");
                dict.Add("date_created", "2016-01-01 00:00:00.000");
                dict.Add("date_updated", null);
                return dict;
            }
            else if (keyGiven.Equals("MINIMAL_TOPUP"))
            {
                Dictionary<String, String> dict = new Dictionary<String, String>();
                dict.Add("id", "13");
                dict.Add("code", "MIN_TOPUP");
                dict.Add("description", "Minimal topup");
                dict.Add("value", "1250");
                dict.Add("flag", "1");
                dict.Add("data_type", "int");
                dict.Add("data_length", "15");
                dict.Add("initial_value", "0");
                dict.Add("user_id", "SYST");
                dict.Add("date_created", "2016-01-01 00:00:00.000");
                dict.Add("date_updated", null);
                return dict;
            }
            return null;
        }
        public long LongRandom(long min, long max, Random rand)
        {
            byte[] buf = new byte[8];
            rand.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);

            return (Math.Abs(longRand % (max - min)) + min);
        }
    }   
}
