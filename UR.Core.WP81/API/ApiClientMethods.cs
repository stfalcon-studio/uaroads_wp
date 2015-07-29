#define TEST
//#undef TESt

using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Eve.Core.WPA81.Helpers;
using UR.Core.WP81.API.ApiResponses;
using UR.Core.WP81.Services;

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
        public async Task<ApiResponse> RegisterDevice(string userEmail, string deviceName, string deviceOsVersion, string deviceUid)
        {

            //+ email=email@here.com
            // &os=Win32NT
            // &device_name=XDeviceEmulator
            // &os_version=8.0.10512.0
            // &uid=8QVDrW2CVKndzQlh3z7DFPdVPyb8rV/tT0BsbI5+m3w=
            // &app_ver=0.0.0.1
            var c = new RequestParametersContainer
            {
                { "email", userEmail },
                { "os", "WP81" },
                { "device_name", deviceName }, //todo promt user about it
                { "os_version", ">=WP8.1" },
                { "app_ver", Package.Current.Id.Version.GetAsString() },
                { "uid", deviceUid }
            };
            return await SendRequest<ApiResponse>("http://uaroads.com/register-device", HttpMethod.Post, c);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceUid">device UID</param>
        /// <param name="routeId">route ID, should be unique</param>
        /// <param name="fileName"></param>
        /// <param name="routeComment">route comment</param>
        /// <param name="data">route data</param>
        /// <returns></returns>
        public async Task<ApiResponse> Add(Guid routeId, string data, string fileName, string routeComment)
        {
            //comment=formula5&data=H4sIAAAAAAAEAMWSOQ5CMRBDD0P/NfuifxwK9BtAXI+CI3EFInYkVxSQ8ike25Ocjyc2kghWoaCYed4dls2yXT0xMyXC3ggLDWw5dWhIVLCn6Kw5eY0jZiHVPK/3f5GAvEEGsDIxwkECsFGjIYZX5Ex6D59WrVxldQ3f1E7EI2t+ZP+ZAoTFHQI3Doc4GS45vdGHKzy7WB6VRqFq0nHxVknJuYcHDZv3Tt9IvAuYj62hh2/vfuELfVQeEU0DAAA=&routeId=idTest60&uid=___________________DEVICE_ID_

            // uid=8QVDrW2CVKndzQlh3z7DFPdVPyb8rV/tT0BsbI5+m3w=
            // &app_ver=0.0.0.1
            // &data=NjM1NzA2OTg0NTEyMjQ1MjE1
            // &comment=1
            // &routeId=1


            var c = new RequestParametersContainer
            {
                { "comment", routeComment }, 
               // { "data", data },
                { "routeId", routeId.ToString() },
                { "app_ver", Package.Current.Id.Version.GetAsString() },
                { "uid",  SettingsService.DeviceId }
            };

            var enc = new AsciiEncoding();

            c.Add("data", fileName, enc.GetBytes(data));

            return await SendRequest<ApiResponse>("http://api.uaroads.com/add", HttpMethod.Post, c);
        }
    }

    /// <summary>
    /// Silverlight doesn't have an ASCII encoder, so here is one:
    /// </summary>
    public class AsciiEncoding : System.Text.Encoding
    {
        public override int GetMaxByteCount(int charCount)
        {
            return charCount;
        }

        public override int GetMaxCharCount(int byteCount)
        {
            return byteCount;
        }

        public override int GetByteCount(char[] chars, int index, int count)
        {
            return count;
        }

        public override string WebName
        {
            get { return "US-ASCII"; }
        }

        public override byte[] GetBytes(char[] chars)
        {
            return base.GetBytes(chars);
        }

        public override int GetCharCount(byte[] bytes)
        {
            return bytes.Length;
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            for (int i = 0; i < charCount; i++)
            {
                bytes[byteIndex + i] = (byte)chars[charIndex + i];
            }
            return charCount;
        }

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            return count;
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            for (int i = 0; i < byteCount; i++)
            {
                chars[charIndex + i] = (char)bytes[byteIndex + i];
            }
            return byteCount;
        }
    }
}
