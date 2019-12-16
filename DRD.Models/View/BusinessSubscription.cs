using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace DRD.Models.View
{
    public class BusinessSubscription
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long Price { get; set; }
        public long DurationInDays { get; set; }
        public int AdministratorQuota { get; set; }
        public long StorageQuotaInByte { get; set; }
        public static BusinessSubscription FromCsv(string csvLine)
        {
            /// TODO: implement ChildCount and filter neccessary standard menu
            string[] values = csvLine.Split(',');
            BusinessSubscription businessSubscription = new BusinessSubscription();
            //System.Diagnostics.Debug.WriteLine(":");
            System.Diagnostics.Debug.WriteLine("THIS IS SPLIT : " + values);
            System.Diagnostics.Debug.WriteLine("THIS IS ORI : " + csvLine);
            if (csvLine != "")
            {
                businessSubscription.Id = Convert.ToInt64(values[0]);
                businessSubscription.Name = Convert.ToString(values[1]);
                businessSubscription.Price = Convert.ToInt64(values[2]);
                businessSubscription.DurationInDays = Convert.ToInt32(values[3]);
                businessSubscription.AdministratorQuota = Convert.ToInt32(values[4]);
                businessSubscription.StorageQuotaInByte = Convert.ToInt64(values[5]);
            }
            return businessSubscription;
        }
    }
}
