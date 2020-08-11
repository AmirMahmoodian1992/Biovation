using System.Net;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serialization;

namespace Biovation.CommonClasses.Manager
{
    public class RestRequestJsonSerializer : IRestSerializer
    {
        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public string Serialize(Parameter parameter)
        {
            return JsonConvert.SerializeObject(parameter.Value);
        }

        public T Deserialize<T>(IRestResponse response)
        {
            return (response.IsSuccessful && response.StatusCode == HttpStatusCode.OK) ? JsonConvert.DeserializeObject<T>(response.Content): default;
        }

        public string[] SupportedContentTypes { get; } =
        {
            "application/json", "text/json", "text/x-json", "text/javascript", "*+json"
        };

        public string ContentType { get; set; } = "application/json";

        public DataFormat DataFormat { get; } = DataFormat.Json;
    }
}
