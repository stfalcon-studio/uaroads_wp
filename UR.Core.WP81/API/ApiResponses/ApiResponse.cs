using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UR.Core.WP81.API.ApiResponses
{
    public class ApiResponse
    {
        [JsonProperty("result")]
        public string Result { get; set; }

        public string ErrorCode { get; set; }

        [JsonProperty("message")]
        public string ErrorMessage { get; set; }

        [JsonProperty("errors")]
        public JRaw ErrorsRaw { get; set; }

        public List<ErrorsContainer> Errors { get; private set; }

        protected virtual string GetOkString()
        {
            return "ok";
        }

        //public Dictionary<string, string> Errors { get; set; }

        //public string termsOfService { get; set; }
        //public bool requireFullSSN { get; set; }
        //public bool logUserOut { get; set; }
        //public string jsonString { get; set; }

        //public string trackingeventid { get; set; }

        //[JsonIgnore]
        //[IgnoreDataMember]
        //public Type TargetType { get; set; }

        public virtual bool IsSuccess
        {
            get
            {
                var res = true;

                res = String.Compare(Result, GetOkString(), StringComparison.CurrentCultureIgnoreCase) == 0;

                if (res)
                {
                    //parce raw
                    ParceRaw();

                    if (Errors != null)
                    {
                        if (Errors.Any())
                        {
                            res = false;
                        }
                    }
                }

                return res;
            }
        }

        private void ParceRaw()
        {
            if (ErrorsRaw != null)
            {
                //var first = ErrorsRaw.First;

                //var reader = first.CreateReader();

                //reader.Read();

                Errors = new List<ErrorsContainer>();

                var firstOk = false;

                try
                {
                    var data = ErrorsRaw.ToObject<string[]>();

                    foreach (var err in data)
                    {
                        Errors.Add(new ErrorsContainer()
                        {
                            ErrorCode = "",
                            ErrorMessage = err
                        });
                    }

                    firstOk = true;

                    //r.Value.FieldsByAccount = data;
                }
                catch (Exception)
                {
                }


                try
                {
                    var data = ErrorsRaw.ToObject<Dictionary<string, string>>();

                    foreach (var err in data)
                    {
                        Errors.Add(new ErrorsContainer()
                        {
                            ErrorCode = err.Key,
                            ErrorMessage = err.Value
                        });
                    }

                    firstOk = true;

                    //r.Value.FieldsByAccount = data;
                }
                catch (Exception)
                {
                }
            }
        }

        //public ErrorHandlerResult? ErrorHandlerResult { get; set; }


        //public string CookieToken { get; set; }
        //public string CookieUid { get; set; }
    }

    public class ErrorsContainer
    {
        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}
