using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UaRoadsWpApi.Models
{
    //data - string (заархівована строка вигляду: time;pit;lat;lng;type#time;pit;lat;lng;type#..... , де:
    //     time - float (час у мілісекундах)
    //     pit - float (сила струсу від 0 до 1, 0 - checkpoint)
    //     lat, lng - float (широта, довгота точки)
    //     type - string відмітка яка це саме точка (old, new, cp)
    //     )
    public struct RoutePoint
    {
        /// <summary>
        /// unix timestamp, but in ms (*1000, DateTime.Ticks)
        /// Timestamp in seconds  : 1400339028
        /// Timestamp in milliseconds : 1400339028000        /// 
        /// </summary>
        [JsonProperty("time")]
        public long time { get; set; }

        [JsonProperty("pit")]
        public float pit { get; set; }

        [JsonProperty("lat")]
        public float lat { get; set; }

        [JsonProperty("lng")]
        public float lng { get; set; }

        [JsonProperty("type")]
        public ERoutePointType type { get; set; }
    }
}
