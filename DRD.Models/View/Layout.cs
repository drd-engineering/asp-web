using DRD.Models.Custom;
using System.Collections.Generic;


namespace DRD.Models.View
{
    public class Layout
    {
        public List<Menu> Menus { get; set; }
        public List<Menu> Dbmenus { get; set; }
        public List<string> ObjectItems { get; set; }
        public UserSession User { get; set; }
        public object Object { get; set; }
        public string Key { get; set; }
        public string Url { get; set; }
        public int DataId { get; set; } // cover godeg program
        public bool IsLayout { get; set; }
        public ErrorInfo ErrorInfo { get; set; }
        public Layout()
        {
            IsLayout = true;
        }
    }
}
