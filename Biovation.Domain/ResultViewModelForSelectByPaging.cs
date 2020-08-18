using Biovation.Domain.DataMappers;
using DataAccessLayerCore.Attributes;

namespace Biovation.Domain
{
  
    public class ResultViewModelForSelectByPaging<T>
    {
        [DataMapper(Mapper = typeof(IntToLongMapper))]
        public T Data { get; set; }
        public int From { get; set; }
        public int PageNumber { get; set; }
        public int Size { get; set; }
        public int Count { get; set; }

    }
}
