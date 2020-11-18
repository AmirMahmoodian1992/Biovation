using Biovation.Domain;
using Biovation.Repository.Sql.v2;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


namespace Biovation.Data.Commands.Controllers.v2
{
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    //[ApiVersion("1.0")]
    public class FaceTemplateController : ControllerBase
    {
        private readonly FaceTemplateRepository _faceTemplateRepository;

        public FaceTemplateController(FaceTemplateRepository faceTemplateRepository)
        {
            _faceTemplateRepository = faceTemplateRepository;
        }

        [HttpPut]
        [Authorize]
        public Task<ResultViewModel> ModifyFaceTemplate([FromBody] FaceTemplate faceTemplate)
        {
            return Task.Run(() => _faceTemplateRepository.ModifyFaceTemplate(faceTemplate));
        }

        [HttpDelete]
        [Authorize]
        public Task<ResultViewModel> DeleteFaceTemplate(long userId = 0, int index = 0)
        {
            return Task.Run(() => _faceTemplateRepository.DeleteFaceTemplate(userId, index));
        }
    }
}
