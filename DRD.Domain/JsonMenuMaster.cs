using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonMenuMaster
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

        //// Reverse navigation
        //public virtual System.Collections.Generic.ICollection<GroupMenu> GroupMenus { get; set; } // GroupMenu.FK_GroupMenu_MenuMaster
        //public virtual System.Collections.Generic.ICollection<UserMenu> UserMenus { get; set; } // UserMenu.FK_UserMenu_MenuMaster

        public JsonMenuMaster()
        {
            SecondaryKey = "";
            ItemType = 0;
            //GroupMenus = new System.Collections.Generic.List<GroupMenu>();
            //UserMenus = new System.Collections.Generic.List<UserMenu>();
        }
    }
}
