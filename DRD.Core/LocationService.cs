using DRD.Domain;
using Geocoding;
using Geocoding.Google;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Based.Core;

namespace DRD.Service
{
    public class LocationService
    {
        public JsonAddressComponent GetAddressComponent(double lat, double lng)
        {
            Geocoding.IGeocoder geocoder = new GoogleGeocoder() { ApiKey = ConfigConstant.API_KEY };
            IEnumerable<Address> addresses = geocoder.ReverseGeocode(lat, lng);
            string sjson = addresses.FirstOrDefault().ToJSON();

            return JsonConvert.DeserializeObject<JsonAddressComponent>(sjson);
        }
    }
}
