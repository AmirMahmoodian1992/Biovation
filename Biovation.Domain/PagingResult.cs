using Biovation.Domain.DataMappers;
using DataAccessLayerCore.Attributes;
using System.Collections.Generic;

namespace Biovation.Domain
{
  
    public class PagingResult<T>
    {
        [OneToMany]
        public  List<T> Data { get; set; }     
        public int From { get; set; }
        [Id]
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }

    }
}
