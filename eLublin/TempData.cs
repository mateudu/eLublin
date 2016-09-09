using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace eLublin
{
    class TempData
    {
        public static Geoposition Position { get; set; }
        public async static Task<Geoposition> Lokalizuj()
        {
            var loc = new Geolocator {DesiredAccuracyInMeters = 50};
            return await loc.GetGeopositionAsync(new TimeSpan(0, 0, 0, 6), new TimeSpan(0, 0, 0, 6));
        }
    }
}
