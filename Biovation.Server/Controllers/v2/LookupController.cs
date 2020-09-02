using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Service.API.v2;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Server.Controllers.v2
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class LookupController : Controller
    {

        private readonly LookupService _lookupService;

        public LookupController(LookupService lookupService)
        {
            _lookupService = lookupService;
        }

        [HttpGet]
        public Task<ResultViewModel<PagingResult<Lookup>>> GetLookups(string code = default, string name = default, int lookupCategoryId = default, string codePrefix = default, int pageNumber = default,
            int pageSize = default)
        {
            return Task.Run(async () => { return _lookupService.GetLookups(code,name,lookupCategoryId,codePrefix,pageNumber,pageSize); });
        }
    }
}