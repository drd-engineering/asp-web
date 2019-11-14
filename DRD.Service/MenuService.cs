using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DRD.Models.View;
using System.IO;

namespace DRD.Service
{
    public class MenuService
    {
        public List<Menu> GetMenus(int activeId)
        {
            List<Menu> values = File.ReadAllLines("Menu.csv")
                                           .Skip(1)
                                           .Select(v => Menu.FromCsv(v))
                                           .ToList();
            return values;
        }
    }
}


