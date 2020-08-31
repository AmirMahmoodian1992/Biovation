using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;


namespace Biovation.Data.Commands.Controllers.v2
{
    //[Route("Biovation/Api/{controller}/{action}", Name = "Device")]
    //[Route("biovation/api/v{version:apiVersion}/[controller]")]
    [Route("biovation/api/queries/v2/[controller]")]
    //[ApiVersion("1.0")]
    public class FaceTemplateController : Controller
    {
        private readonly FaceTemplateRepository _faceTemplateRepository;


        public FaceTemplateController(FaceTemplateRepository faceTemplateRepository)
        {
            _faceTemplateRepository = faceTemplateRepository;
        }


        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک یوزر را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <param name="faceTemplate"></param>
        /// <returns></returns>
        [HttpPut]
        public Task<ResultViewModel> ModifyFaceTemplate(FaceTemplate faceTemplate)
        {
            return Task.Run(() => _faceTemplateRepository.ModifyFaceTemplate(faceTemplate));
        }

        [HttpDelete]
        [Route("DeleteFaceTemplate")]
        public Task<ResultViewModel> DeleteFaceTemplate(long userId = 0 , int index = 0)
        {
            return Task.Run(() => _faceTemplateRepository.DeleteFaceTemplate(userId ,index));
        }

 
    }
}
