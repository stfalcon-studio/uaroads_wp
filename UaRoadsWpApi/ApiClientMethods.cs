#define TEST
//#undef TESt

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UaRoadsWpApi.Generic;
using UaRoadsWpApi.Models;

namespace UaRoadsWpApi
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
            var uri = _isProductionEnvironment ? "http://uaroads.com" : "http://backend-uaroads-com.dev.stfalcon.com";
            return await SendRequest<ApiResponse>(uri + "/register-deviсe", HttpMethod.Post, c);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceUid">device UID</param>
        /// <param name="routeId">route ID, should be unique</param>
        /// <param name="routeComment">route comment</param>
        /// <param name="routeData">route data</param>
        /// <returns></returns>
        public async Task<ApiResponse> Add(string deviceUid, Guid routeId, List<RoutePoint> routeComment, string routeData)
        {
            //uid - string (IMEI девайсу)
            //routeId - string (Id роуту, який генерується на стороні двайсу, має бути унікальним)
            //comment - string (коментар до маршруту)
            //data - string (заархівована строка вигляду: time;pit;lat;lng;type#time;pit;lat;lng;type#..... , де:

            return await SendRequest<ApiResponse>("add", HttpMethod.Post);
        }
    }
}
