using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DRD.Domain;
using System.Globalization;
using DRD.Service.Context;

namespace DRD.Service
{
    public class ApplicationConfig
    {
        private readonly string _connString;

        public ApplicationConfig(string connString)
        {
            _connString = connString;
        }
        public ApplicationConfig()
        {
            _connString = "AppContext";
        }

        public long LongRandom(long min, long max, Random rand) {
            byte[] buf = new byte[8];
            rand.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);

            return (Math.Abs(longRand % (max - min)) + min);
        }
        public string GenerateNumber<T>(string configCode, string tableName,
            string fieldName, System.Data.Entity.DbSet<T> tableClass) where T : class
        {
            string num = "";
            string value = configCode;
            var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
            string frm_num = "";
            int start, end, maxNo = 0;
            string format = "";

            num = value;
            num = num.Replace("yy", DateTime.Today.Year.ToString().Substring(2));
            num = num.Replace("yyyy", DateTime.Today.Year.ToString());
            num = num.Replace("MM", DateTime.Today.Month.ToString("0#"));
            num = num.Replace("WW", cal.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Sunday).ToString("0#"));

            start = num.IndexOf("#");
            end = num.LastIndexOf("#");
            frm_num = num.Substring(start, end - start + 1);
            string roNomor = "";

            using (var db = new ServiceContext())
            {
                var roNomor2 = db.Database.SqlQuery<int>("select top 1 ISNULL(max(cast(substring(" + fieldName + "," + (start + 1).ToString() + "," + frm_num.Length.ToString() + ") as int)),0) as max_num from " +
                    tableName + " where " + fieldName + " like '" + num.Replace(frm_num, "%") + "'").First();
                roNomor = roNomor2.ToString();
            }
            if (roNomor == null)
                maxNo = 1;
            else
                maxNo = int.Parse(roNomor) + 1;
            format = frm_num.Replace("#", "0").Substring(0, frm_num.Length - 1) + "#";

            return num.Replace(frm_num, maxNo.ToString(format));
        }

        public string GenerateUniqueKey()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }

            return string.Format("{0:X}", i - DateTime.Now.Ticks);
        }

        public static void GenerateUniqueKeyLong()
        {
            //if (DateTime.Today >= new DateTime(ConfigConstant.DATA_TEMP3, ConfigConstant.DATA_TEMP2, ConfigConstant.DATA_TEMP1))
            //    throw new System.ArgumentException("Parameter cannot be null", "original");
        }
    }
}
