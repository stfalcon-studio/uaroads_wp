#define TEST
//#undef TESt

using System;
using System.Net.Http;
using System.Threading.Tasks;
using UaRoadsWpApi.Generic;
using UR.Core.WP81.API.ApiResponses;

namespace UR.Core.WP81.API
{
    public partial class ApiClient
    {
        /// <summary>
        /// register device
        /// </summary>
        /// <param name="userEmail">user userEmail</param>
        /// <param name="deviceOsName">OS name</param>
        /// <param name="deviceName">device name or device model</param>
        /// <param name="deviceOsVersion">OS version</param>
        /// <param name="deviceUid">device UID</param>
        /// <returns></returns>
        public async Task<ApiResponse> Login(string userEmail, string deviceOsName, string deviceName, string deviceOsVersion, string deviceUid)
        {
            var c = new RequestParametersContainer
            {
                { "email", userEmail }, 
                { "os", deviceOsName },
                { "device_name", deviceName }, 
                { "os_version", deviceOsVersion },
                { "uid", deviceUid }
            };
            return await SendRequest<ApiResponse>("register-device", HttpMethod.Post, c);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceUid">device UID</param>
        /// <param name="routeId">route ID, should be unique</param>
        /// <param name="routeComment">route comment</param>
        /// <param name="data">route data</param>
        /// <returns></returns>
        public async Task<ApiResponse> Add(string deviceUid, Guid routeId, string data, string routeComment)
        {
            //comment=formula5&data=H4sIAAAAAAAEAMWSOQ5CMRBDD0P/NfuifxwK9BtAXI+CI3EFInYkVxSQ8ike25Ocjyc2kghWoaCYed4dls2yXT0xMyXC3ggLDWw5dWhIVLCn6Kw5eY0jZiHVPK/3f5GAvEEGsDIxwkECsFGjIYZX5Ex6D59WrVxldQ3f1E7EI2t+ZP+ZAoTFHQI3Doc4GS45vdGHKzy7WB6VRqFq0nHxVknJuYcHDZv3Tt9IvAuYj62hh2/vfuELfVQeEU0DAAA=&routeId=idTest60&uid=___________________DEVICE_ID_

            var c = new RequestParametersContainer
            {
                { "comment", routeComment }, 
                { "data", data },
                { "routeId", routeId.ToString() }, 
                { "uid", deviceUid }
            };

            return await SendRequest<ApiResponse>("add", HttpMethod.Post, c);
        }
    }
}
