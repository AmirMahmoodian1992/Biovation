﻿using Biovation.CommonClasses;
using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using Biovation.Service.Api.v1;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biovation.Server.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("biovation/api/v{version:apiVersion}/[controller]")]
    public class FingerTemplateController : ControllerBase
    {
        private readonly FingerTemplateService _fingerTemplateService;
        private readonly string _kasraAdminToken;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;

        public FingerTemplateController(FingerTemplateService fingerTemplateService, BiovationConfigurationManager biovationConfigurationManager)
        {
            _fingerTemplateService = fingerTemplateService;
            _kasraAdminToken = _biovationConfigurationManager.KasraAdminToken;
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        [HttpPost]
        public ResultViewModel ModifyUser(FingerTemplate fingerTemplate)
        {
            try
            {
                return _fingerTemplateService.ModifyFingerTemplate(fingerTemplate, token: _kasraAdminToken);
            }
            catch (Exception exception)
            {
                Logger.Log(exception, $"Error on modifying user template, user: {fingerTemplate.UserId}.");
                return new ResultViewModel { Id = fingerTemplate.UserId, Validate = 0, Message = $"Error on modifying user template, user: {fingerTemplate.UserId}." };
            }
        }

        [HttpGet]
        [Route("GetTemplateCount")]
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
        [Route("GetFingerTemplateByUserId")]
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
        [Route("GetFingerTemplateByUserIdAndTemplateIndex")]
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
        [Route("DeleteFingerTemplateByUserId")]
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
        [Route("DeleteFingerTemplateByUserIdAndTemplateIndex")]
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

        [HttpGet]
        [Route("GetFingerTemplateTypes")]
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
