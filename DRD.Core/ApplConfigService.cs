using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DRD.Domain;
using System.Globalization;
using System.Based.Core;
using System.Based.Core.Entity;

namespace DRD.Service
{
    public class ApplConfigService
    {
        private readonly string _connString;

        public ApplConfigService(string connString)
        {
            _connString = connString;
        }
        public ApplConfigService()
        {
            _connString = ConfigConstant.CONSTRING;
        }

        public string GetValue(string configCode)
        {
            using (var db = new DrdContext(_connString))
            {
                var appl = db.ApplConfigs.FirstOrDefault(c => c.Code == configCode);
                if (appl == null) return null;

                return appl.Value;
            }
        }

        public string GenerateNumber<T>(string configCode, string tableName,
            string fieldName, System.Data.Entity.DbSet<T> tableClass) where T : class
        {
            string num = "";
            string value = this.GetValue(configCode);
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

            using (var db = new DrdContext(_connString))
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
