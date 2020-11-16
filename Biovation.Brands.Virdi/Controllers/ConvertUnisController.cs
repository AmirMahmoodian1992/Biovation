using Biovation.Brands.Virdi.Manager;
using Biovation.Brands.Virdi.Model;
using Biovation.Brands.Virdi.Service;
using Biovation.Constants;
using Biovation.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biovation.Service.Api.v1;
using UNIONCOMM.SDK.UCBioBSP;

namespace Biovation.Brands.Virdi.Controllers
{
    [Route("Biovation/Api/[controller]/[action]")]
    public class ConvertUnisController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly UserCardService _userCardService;
        private readonly FaceTemplateTypes _faceTemplateTypes;
        private readonly FaceTemplateService _faceTemplateService;
        private readonly FingerTemplateTypes _fingerTemplateTypes;
        private readonly FingerTemplateService _fingerTemplateService;
        private readonly BiometricTemplateManager _biometricTemplateManager;

        public ConvertUnisController(UserService userService, UserCardService userCardService, FaceTemplateService faceTemplateService, FingerTemplateService fingerTemplateService, FingerTemplateTypes fingerTemplateTypes, FaceTemplateTypes faceTemplateTypes, BiometricTemplateManager biometricTemplateManager)
        {
            _userService = userService;
            _userCardService = userCardService;
            _faceTemplateService = faceTemplateService;
            _fingerTemplateService = fingerTemplateService;
            _fingerTemplateTypes = fingerTemplateTypes;
            _faceTemplateTypes = faceTemplateTypes;
            _biometricTemplateManager = biometricTemplateManager;
        }

        [HttpPost]
        public IActionResult Convert(DatabaseConnectionParameters connectionParameters)
        {
            try
            {
                connectionParameters.DatabaseName = string.IsNullOrWhiteSpace(connectionParameters.DatabaseName) ? "Unis" : connectionParameters.DatabaseName;
                connectionParameters.UserName = string.IsNullOrWhiteSpace(connectionParameters.UserName) ? "unisuser" : connectionParameters.UserName;
                connectionParameters.Password = string.IsNullOrWhiteSpace(connectionParameters.Password) ? "unisamho" : connectionParameters.Password;
                ConvertToUnis(connectionParameters).Wait();
                return Ok();
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }
        }

        [HttpGet]
        public IActionResult Convert(string serverAddress, string databaseName = "Unis", string username = "unisuser", string password = "unisamho")
        {
            try
            {
                var databaseConnectionParameters = new DatabaseConnectionParameters
                { Server = serverAddress, DatabaseName = databaseName, UserName = username, Password = password };
                ConvertToUnis(databaseConnectionParameters).Wait();
                return Ok();
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }
        }

        private Task ConvertToUnis(DatabaseConnectionParameters connectionParameters)
        {
            return Task.Run(async () =>
            {
                var virdiUnisService = new VirdiUnisService(connectionParameters.Server,
                    connectionParameters.DatabaseName, connectionParameters.UserName, connectionParameters.Password);

                var usersAwaiter = virdiUnisService.GetAllUser();
                var userCardsAwaiter = virdiUnisService.GetAllUserCards();
                var faceTemplatesAwaiter = virdiUnisService.GetAllFaceTemplates();
                var fingerTemplatesAwaiter = virdiUnisService.GetAllFingerTemplatesFromUnis();

                var users = await usersAwaiter;
                var userCards = await userCardsAwaiter;
                var faceTemplates = await faceTemplatesAwaiter;
                var fingerTemplates = await fingerTemplatesAwaiter;

                foreach (var user in users)
                {
                    var biovationUser = new User
                    {
                        Id = 0,
                        Code = user.UserId,
                        SurName = user.UserName,
                        UserName = user.UserName,
                        AdminLevel = 0,
                        StartDate = user.StartDate == default ? new DateTime(1990, 1, 1) : user.StartDate,
                        EndDate = user.EndDate == default ? new DateTime(1990, 1, 1) : user.EndDate,
                        IsActive = true,
                        RegisterDate = DateTime.Now,
                        EntityId = 1,
                        Type = 1
                    };

                     _userService.ModifyUser(biovationUser);
                }

                var ucBioApi = new UCBioAPI();
                var ucBioApiExport = new UCBioAPI.Export(ucBioApi);

                var biovationFingerTemplates = new List<FingerTemplate>();
                foreach (var fingerTemplate in fingerTemplates)
                {
                    var encodedTemplate = System.Text.Encoding.ASCII.GetString(fingerTemplate.Template);
                    var fir = new UCBioAPI.Type.FIR_TEXTENCODE
                    {
                        IsWideChar = fingerTemplate.IsWideChar == 1,
                        TextFIR = encodedTemplate
                    };

                    ucBioApiExport.FIRToTemplate(fir, out var export, UCBioAPI.Type.TEMPLATE_TYPE.SIZE400);

                    for (var j = 0; j < export.FingerNum; j++)
                    {
                        var finger = export.FingerInfo[j];
                        for (var k = 0; k < export.SamplesPerFinger; k++)
                        {
                            var templateSample = finger.Template[k];
                            biovationFingerTemplates.Add(new FingerTemplate
                            {
                                UserId = fingerTemplate.UserId,
                                Index = export.FingerInfo[j].FingerID,
                                FingerIndex = _biometricTemplateManager.GetFingerIndex(export.FingerInfo[j].FingerID),
                                Template = templateSample.Data,
                                CheckSum = templateSample.Data.Sum(x => x),
                                Size = templateSample.Data.Length,
                                TemplateIndex = k + 1,
                                FingerTemplateType = _fingerTemplateTypes.V400
                            });
                        }
                    }
                }

                var fingerTemplateInsertionAwaiter = Task.Run(() =>
                {
                    foreach (var fingerTemplate in biovationFingerTemplates)
                    {
                        _fingerTemplateService.ModifyFingerTemplate(fingerTemplate);
                    }
                });

                var userCardInsertionAwaiter = Task.Run(() =>
                {
                    foreach (var userCard in userCards)
                    {
                        _userCardService.ModifyUserCard(userCard);
                    }
                });

                var faceTemplateInsertionAwaiter = Task.Run(() =>
                {
                    var biovationFaceTemplates = faceTemplates.Select(template => new FaceTemplate
                    {
                        FaceTemplateType = _faceTemplateTypes.VFACE,
                        Index = 1,
                        Template = template.Template,
                        UserId = template.UserId,
                        CheckSum = template.Template.Sum(x => x),
                        Size = template.Template.Length,
                        CreateAt = DateTime.Now,
                        SecurityLevel = 0,
                        EnrollQuality = 0
                    });
                    foreach (var faceTemplate in biovationFaceTemplates)
                    {
                        _faceTemplateService.ModifyFaceTemplate(faceTemplate);
                    }
                });

                await userCardInsertionAwaiter;
                await fingerTemplateInsertionAwaiter;
                await faceTemplateInsertionAwaiter;
            });
        }
    }
}
