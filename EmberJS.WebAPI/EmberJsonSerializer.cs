using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmberJS.WebAPI
{
    public class EmberJsonSerializer
    {
        public virtual JObject Deserialize(string json)
        {
            var rootRemovedJson = this.RemoveRoot(json);
            return JObject.Parse(rootRemovedJson);
        }

        private string RemoveRoot(string json)
        {
            json = json.Trim();
            var bracesChopped = json.Substring(1, json.Length - 2);

            if (bracesChopped.EndsWith("}"))
            {
                return bracesChopped.Substring(bracesChopped.IndexOf(":") + 1);
            }

            return json;
        }
    }
}