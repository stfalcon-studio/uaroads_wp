using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UR.Core.WP81.API
{
    public class RequestParametersContainer : List<KeyValuePair<string, string>>
    {
        private Dictionary<string, KeyValuePair<string, byte[]>> _binaryData =
            new Dictionary<string, KeyValuePair<string, byte[]>>();

        public Dictionary<string, KeyValuePair<string, byte[]>> BinaryData
        {
            get { return _binaryData; }
        }

        public string PropertyName { get; set; }

        public string JsonStringData { get; set; }

        public JObject JsonData { get; set; }

        public JsonSerializerSettings JsonSerializerSettings { get; set; }

        public void Add(object key, IEnumerable<string> value)
        {
            foreach (var v in value)
            {
                Add(key, v);
            }
        }

        public void Add(object key, IEnumerable<int> value)
        {
            Add(key, value.Select(x => x.ToString()));
        }

        public void Add<T>(object key, Nullable<T> value) where T : struct
        {
            if (value.HasValue)
            {
                Add(key, value.Value);
            }
        }

        public void Add(object key, object value, bool addAnyWay = false)
        {
            string v = "";
            if (value != null || addAnyWay)
            {
                if (value is bool)
                {
                    v = ((bool)value) ? "true" : "false";
                }
                else
                {
                    v = value.ToString();
                }
                this.Add(new KeyValuePair<string, string>(key.ToString(), v));
            }
        }

        public void Add(string key, string fileName, byte[] data)
        {
            _binaryData.Add(key.ToString(), new KeyValuePair<string, byte[]>(fileName, data));
        }


        public bool HasBinaryContents()
        {
            return _binaryData.Any();
        }

        public void Merge(RequestParametersContainer pairs)
        {
            foreach (KeyValuePair<string, string> keyValuePair in pairs)
            {
                Add(keyValuePair);
            }
        }

        public string ToStringData()
        {
            return string.Join("&", this.Select(x => string.Format("{0}={1}", x.Key, x.Value)));
        }
    }
}
