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

        [HttpGet]
        [Route("TemplateCount")]
        public Task<ResultViewModel<PagingResult<UserTemplateCount>>> GetTemplateCount()
        {
            return Task.Run(() => _fingerTemplateRepository.GetFingerTemplatesCount());
        }
        
        [HttpGet]
        public Task<ResultViewModel<PagingResult<FingerTemplate>>> FingerTemplates(int userId, int templateIndex,Lookup fingerTemplateType, int from = 0, int size = 0, int pageNumber = default,
            int PageSize = default)
        {
            return Task.Run(() => _fingerTemplateRepository.FingerTemplates(userId, templateIndex, fingerTemplateType, from, size, pageNumber, PageSize));
        }

        [HttpGet]
        [Route("FingerTemplateTypes")]
        public Task<int> GetFingerTemplatesCountByFingerTemplateType(Lookup fingerTemplateType)
        {
            try
            {
                return Task.Run(() => _fingerTemplateRepository.GetFingerTemplatesCountByFingerTemplateType(fingerTemplateType));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpGet]
        public Task<ResultViewModel<PagingResult<Lookup>>> GetFingerTemplateTypes(string brandId)
        {
            return Task.Run(() => _fingerTemplateRepository.GetFingerTemplateTypes(brandId));
        }
    }
}
