using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonUserMenu
    {
        public int Id { get; set; } // ID (Primary key)
        public int UserMasterId { get; set; } // UserMasterID
        public int MenuMasterId { get; set; } // MenuMasterID
        public int AccessBit { get; set; } // AccessBit

        public int ChildCount { get; set; } 

        // Foreign keys
        public JsonMenuMaster MenuMaster { get; set; } // FK_UserMenu_MenuMaster
        public JsonUserMaster UserMaster { get; set; } // FK_UserMenu_UserMaster
    }
}
