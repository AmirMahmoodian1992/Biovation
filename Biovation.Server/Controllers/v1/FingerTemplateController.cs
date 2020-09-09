using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.CommonClasses;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;

namespace Biovation.Server.Controllers.v1
{
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class FingerTemplateController : Controller
    {
        private readonly FingerTemplateService _fingerTemplateService;

        public FingerTemplateController(FingerTemplateService fingerTemplateService)
        {
            _fingerTemplateService = fingerTemplateService;
        }
        public ResultViewModel ModifyUser(FingerTemplate fingerTemplate)
        {
            try
            {
                return _fingerTemplateService.ModifyFingerTemplate(fingerTemplate);
            }
            catch (Exception exception)
            {
                Logger.Log(exception, $"Error on modifying user template, user: {fingerTemplate.UserId}.");
                return new ResultViewModel { Id = fingerTemplate.UserId, Validate = 0, Message = $"Error on modifying user template, user: {fingerTemplate.UserId}." };
            }
        }

        [HttpGet]
        public List<UserTemplateCount> GetTemplateCount()
        {
            try
            {
                return _fingerTemplateService.GetTemplateCount();
            }
            catch (Exception exception)
            {
                Logger.Log(exception, "Error on get count template.");
                return new List<UserTemplateCount>();
            }
        }

        [HttpGet]
        public List<FingerTemplate> GetFingerTemplateByUserId(long userId)
        {
            try
            {
                return _fingerTemplateService.FingerTemplates(userId: (int)userId);
            }
            catch (Exception exception)
            {
                Logger.Log(exception, $"Error on getting finger template of user: {userId}.");
                return new List<FingerTemplate>();
            }
        }

        [HttpGet]
        public List<FingerTemplate> GetFingerTemplateByUserIdAndTemplateIndex(int userId, int templateIndex)
        {
            try
            {
                return _fingerTemplateService.FingerTemplates(userId: userId, templateIndex: templateIndex);
            }
            catch (Exception exception)
            {
                Logger.Log(exception, $"Error on getting finger template of user: {userId}.");
                return new List<FingerTemplate>();
            }
        }

        [HttpPost]
        public ResultViewModel DeleteFingerTemplateByUserId(int userId)
        {
            try
            {
                return _fingerTemplateService.DeleteFingerTemplate(userId: userId);
            }
            catch (Exception exception)
            {
                Logger.Log(exception, $"Error on deleting finger template of user: {userId}.");
                return new ResultViewModel { Id = userId, Validate = 0, Message = $"Error on deleting finger template of user: {userId}." };
            }
        }

        [HttpPost]
        public ResultViewModel DeleteFingerTemplateByUserIdAndTemplateIndex(int userId, int templateIndex)
        {
            try
            {
                return _fingerTemplateService.DeleteFingerTemplate(userId, templateIndex);
            }
            catch (Exception exception)
            {
                Logger.Log(exception, $"Error on deleting finger template of user: {userId}.");
                return new ResultViewModel { Id = userId, Validate = 0, Message = $"Error on deleting finger template of user: {userId}." };
            }
        }


        public Task<ResultViewModel<List<Lookup>>> GetFingerTemplateTypes(string brandId = default)
        {
            return Task.Run(() =>
            {
                try
                {
                    var templateTypes = _fingerTemplateService.GetFingerTemplateTypes(brandId);
                    return new ResultViewModel<List<Lookup>> { Id = Convert.ToInt64(brandId), Validate = 1, Data = templateTypes };
                }
                catch (Exception exception)
                {
                    Logger.Log(exception, "Error on getting finger template types.");
                    return new ResultViewModel<List<Lookup>> { Validate = 0, Message = exception.Message };
                }
            });
        }
    }
}
