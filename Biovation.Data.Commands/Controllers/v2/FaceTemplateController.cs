using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Biovation.Repository.Sql.v2;


namespace Biovation.Data.Commands.Controllers.v2
{

    [Route("biovation/api/commands/v2/[controller]")]
    //[ApiVersion("1.0")]
    public class FaceTemplateController : Controller
    {
        private readonly FaceTemplateRepository _faceTemplateRepository;


        public FaceTemplateController(FaceTemplateRepository faceTemplateRepository)
        {
            _faceTemplateRepository = faceTemplateRepository;
        }

        [HttpPut]
        [Route("ModifyFaceTemplate")]
        public Task<ResultViewModel> ModifyFaceTemplate([FromBody]FaceTemplate faceTemplate)
        {
            return Task.Run(() => _faceTemplateRepository.ModifyFaceTemplate(faceTemplate));
        }

        [HttpDelete]
        [Route("DeleteFaceTemplate")]
        public Task<ResultViewModel> DeleteFaceTemplate(long userId = 0, int index = 0)
        {
            return Task.Run(() => _faceTemplateRepository.DeleteFaceTemplate(userId, index));
        }


    }
}
