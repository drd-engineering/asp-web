using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Service
{
    public class Utilities
    {
        public static long RandomLongGenerator(long minimumValue, long maximumValue)
        {
            Random randomClass = new Random();
            byte[] buf = new byte[8];
            randomClass.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);

            return (Math.Abs(longRand % (maximumValue - minimumValue)) + minimumValue);
        }
    }
}
