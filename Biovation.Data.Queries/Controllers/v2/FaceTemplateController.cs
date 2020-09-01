using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Biovation.Data.Queries.Controllers.v2
{
    //[Route("Biovation/Api/{controller}/{action}", Name = "Device")]
    //[Route("biovation/api/v{version:apiVersion}/[controller]")]
    [Route("biovation/api/queries/v2/[controller]")]

    public class FaceTemplateController : Controller
    {

        private readonly FaceTemplateRepository _faceTemplateRepository;


        public FaceTemplateController(FaceTemplateRepository faceTemplateRepository)
        {
            _faceTemplateRepository = faceTemplateRepository;
        }

        [HttpGet]
        public Task<ResultViewModel<PagingResult<FaceTemplate>>> FaceTemplates(string fingerTemplateTypeCode = default, long userId = 0, int index = 0, int pageNumber = default,
            int pageSize = default)
        {
            return Task.Run(() => _faceTemplateRepository.GetFaceTemplates(fingerTemplateTypeCode, userId, index, pageNumber, pageSize));
        }

    }
}