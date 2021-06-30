using Biovation.Domain;
using Biovation.Repository.Api.v2;
using System.Threading.Tasks;

namespace Biovation.Service.Api.v2
{
    public class VehicleService
    {
        private readonly VehicleRepository _vehicleRepository;
        public VehicleService(VehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        public async Task<ResultViewModel<PagingResult<Vehicle>>> GetVehicle(int vehicleId, string token = default)
        {
            return await _vehicleRepository.GetVehicle(vehicleId, token);
        }

        public async Task<ResultViewModel> InsertVehicle(Vehicle vehicle, string token = default)
        {
            return await _vehicleRepository.InsertVehicle(vehicle, token);
        }

        public async Task<ResultViewModel> ModifyVehicle(Vehicle vehicle, string token = default)
        {
            return await _vehicleRepository.ModifyVehicle(vehicle, token);
        }


    }
}
