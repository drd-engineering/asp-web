using DRD.Models.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.API
{
    public class Layout
    {
        public List<Menu> menus { get; set; }
        public List<Menu> dbmenus { get; set; }
        public List<string> objItems { get; set; }
        public UserSession user { get; set; }
        public object obj { get; set; }
        public int activeId { get; set; }
        public string key { get; set; }
        public string url { get; set; }
        public int dataId { get; set; } // cover godeg program
        public bool isLayout { get; set; }

        public Layout()
        {
            isLayout = true;
        }
    }
}   
