using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System.Threading.Tasks;

namespace Biovation.Service.Api.v2
{
    public class TimeZoneService
    {
        private readonly TimeZoneRepository _timeZoneRepository;
        private readonly DeviceService _deviceService;

        public TimeZoneService(TimeZoneRepository timeZoneRepository, DeviceService deviceService)
        {
            _timeZoneRepository = timeZoneRepository;
            _deviceService = deviceService;
        }

        public async Task<ResultViewModel<TimeZone>> TimeZones(int id = default, string token = default)
        {
            return await _timeZoneRepository.TimeZones(id, token);
        }

        public async Task<ResultViewModel<PagingResult<TimeZone>>> GetTimeZones(string token = default)
        {
            return await _timeZoneRepository.GetTimeZones(token);
        }

        public async Task<ResultViewModel> ModifyTimeZone(TimeZone timeZone, string token = default)
        {
            return await _timeZoneRepository.ModifyTimeZone(timeZone, token);
        }

        public async Task<ResultViewModel> DeleteTimeZone(int id, string token = default)
        {
            return await _timeZoneRepository.DeleteTimeZone(id, token);
        }

        public ResultViewModel SendTimeZoneDevice(int id, int deviceId, string token)
        {
            var device = _deviceService.GetDevice(deviceId, token: token).Result.Data;

            if (device is null)
                return new ResultViewModel
                {
                    Id = id,
                    Code = 404,
                    Message = "The provided device id is wrong",
                    Success = false,
                    Validate = 0
                };

            _timeZoneRepository.SendTimeZoneDevice(id, device);

            return new ResultViewModel { Validate = 1 };
        }

        public ResultViewModel SendTimeZoneToAllDevices(int id, DeviceBasicInfo device, string token = default)
        {
            _timeZoneRepository.SendTimeZoneDevice(id, device, token);

            return new ResultViewModel { Validate = 1 };
        }
    }
}
