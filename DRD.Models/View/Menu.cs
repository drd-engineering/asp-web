using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.View
{
    public class Menu
    {
        public string Code { get; set; }
        public string SecondaryKey { get; set; }
        public string Name { get; set; }
        public string UrlPage { get; set; }
        public int ChildCount { get; set; }
        public string ParentCode { get; set; }
        public string Icon { get; set; }
        public string ObjectName { get; set; }
        public int ItemType { get; set; }
        public bool IsActive { get; set; }

        public Menu(string Code, string SecondaryKey, string Name, string UrlPage, int ChildCount, string ParentCode, string Icon, string ObjectName, int ItemType, bool IsActive)
        {
            this.Code = Code;
            this.SecondaryKey = SecondaryKey;
            this.Name = Name;
            this.UrlPage = UrlPage;
            this.ChildCount = ChildCount;
            this.ParentCode = ParentCode;
            this.Icon = Icon;
            this.IsActive = IsActive;
            this.ObjectName = ObjectName;
            this.ItemType = ItemType;
        }
        public Menu()
        {
        }

        public static Menu FromCsv(string csvLine)
        {
            /// TODO: implement ChildCount and filter neccessary standard menu
            string[] values = csvLine.Split(',');
            Menu menuItem = new Menu();
            menuItem.Code = Convert.ToString(values[0]);
            menuItem.SecondaryKey = Convert.ToString(values[1]);
            menuItem.Name = Convert.ToString(values[2]);
            menuItem.Icon = Convert.ToString(values[3]);
            menuItem.UrlPage = Convert.ToString(values[4]);
            menuItem.ParentCode = Convert.ToString(values[7]);
            menuItem.ItemType = Convert.ToInt16(values[8]);
            menuItem.ObjectName = Convert.ToString(values[9]);
            menuItem.IsActive = false;
            return menuItem;
        }
    }
}
