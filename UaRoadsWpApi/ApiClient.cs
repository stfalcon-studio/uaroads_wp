#define SANDBOX_ENV
#undef SANDBOX_ENV

#define LOG_REQ
//#undef LOG_REQ
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UaRoadsWpApi.Generic;

namespace UaRoadsWpApi
{
    public partial class ApiClient
    {
        private HttpClient _httpClient;
        private bool _isProductionEnvironment;

        private const string EndpointFrontendBackend =
#if SANDBOX_ENV
            "http://backend-uaroads-com.dev.stfalcon.com/";
#else
 "http://uaroads.com/";
#endif

        private const string EndpointApi =
#if SANDBOX_ENV
            "http://uaroads-com.dev.stfalcon.com/";
#else
 "http://api.uaroads.com/";
#endif


        public static Action<string> OnErrorAction;

        public ApiClient()
        {
            _isProductionEnvironment = false;

            _httpClient =
            new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            });
        }


        private string GetUrl(EServerType endpointType)
        {
            return endpointType == EServerType.Api ? EndpointApi : EndpointFrontendBackend;
        }

        async Task<T> SendRequest<T>(EServerType endpointType, string url, HttpMethod httpMethod, RequestParametersContainer container = null) where T : ApiResponse, new()
        {
            var message = new HttpRequestMessage(httpMethod, Path.Combine(GetUrl(endpointType), url));

            message.Content = ConvertToHttpContent(container);

            message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

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


#if DEBUG
#if LOG_REQ
                Debug.WriteLine("RESPONSE:\r\n{0}\r\n", s);
#endif
#endif


                if (String.IsNullOrEmpty(s) & response.StatusCode != HttpStatusCode.OK)
                {
                    return new T()
                    {
                        success = false,
                        message = ApiResponseProcessor.GetErrorMessage(ErrorCodes.NETWORK_ERROR)
                    };
                }

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return new T()
                    {
                        success = false,
                        message = s
                    };
                }

#if LOG_REQ
                Debug.WriteLine("RESPONSE:\r\n{0}\r\n", s);
#endif

                try
                {
                    return JsonConvert.DeserializeObject<T>(s);
                }
                catch (Exception)
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                        return new T()
                        {
                            success = true
                        };

                    return new T()
                    {
                        message = ApiResponseProcessor.GetErrorMessage(ErrorCodes.NETWORK_ERROR)
                    };
                }


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


    internal enum EServerType
    {
        Backend, Api
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
