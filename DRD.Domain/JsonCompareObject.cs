using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonCompareObject
    {
        public string value1;
        public string value2;
        public decimal number1;
        public decimal number2;

        public JsonCompareObject()
        {
        }
        public JsonCompareObject(string val1, string val2)
        {
            value1 = val1;
            value2 = val2;
        }
        public JsonCompareObject(decimal val1, decimal val2)
        {
            number1 = val1;
            number2 = val2;
        }
    }
}
