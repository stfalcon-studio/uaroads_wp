
//"";
#define LOG_REQ
//#undef LOG_REQ


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UR.Core.WP81.API.ApiResponses;
using UR.Core.WP81.Services;

namespace UR.Core.WP81.API
{
    public partial class ApiClient
    {
        private HttpClient _httpClient;

        private HttpClientHandler _httpClientHandler;

        private static CookieContainer _cookieContainer;

        private ApiClient()
        {
            //var cc = new CookieContainer();
            _httpClientHandler = new HttpClientHandler()
            {
                UseCookies = true,
                //CookieContainer = cc,
                AllowAutoRedirect = true,
            };

            //AddCookies(_httpClientHandler);

            _httpClient = new HttpClient(_httpClientHandler);

            //AddHeaders(_httpClient);
        }

        public static ApiClient Create()
        {
            return new ApiClient();
        }


        private static bool _logger = false;
        public static void SetLogger(bool enabled)
        {
            _logger = enabled;
        }

        protected async Task<T> SendRequest<T>(string url, HttpMethod httpMethod, RequestParametersContainer container = null, bool includeJsonString = false) where T : ApiResponse, new()
        {
            //if (url.Contains("auth"))
            //{

            //}


            if (container == null)
            {
                container = new RequestParametersContainer();
            }

            //var sign = GetSign(container);

            //container.Add("sign", sign);



            //var ub = new Uri(new Uri(AppConstants.BaseApiUrl), url + "?" + query);

            var ub = new Uri(url);

            //if (httpMethod == HttpMethod.Get)
            //{
            //    ub = AttachParameters(ub, container);
            //}


            var message = new HttpRequestMessage(httpMethod, ub)
            {
                //Content = new StringContent(query),

            };

            //message.Content 

            AddHeaders(message);

            if (httpMethod == HttpMethod.Post)
            {
                var query = await ConvertToHttpContent(container).ReadAsStringAsync();

                query = WebUtility.UrlDecode(query);

                message.Content = new StringContent(query);
                message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            }


            try
            {
                //throw new Exception("test");
#if DEBUG
#if LOG_REQ

                if (_logger)
                {
                    Debug.WriteLine("\r\nHEADERS:");

                    foreach (var header in _httpClient.DefaultRequestHeaders)
                    {
                        Debug.WriteLine("{0}:{1}", header.Key, header.Value.Aggregate((y, u) => y + "," + u));
                    }

                    var str = "";
                    if (message.Content != null)
                    {
                        str = await message.Content.ReadAsStringAsync();
                    }
                    Debug.WriteLine("REQUEST:\t{0}\r\n{1}\r\n", message.RequestUri, str);
                }
#endif
#endif



                //var param = new Dictionary<string, string>();

                //if (container != null)
                //{
                //    foreach (var k in container)
                //    {
                //        param.Add(k.Key, k.Value);
                //    }
                //}

                //string s = await Get(url, httpMethod, container == null ? null : param);

                var response = await _httpClient.SendAsync(message);
                var s = await response.Content.ReadAsStringAsync();


                // _//httpClientHandler



#if DEBUG
#if LOG_REQ
                if (_logger)
                {
                    Debug.WriteLine("RESPONSE: \r\n{0}\r\n", s);
                }
#endif
#endif

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Debug.WriteLine("RESPONSE ERROR CODE:\t{0}\r\n{1}\r\n", response.StatusCode,
                        response.ReasonPhrase);
                }


                if (string.IsNullOrEmpty(s))//s
                {
                    return new T()
                    {
                        ErrorCode = "NETWORK_ERROR",
                        ErrorMessage = response.StatusCode + response.ReasonPhrase
                    };
                }

                var content = await response.Content.ReadAsStringAsync();

                return new T()
                {
                    StatusCode = response.StatusCode,
                    ErrorMessage = content
                };


                //#if LOG_REQ
                //                Debug.WriteLine("RESPONSE:\r\r\n{0}\r\r\n", s);
                //#endif
                //check for coocies

                //if (includeJsonString)
                //{
                //    var res = JsonConvert.DeserializeObject<T>(s);
                //    //res.jsonString = s;

                //    return res;
                //}

                //var apiresponse = JsonConvert.DeserializeObject<T>(s);

                //return apiresponse;
            }
            catch (Exception ex)
            {
#if LOG_REQ
                Debug.WriteLine("ERROR:\r\r\n{0}\r\r\n", ex.Message);
#endif

                return new T()
                {
                    ErrorCode = "NETWORK_ERROR",
                    ErrorMessage = ex.Message
                };
            }
            finally
            {
                _httpClient.Dispose();
            }
        }


        public static Uri AttachParameters(Uri uri, RequestParametersContainer parameters)
        {
            var stringBuilder = new StringBuilder();
            string str = "?";
            for (int index = 0; index < parameters.Count; ++index)
            {
                stringBuilder.Append(str + parameters[index].Key + "=" + parameters[index].Value);
                str = "&";
            }
            return new Uri(uri + stringBuilder.ToString());
        }



        private HttpContent ConvertToHttpContent(RequestParametersContainer container)
        {
            if (container == null) return null;

            if (!String.IsNullOrEmpty(container.JsonStringData))
            {
                if (String.IsNullOrEmpty(container.PropertyName))
                {
                    return new StringContent(container.JsonStringData);
                }
                else
                {
                    var json = new JObject();
                    json.Add(container.PropertyName, container.JsonStringData);

                    return new StringContent(json.ToString());
                }
            }

            return new FormUrlEncodedContent((IEnumerable<KeyValuePair<string, string>>)container);
        }




        ////public async Task<string> Get(string command, HttpMethod method, Dictionary<string, string> dictionary = null, string deviceId = "11")
        ////{
        ////    var cc = new CookieContainer();
        ////    var ch = new HttpClientHandler()
        ////    {
        ////        UseCookies = true,
        ////        CookieContainer = cc,
        ////        AllowAutoRedirect = true,

        ////        //sUseDefaultCredentials = false
        ////    };

        ////    //_httpClientHandler.CookieContainer.Add(new Uri(AppConstants.BaseApiUrl), new Cookie("name", "value", "/"));

        ////    if (command.Contains("auth"))
        ////    {

        ////    }

        ////    using (var client = new HttpClient(ch) { BaseAddress = new Uri(AppConstants.BaseApiUrl) })
        ////    {
        ////        AddHeaders(client);

        ////        var dict = new Dictionary<string, string>();

        ////        if (dictionary != null)
        ////        {
        ////            dict = dict.Union(dictionary).ToDictionary(k => k.Key, v => v.Value);
        ////        }

        ////        var sign = GetSign(dict);

        ////        var request = command + "?"
        ////             + string.Join("&", dict.Select(p => p.Key + '=' + p.Value).ToArray())
        ////             + "&sign=" + sign;

        ////        using (var message = new HttpRequestMessage(method, request))
        ////        {
        ////            using (var response = await client.SendAsync(message))
        ////            {
        ////                var responseStr = await response.Content.ReadAsStringAsync();

        ////                Debug.WriteLine("RESPONSE:\r\r\n{0}\r\r\n", responseStr);

        ////                var uri = new Uri(AppConstants.BaseApiUrl);

        ////                IEnumerable<Cookie> responseCookies = ch.CookieContainer.GetCookies(uri).Cast<Cookie>();
        ////                foreach (Cookie cookie in responseCookies)
        ////                    Debug.WriteLine(cookie.Name + ": " + cookie.Value);

        ////                IEnumerable<string> cookies;

        ////                if (response.Headers.TryGetValues("Set-Cookie", out cookies))
        ////                {
        ////                    //foreach (var c in cookies)
        ////                    //{
        ////                    //    cookieContainer.SetCookies(pageUri, c);
        ////                    //}
        ////                }





        ////                return responseStr;
        ////            }
        ////        }
        ////    }
        ////}


        //private string GetSign(IEnumerable<KeyValuePair<string, string>> parameters)
        //{
        //    var s = string.Join("", parameters.Select(x => x.Key + "=" + x.Value)) + AppConstants.PrivateKey;

        //    var md5 = CalculateMd5Hash(s);

        //    return md5 + AppConstants.PublicKey;
        //}

        //private static string CalculateMd5Hash(string str)
        //{
        //    var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
        //    IBuffer buff = CryptographicBuffer.ConvertStringToBinary(str, BinaryStringEncoding.Utf8);
        //    var hashed = alg.HashData(buff);
        //    var res = CryptographicBuffer.EncodeToHexString(hashed);
        //    return res;
        //}

        private void AddHeaders(HttpRequestMessage httpClient)
        {
            //httpClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            //httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");

            //httpClient.DefaultRequestHeaders.Add("Pragma", "no-cache");
        }

        //private void AddCookies(HttpClientHandler handler)
        //{
        //    if (StateService.Instance.UserToken != null)
        //    {
        //        if (_cookieContainer == null)
        //        {
        //            _cookieContainer = new CookieContainer();
        //        }

        //        _cookieContainer.Add(new Uri(AppConstants.BaseApiUrl), new Cookie("token", StateService.Instance.UserToken.Token));
        //        _cookieContainer.Add(new Uri(AppConstants.BaseApiUrl), new Cookie("uid", StateService.Instance.UserToken.Uid));

        //        handler.CookieContainer = _cookieContainer;
        //    }

        //}
    }
}
