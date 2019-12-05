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

        public MenuMaster MenuMaster { get; set; } // FK_UserMenu_MenuMaster

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
            //System.Diagnostics.Debug.WriteLine(":");
            System.Diagnostics.Debug.WriteLine(values);
            //System.Diagnostics.Debug.WriteLine(values.ToString());
            if (menuItem != null)
            {
                menuItem.Code = Convert.ToString(values[0]);
                menuItem.SecondaryKey = Convert.ToString(values[1]);
                menuItem.Name = Convert.ToString(values[2]);
                menuItem.Icon = Convert.ToString(values[3]);
                menuItem.UrlPage = Convert.ToString(values[4]);
                menuItem.ParentCode = Convert.ToString(values[5]);
                menuItem.ObjectName = Convert.ToString(values[6]);
                menuItem.ItemType = Convert.ToInt16(values[7]);
                menuItem.IsActive = false;
            }
            return menuItem;
        }
    }
    public class MenuMaster
    {
        public int Id { get; set; } // ID (Primary key)
        public string SecondaryKey { get; set; } // SecondaryKey (length: 20)
        public string Caption { get; set; } // Caption (length: 100)
        public string Icon { get; set; } // Icon (length: 20)
        public string Link { get; set; } // Link (length: 500)
        public int SeqNo { get; set; } // SeqNo
        public bool? IsFraming { get; set; } // IsFraming
        public int? ParentId { get; set; } // ParentID
        public int ItemType { get; set; } // ItemType
        public string ObjectName { get; set; } // ObjectName (length: 50)
        public MenuMaster()
        {
            SecondaryKey = "";
            ItemType = 0;
        }
    }
}
