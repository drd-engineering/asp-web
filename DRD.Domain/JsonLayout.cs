using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonLayout
    {
        public List<JsonMenu> menus {get; set;}
        public List<JsonMenu> dbmenus { get; set; }
        public List<string> objItems { get; set; }
        public DtoMemberLogin user { get; set; }
        public object obj { get; set; }
        public int activeId { get; set; }
        public string key { get; set; }
        public string url { get; set; }
        public int dataId { get; set; } // cover godeg program
        public bool isLayout { get; set; } 

        public JsonLayout()
        {
            isLayout = true; 
        }
    }
}
