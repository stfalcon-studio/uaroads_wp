using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UR.Core.WP81.API.ApiResponses
{
    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }


        //[JsonProperty("result")]
        public string Result { get; set; }

        public string ErrorCode { get; set; }

        //[JsonProperty("message")]
        public string ErrorMessage { get; set; }

        //[JsonProperty("errors")]
        //public JRaw ErrorsRaw { get; set; }

        //public List<ErrorsContainer> Errors { get; private set; }

        protected virtual string GetOkString()
        {
            return "ok";
        }

        public virtual bool IsSuccess
        {
            get
            {
                if (StatusCode == HttpStatusCode.OK) return true;

                return false;
            }
        }
    }

    public class ErrorsContainer
    {
        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}
