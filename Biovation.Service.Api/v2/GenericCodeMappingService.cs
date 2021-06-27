using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.Api.v2;

namespace Biovation.Service.Api.v2
{
    public class GenericCodeMappingService
    {
        private readonly GenericCodeMappingRepository _genericCodeMappingRepository;

        public GenericCodeMappingService(GenericCodeMappingRepository genericCodeMappingRepository)
        {
            _genericCodeMappingRepository = genericCodeMappingRepository;
        }

        public Task<ResultViewModel<PagingResult<GenericCodeMapping>>> GetGenericCodeMappings(int categoryId = default,
            string brandCode = default, int manufactureCode = default, int genericCode = default,
            int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _genericCodeMappingRepository.GetGenericCodeMappings(categoryId, brandCode, manufactureCode, genericCode,pageNumber,pageSize));
        }
    }
}
