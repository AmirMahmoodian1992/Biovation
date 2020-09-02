using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Biovation.Data.Queries.Controllers.v2
{

    [Route("biovation/api/queries/v2/[controller]")]

        public class GenericCodeMappingController : Controller
        {

            private readonly GenericCodeMappingRepository _genericCodeMappingRepository;


            public GenericCodeMappingController(GenericCodeMappingRepository genericCodeMappingRepository)
            {
                _genericCodeMappingRepository = genericCodeMappingRepository;
            }

            [HttpGet]
        [Route("FingerTemplateTypes")]
        public Task<ResultViewModel<PagingResult<GenericCodeMapping>>> GetGenericCodeMappings(int categoryId = default, string brandCode = default, int manufactureCode = default, int genericCode = default, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _genericCodeMappingRepository.GetGenericCodeMappings(categoryId,brandCode,manufactureCode,genericCode,pageNumber,pageSize));
        }
    }
}
