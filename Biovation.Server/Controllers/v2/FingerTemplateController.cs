using Biovation.Domain;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Biovation.Server.Controllers.v2
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    [Authorize]
    public class FingerTemplateController : Controller
    {
        private readonly FingerTemplateService _fingerTemplateService;

        public FingerTemplateController(FingerTemplateService fingerTemplateService)
        {
            _fingerTemplateService = fingerTemplateService;
        }

        [HttpGet]
        [Route("TemplateCount")]
        public Task<ResultViewModel<PagingResult<UserTemplateCount>>> GetTemplateCount()
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() => _fingerTemplateService.GetTemplateCount(token));
        }

        [HttpGet]
        [Route("FingerTemplateTypes")]
        public Task<ResultViewModel<PagingResult<Lookup>>> GetFingerTemplateTypes(string brandId = default)
        {
            var token = (string)HttpContext.Items["Token"];
            return Task.Run(() => _fingerTemplateService.GetFingerTemplateTypes(brandId, token));
        }
    }
}
