using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Biovation.Repository.Sql.v2;

namespace Biovation.Data.Queries.Controllers.v2
{

    [Route("biovation/api/v2/[controller]")]

    public class GenericCodeMappingController : ControllerBase
    {

        private readonly GenericCodeMappingRepository _genericCodeMappingRepository;


        public GenericCodeMappingController(GenericCodeMappingRepository genericCodeMappingRepository)
        {
            _genericCodeMappingRepository = genericCodeMappingRepository;
        }

        [HttpGet]
        [Route("GenericCodeMappings")]
        public Task<ResultViewModel<PagingResult<GenericCodeMapping>>> GetGenericCodeMappings(int categoryId = default, string brandCode = default, int manufactureCode = default, int genericCode = default, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _genericCodeMappingRepository.GetGenericCodeMappings(categoryId, brandCode, manufactureCode, genericCode, pageNumber, pageSize));
        }
    }
}
