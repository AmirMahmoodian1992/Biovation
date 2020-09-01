using Biovation.Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Biovation.Repository.SQL.v2;
using Microsoft.AspNetCore.Mvc;


namespace Biovation.Data.Queries.Controllers.v2
{
    //[Route("Biovation/Api/{controller}/{action}", Name = "Device")]
    //[Route("biovation/api/v{version:apiVersion}/[controller]")]
    [Route("biovation/api/queries/v2/[controller]")]

    public class FingerTemplateController : Controller
    {

        private readonly FingerTemplateRepository _fingerTemplateRepository;


        public FingerTemplateController(FingerTemplateRepository fingerTemplateRepository)
        {
            _fingerTemplateRepository = fingerTemplateRepository;
        }
        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک یوزر را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <param name="fingerTemplate"></param>
        /// <returns></returns>

        public List<UserTemplateCount> GetFingerTemplatesCount()
        {
            return _repository.ToResultList<UserTemplateCount>("SelectTemplatesCount").Data;
        }


        [HttpGet]
        [Route("TemplateCount")]
        public Task<IActionResult> GetTemplateCount()
        {
            throw null;
        }

        [HttpGet]
        [Route("FingerTemplateTypes")]
        public Task<IActionResult> GetFingerTemplateTypes(string brandId = default)
        {
            throw null;
        }

        [HttpGet]
        public Task<ResultViewModel<PagingResult<FingerTemplate>>> FingerTemplates(int userId, int templateIndex,Lookup fingerTemplateType, int from = 0, int size = 0, int pageNumber = default,
            int PageSize = default)
        {
            return Task.Run(() => _fingerTemplateRepository.FingerTemplates(userId,templateIndex,fingerTemplateType,from,size,pageNumber,PageSize));
        }

        public int GetFingerTemplatesCountByFingerTemplateType(Lookup fingerTemplateType)
        {
            try
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@FingerTemplateType", fingerTemplateType.Code)
                };

                return _repository.ToResultList<int>("SelectFingerTemplatesCountByFingerTemplateType", parameters).Data.FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public List<Lookup> GetFingerTemplateTypes(string brandId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@brandId", brandId)
            };

            return _repository.ToResultList<Lookup>("SelectFingerTemplateTypes", parameters, fetchCompositions: true).Data;
        }
    }
}
