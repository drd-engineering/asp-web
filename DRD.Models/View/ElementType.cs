using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Models.View
{
    public class ElementType
    {
        public long Id; // ElementType from id
        public string Code;
        public string Description; // ??

        public ElementType() { }

        public static ElementType fromCsv(string csvName)
        {
            /// TODO: implement ChildCount and filter neccessary standard menu
            string[] values = csvName.Split(',');
            ElementType elementTypeItem = new ElementType();
            elementTypeItem.Id = Convert.ToInt32(values[0]);
            elementTypeItem.Code = Convert.ToString(values[1]);
            elementTypeItem.Description = Convert.ToString(values[2]);
            return elementTypeItem;
        }
    }
}
