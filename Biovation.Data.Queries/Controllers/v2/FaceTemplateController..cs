using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Repository.SQL.v2;
using DataAccessLayerCore.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Data.Queries.Controllers.v2
{
    //[Route("Biovation/Api/{controller}/{action}", Name = "Device")]
    //[Route("biovation/api/v{version:apiVersion}/[controller]")]
    [Route("biovation/api/queries/v2/[controller]")]

    public class FaceTemplateController : Controller
    {

        private readonly FaceTemplateRepository _FaceTemplateRepository;


        public FaceTemplateController(FaceTemplateRepository faceTemplateRepository)
        {
            _FaceTemplateRepository = faceTemplateRepository;
        }

        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک یوزر را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <param name="faceTemplate"></param>
        /// <returns></returns>

        [HttpGet]
        public Task<ResultViewModel<PagingResult<FaceTemplate>>> FaceTemplates(string fingerTemplateTypeCode = default, long userId = 0, int index = 0, int pageNumber = default,
            int PageSize = default)
        {
            return Task.Run(() =>
                _FaceTemplateRepository.GetFaceTemplates(fingerTemplateTypeCode, userId, index, pageNumber, PageSize);
        }

    }
}