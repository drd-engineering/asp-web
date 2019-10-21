using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonMenu
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

        public JsonMenu(string Code, string SecondaryKey, string Name, string UrlPage, int ChildCount, string ParentCode, string Icon, string ObjectName, int ItemType, bool IsActive)
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
        public JsonMenu()
        {
        }
    }
}
