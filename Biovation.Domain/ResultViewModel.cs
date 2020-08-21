using System.Net.Http;
using Biovation.Domain.DataMappers;
using DataAccessLayerCore.Attributes;
using Microsoft.AspNetCore.Http;

namespace Biovation.Domain
{
    public class ResultViewModel
    {
        public bool Validate { get; set; }
        public string Message { get; set; }
        [DataMapper(Mapper = typeof(IntToLongMapper))]
        public long Code { get; set; }
        public long Id { get; set; }
        //public HttpResponse StatusCode { get; set; }
    }

    public class ResultViewModel<T>
    {
        public bool Validate { get; set; }
        public string Message { get; set; } 
        [DataMapper(Mapper = typeof(IntToLongMapper))]
        public T Data { get; set; }
        public long Id { get; set; }
        public long Code { get; set; }
        //public HttpResponse StatusCode { get; set; }
    }
}
