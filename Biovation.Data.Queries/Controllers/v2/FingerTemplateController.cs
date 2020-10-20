using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Biovation.Repository.Sql.v2;


namespace Biovation.Data.Queries.Controllers.v2
{
    //[Route("Biovation/Api/{controller}/{action}", Name = "Device")]
    //[Route("biovation/api/v{version:apiVersion}/[controller]")]
    [Route("biovation/api/v2/[controller]")]

    public class FingerTemplateController : Controller
    {

        private readonly FingerTemplateRepository _fingerTemplateRepository;


        public FingerTemplateController(FingerTemplateRepository fingerTemplateRepository)
        {
            _fingerTemplateRepository = fingerTemplateRepository;
        }



        [HttpGet]
        [Route("TemplateCount")]
        [Authorize]

        public Task<ResultViewModel<PagingResult<UserTemplateCount>>> GetTemplateCount()
        {
            return Task.Run(() => _fingerTemplateRepository.GetFingerTemplatesCount());
        }


        [HttpGet]
        [Authorize]

        public Task<ResultViewModel<PagingResult<FingerTemplate>>> FingerTemplates(int userId, int templateIndex, Lookup fingerTemplateType, int from = 0, int size = 0, int pageNumber = default, int pageSize = default)
        {
            return Task.Run(() => _fingerTemplateRepository.FingerTemplates(userId, templateIndex, fingerTemplateType, from, size, pageNumber, pageSize));
        }

        [HttpGet]
        [Route("FingerTemplateTypes/brandId")]
        [Authorize]

        public Task<ResultViewModel<PagingResult<Lookup>>> GetFingerTemplateTypes(string brandId, int pageNumber = default,
        int pageSize = default)
        {
            return Task.Run(() => _fingerTemplateRepository.GetFingerTemplateTypes(brandId,pageNumber,pageSize));
        }

        [HttpGet]
        [Route("FingerTemplatesCountByFingerTemplateType")]
        [Authorize]

        public Task<ResultViewModel<int>> GetFingerTemplatesCountByFingerTemplateType(Lookup fingerTemplateType)
        {
            return Task.Run(() => _fingerTemplateRepository.GetFingerTemplatesCountByFingerTemplateType(fingerTemplateType));
        }


        


    }
}
