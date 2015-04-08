using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web;
using Humanizer;

namespace EmberJS.WebAPI
{
    public class EmberJsonMediaTypeFormatter : JsonMediaTypeFormatter
    {
        private int _maxDepth = 256;
        private JsonSerializerSettings _jsonSerializerSettings;
        public EmberJsonMediaTypeFormatter()
        {
            _jsonSerializerSettings = CreateDefaultSerializerSettings();
        }
        public override System.Threading.Tasks.Task WriteToStreamAsync(System.Type type, object value, System.IO.Stream writeStream, System.Net.Http.HttpContent content, System.Net.TransportContext transportContext)
        {
            var root = GetRootFieldName(type, value);
            var obj = new ExpandoObject() as IDictionary<string, Object>;
            obj[root] = value;
            return base.WriteToStreamAsync(type, obj as object, writeStream, content, transportContext);
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            return Task.FromResult(ReadFromStream(type, readStream, content, formatterLogger)); ;
        }

        private object ReadFromStream(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
        {
            var root = GetRootFieldName(type);
            var contentHeaders = content == null ? null : content.Headers;

            // If content length is 0 then return default value for this type
            if (contentHeaders != null && contentHeaders.ContentLength == 0)
                return GetDefaultValueForType(type);

            // Get the character encoding for the content
            var effectiveEncoding = SelectCharacterEncoding(contentHeaders);

            try
            {
                using (var reader = (new StreamReader(readStream, effectiveEncoding)))
                {
                    var json = reader.ReadToEnd();
                    var serializer = new EmberJsonSerializer();
                    var deserialized = serializer.Deserialize(json);
                    return deserialized.ToObject(type);
                }
            }
            catch (Exception e)
            {
                if (formatterLogger == null)
                {
                    throw;
                }
                formatterLogger.LogError(String.Empty, e);
                return GetDefaultValueForType(type);
            }
        }

        private string GetRootFieldName(Type type, dynamic value = null)
        {
            //get element type if array
            var isCollection = false;
            if (value != null && (value is IEnumerable || type.IsArray))
            {
                type = this.GetElementTypeFromCollection(value);
                isCollection = true;
            }

            var attrs = type.CustomAttributes.Where(x => x.AttributeType == typeof(Newtonsoft.Json.JsonObjectAttribute));

            if (attrs.Any())
            {
                var titles = attrs.First().NamedArguments.Where(arg => arg.MemberName == "Title")
                    .Select(arg => arg.TypedValue.Value.ToString()).ToList();

                if (titles.Any())
                {
                    if (isCollection)
                    {
                        return titles.First().Camelize().Pluralize();
                    }
                    return titles.First().Camelize();
                }
            }

            return isCollection ? type.Name.Camelize().Pluralize() : type.Name.Camelize();
        }

        private Type GetElementTypeFromCollection(dynamic collection)
        {
            var type = collection.GetType();
            // For arrays
            var elementType = type.GetElementType();
            if (elementType != null)
            {
                return elementType;
            }

            // For other collections
            var elementTypes = type.GetGenericArguments();
            if (elementTypes.Length > 0)
            {
                return elementTypes[0];
            }

            return null;
        }
    }
}