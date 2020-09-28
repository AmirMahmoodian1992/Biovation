using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Service.Api.v2;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Server.Controllers.v2
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]
    public class FingerTemplateController : Controller
    {
        private readonly FingerTemplateService _fingerTemplateService;

        public FingerTemplateController(FingerTemplateService fingerTemplateService)
        {
            _fingerTemplateService = fingerTemplateService;
        }


        [HttpGet]
        [Route("{userId}")]
        public Task<ResultViewModel<PagingResult<FingerTemplate>>> GetFingerTemplateByUserId(Lookup fingerTemplateType,int userId = default, int templateIndex = default, int from = 0, int size = 0, int pageNumber = default,
        int pageSize = default)
        {
            return Task.Run( () => _fingerTemplateService.FingerTemplates(userId,templateIndex,fingerTemplateType,from,size,pageNumber,pageSize));
        }

        //[HttpPost]
        //[Route("{userId}")]
        //public Task<IActionResult> AddUserFingerTemplate([FromBody]FingerTemplate fingerTemplate = default)
        //{
        //    //TODO: Add fingerTempalte
        //}

        [HttpPatch]
        public Task<ResultViewModel> ModifyUserFingerTemplate([FromBody]FingerTemplate fingerTemplate =default)
        {
            return Task.Run(() => _fingerTemplateService.ModifyFingerTemplate(fingerTemplate));
        }

        [HttpDelete]
        [Route("{userId}")]
        public Task<ResultViewModel> DeleteFingerTemplateByUserId(int userId = default, int templateIndex = default)
        {
            return Task.Run(() => _fingerTemplateService.DeleteFingerTemplate(userId,templateIndex));
        }

        [HttpGet]
        [Route("TemplateCount")]
        public Task<ResultViewModel<PagingResult<UserTemplateCount>>> GetTemplateCount()
        {
            return Task.Run( () =>  _fingerTemplateService.GetTemplateCount());
        }

        [HttpGet]
        [Route("FingerTemplateTypes")]
        public Task<ResultViewModel<PagingResult<Lookup>>> GetFingerTemplateTypes(string brandId = default)
        {
            return Task.Run(() => _fingerTemplateService.GetFingerTemplateTypes(brandId));
        }
    }
}
