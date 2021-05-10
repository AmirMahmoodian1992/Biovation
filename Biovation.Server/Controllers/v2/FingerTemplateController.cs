using Biovation.Domain;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Biovation.Server.Attribute;

namespace Biovation.Server.Controllers.v2
{
    [Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class FingerTemplateController : ControllerBase
    {
        private readonly FingerTemplateService _fingerTemplateService;

        public FingerTemplateController(FingerTemplateService fingerTemplateService)
        {
            _fingerTemplateService = fingerTemplateService;
        }

        [HttpGet]
        [Route("TemplateCount")]
        public async Task<ResultViewModel<PagingResult<UserTemplateCount>>> GetTemplateCount()
        {
            return await _fingerTemplateService.GetTemplateCount(HttpContext.Items["Token"] as string);
        }

        [HttpGet]
        [Route("FingerTemplateTypes")]
        public Task<ResultViewModel<PagingResult<Lookup>>> GetFingerTemplateTypes(string brandId = default)
        {
            return _fingerTemplateService.GetFingerTemplateTypes(brandId, HttpContext.Items["Token"] as string);
        }
    }
}
