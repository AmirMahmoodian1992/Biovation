using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Biovation.Repository.Sql.v2;

namespace Biovation.Data.Queries.Controllers.v2
{
    [Authorize]
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    public class FaceTemplateController : ControllerBase
    {
        private readonly FaceTemplateRepository _faceTemplateRepository;

        public FaceTemplateController(FaceTemplateRepository faceTemplateRepository)
        {
            _faceTemplateRepository = faceTemplateRepository;
        }

        [HttpGet]
        [Authorize]
        public Task<ResultViewModel<PagingResult<FaceTemplate>>> FaceTemplates(string fingerTemplateTypeCode = default, long userId = 0, int index = 0, int pageNumber = default,
            int pageSize = default)
        {
            return Task.Run(() => _faceTemplateRepository.GetFaceTemplates(fingerTemplateTypeCode, userId, index, pageNumber, pageSize));
        }
    }
}