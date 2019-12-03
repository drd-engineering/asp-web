using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.View
{
    public class Symbol
    {
        public int Id { get; set; } // Id (Primary key)
        public string Code { get; set; } // Code (length: 50)
        public string Name { get; set; } // Name (length: 100)
        public string Description { get; set; } // Descr
        public string Image { get; set; } // Image (length: 100)
        public int SymbolType { get; set; } // SymbolType
        public string UserId { get; set; } // UserId (length: 50)
        public System.DateTime DateCreated { get; set; } // DateCreated
        public System.DateTime? DateUpdated { get; set; } // DateUpdated

        public Symbol() { }

        public static Symbol FromCsv(string csvName)
        {
            /// TODO: implement ChildCount and filter neccessary standard menu
            string[] values = csvName.Split(',');
            Symbol symbolItem = new Symbol();
            symbolItem.Id = Convert.ToInt32(values[0]);
            symbolItem.Code = Convert.ToString(values[1]);
            symbolItem.Name = Convert.ToString(values[2]);
            symbolItem.Description = Convert.ToString(values[3]);
            symbolItem.Image = Convert.ToString(values[6]);
            symbolItem.SymbolType = Convert.ToInt32(values[7]);
            symbolItem.UserId = Convert.ToString(values[9]);
            return symbolItem;
        }
    }
}
