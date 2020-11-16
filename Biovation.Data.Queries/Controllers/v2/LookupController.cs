using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Biovation.Repository.Sql.v2;

namespace Biovation.Data.Queries.Controllers.v2
{
    [Route("biovation/api/v2/[controller]")]
    public class LookupController : ControllerBase
    {
        private readonly LookupRepository _lookupRepository;


        public LookupController(LookupRepository lookupRepository)
        {
            _lookupRepository = lookupRepository;
        }
        [HttpGet]
        public Task<ResultViewModel<PagingResult<Lookup>>> GetLookups(string code = default, string name = default, int lookupCategoryId = default, string codePrefix = default, int pageNumber = default,
            int pageSize = default)
        {
            return Task.Run(() => _lookupRepository.GetLookups(code, name, lookupCategoryId, codePrefix, pageNumber, pageSize));
        }
    }
}