using Biovation.CommonClasses.DataMappers;
using DataAccessLayer.Attributes;

namespace Biovation.CommonClasses.Models
{
    public class ResultViewModel
    {
        public int Validate { get; set; }
        public string Message { get; set; }
        [DataMapper(Mapper = typeof(IntToLongMapper))]
        public long Code { get; set; }
        public long Id { get; set; }
    }

    public class ResultViewModel<T>
    {
        public int Validate { get; set; }
        public string Message { get; set; }
        [DataMapper(Mapper = typeof(IntToLongMapper))]
        public T Data { get; set; }
        public long Id { get; set; }
        public long Code { get; set; }
    }
}
