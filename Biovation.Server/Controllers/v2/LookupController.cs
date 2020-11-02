using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Server.Controllers.v2
{
    [ApiVersion("2.0")]
    [Route("biovation/api/v2/[controller]")]
    //[Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class LookupController : Controller
    {
        private readonly LookupService _lookupService;

        public LookupController(LookupService lookupService)
        {
            _lookupService = lookupService;
        }

        [HttpGet]
        [AllowAnonymous]
        public Task<ResultViewModel<PagingResult<Lookup>>> GetLookups(string code = default, string name = default, int lookupCategoryId = default, string codePrefix = default, int pageNumber = default,
            int pageSize = default)
        {
            return Task.Run( () => _lookupService.GetLookups(code,name,lookupCategoryId,codePrefix,pageNumber,pageSize));
        }
    }
}