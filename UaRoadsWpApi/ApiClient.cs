#define LOG_REQ
//#undef LOG_REQ
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UaRoadsWpApi.Generic;

namespace UaRoadsWpApi
{
    public partial class ApiClient
    {
        private HttpClient _httpClient;
        private bool _isProductionEnvironment;
        private const string ProductionEndpoint = "http://api.uaroads.com/ ";
        private const string SandboxEndpoint = "http://uaroads-com.dev.stfalcon.com/";
        ///http://uaroads-com.dev.stfalcon.com

        public static Action<string> OnErrorAction;

        private string BaseUrl
        {
            get { return _isProductionEnvironment ? ProductionEndpoint : SandboxEndpoint; }
        }

        public ApiClient()
        {
            _isProductionEnvironment = false;

            _httpClient =
            new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });
        }

        async Task<T> SendRequest<T>(string url, HttpMethod httpMethod, RequestParametersContainer container = null) where T : ApiResponse, new()
        {
            var message = new HttpRequestMessage(httpMethod, BaseUrl + url);
            message.Content = ConvertToHttpContent(container);

            try
            {
#if DEBUG
#if LOG_REQ
                var str = "";
                if (message.Content != null)
                {
                    var buf = await message.Content.ReadAsByteArrayAsync();
                    var enc = new System.Text.UTF8Encoding();
                    str = enc.GetString(buf, 0, buf.Length);
                }
                Debug.WriteLine("REQUEST:\t{0}\r\n{1}\r\n", message.RequestUri.ToString(), str);
#endif
#endif

                var response = await _httpClient.SendAsync(message);
                var s = await response.Content.ReadAsStringAsync();

                if (String.IsNullOrEmpty(s))
                {
                    return new T()
                    {
                        message = ApiResponseProcessor.GetErrorMessage(ErrorCodes.NETWORK_ERROR)
                    };
                }


#if LOG_REQ
                Debug.WriteLine("RESPONSE:\r\n{0}\r\n", s);
#endif



                return JsonConvert.DeserializeObject<T>(s);
            }
            catch (Exception ex)
            {
#if LOG_REQ
                Debug.WriteLine("ERROR:\r\n{0}\r\n", ex.Message);
#endif

                return new T()
                {
                    message = ApiResponseProcessor.GetErrorMessage(ErrorCodes.NETWORK_ERROR)
                };
            }
            finally
            {
                _httpClient.Dispose();
            }
        }

        HttpContent ConvertToHttpContent(RequestParametersContainer container)
        {
            if (container == null) return null;
            return new FormUrlEncodedContent(container);
        }

    }

    public class ErrorCodes
    {
        public const string NETWORK_ERROR = "NETWORK_ERROR";
        public const string SERVICE_ERROR = "SERVICE_ERROR";
    }

    public class ApiResponse
    {
        /// <summary>
        /// response status
        /// </summary>
        public bool success { get; set; }

        /// <summary>
        /// error message
        /// </summary>
        public string message { get; set; }

        public bool IsSuccess
        {
            get { return success; }
        }
    }
}
