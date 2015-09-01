using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmberJS.WebAPI
{
    public class EmberJsonSerializer
    {
        public virtual JToken Deserialize(string json, string root)
        {
            var parsedJson = JObject.Parse(json);
            return parsedJson.SelectToken(root);
        }
    }
}