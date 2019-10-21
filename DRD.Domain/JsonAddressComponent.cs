using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRD.Domain
{
    public class JsonAddressComponent
    {
        public string Type { get; set; }
        public string LocationType { get; set; }
        public List<Component> Components { get; set; }
        public bool IsPartialMatch { get; set; }
        public CViewport Viewport { get; set; }
        public string PlaceId { get; set; }
        public string FormattedAddress { get; set; }
        public CCoordinates Coordinates { get; set; }
        public string Provider { get; set; }

        public class Component
        {
            public List<string> Types { get; set; }
            public string LongName { get; set; }
            public string ShortName { get; set; }
        }

        public class Northeast
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class Southwest
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class CViewport
        {
            public Northeast Northeast { get; set; }
            public Southwest Southwest { get; set; }
        }

        public class CCoordinates
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }


        public string PointLocation()
        {
            return FormattedAddress;
        }
        public string CountryCode()
        {
            return getByType("Country", true);
        }
        public string CountryName()
        {
            return getByType("Country");
        }
        public string AdminArea()
        {
            return getByType("AdministrativeAreaLevel1");
        }
        public string SubAdminArea()
        {
            return getByType("AdministrativeAreaLevel2");
        }
        public string Locality()
        {
            return getByType("AdministrativeAreaLevel3");
        }
        public string SubLocality()
        {
            return getByType("Unknown");
        }
        public string Thoroughfare()
        {
            return getByType("Route");
        }
        public string SubThoroughfare()
        {
            return getByType("StreetNumber");
        }
        public string PostalCode()
        {
            return getByType("PostalCode");
        }

        private string getByType(string type, bool isShort = false)
        {
            string data = "";
            foreach (Component c in Components)
            {
                foreach (string t in c.Types)
                {
                    if (t.Equals(type))
                    {
                        if (isShort)
                            data = c.ShortName;
                        else
                            data = c.LongName;

                        return data;
                    }
                }
            }
            return data;
        }

    }
}
