using Biovation.CommonClasses.Extension;
using Biovation.Domain;
using Biovation.Repository.Sql.v2;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Data.Commands.Controllers.v2
{
    [ApiController]
    [Route("biovation/api/v2/[controller]")]
    //[ApiVersion("2.0")]
    public class PlateDetectionController : ControllerBase
    {
        private readonly PlateDetectionRepository _plateDetectionRepository;

        public PlateDetectionController(PlateDetectionRepository plateDetectionRepository)
        {
            _plateDetectionRepository = plateDetectionRepository;
        }

        [HttpPost]
        [Authorize]

        public Task<ResultViewModel> AddLicensePlate([FromBody] LicensePlate licensePlate = default)
        {
            return Task.Run(() => _plateDetectionRepository.AddLicensePlate(licensePlate));
        }

        [HttpPost]
        [Route("PlateDetectionLog")]
        [Authorize]

        public Task<ResultViewModel> AddPlateDetectionLog([FromBody] PlateDetectionLog log)
        {
            return Task.Run(() => _plateDetectionRepository.AddPlateDetectionLog(log));
        }

        [HttpDelete]
        [Authorize]
        public Task<ResultViewModel> DeleteLicensePlate([FromBody] LicensePlate licensePlate, DateTime modifiedAt, string modifiedBy, string action)
        {
            return Task.Run(() => _plateDetectionRepository.DeleteLicensePlate(licensePlate, modifiedAt, modifiedBy, action));
        }

        [HttpPost]
        [Route("ManualPlateDetectionLog")]
        [Authorize]

        public async Task<ResultViewModel> AddManualPlateDetectionLog([FromBody] PlateDetectionLog manualLogData)
        {
            var applicantUser = HttpContext.GetUser();
            if (applicantUser is null || applicantUser.Id == 0)
                return new ResultViewModel { Success = false, Code = 400, Message = "User of request is empty, Could not find the applicant user." };

            var plateDetectionLogData = new ManualPlateDetectionLog(manualLogData)
            {
                User = applicantUser
            };

            return await _plateDetectionRepository.AddManualPlateDetectionLog(plateDetectionLogData);
        }

        [HttpPut]
        [Authorize]
        [Route("{parentLogId:int}/ManualPlateDetectionLog")]
        public async Task<ResultViewModel> AddManualPlateDetectionLogOfExistLog([FromRoute] int parentLogId, [FromBody] ManualPlateDetectionLog manualLogData)
        {
            if (parentLogId == 0 || manualLogData.ParentLog?.Id == 0)
                return new ResultViewModel { Success = false, Code = 400, Message = "Wrong parent log id is provided." };

            if (manualLogData.ParentLog?.Id == 0)
            {
                var parentLogData = _plateDetectionRepository.GetPlateDetectionLog(logId: parentLogId);
                if (parentLogData?.Data?.Data?.FirstOrDefault() is null || !parentLogData.Success)
                    return new ResultViewModel { Success = false, Code = 400, Message = "Wrong parent log id provided" };

                manualLogData.ParentLog = parentLogData.Data.Data.FirstOrDefault();
            }

            var applicantUser = HttpContext.GetUser();
            if (applicantUser is null || applicantUser.Id == 0)
                return new ResultViewModel { Success = false, Code = 400, Message = "User of request is empty, Could not find the applicant user." };

            manualLogData.User = applicantUser;
            return await _plateDetectionRepository.AddManualPlateDetectionLog(manualLogData);
        }
    }
}