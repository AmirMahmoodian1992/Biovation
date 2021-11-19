using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using RestSharp;
using System.Threading.Tasks;

namespace Biovation.Repository.Api.v2
{
    public class VehicleRepository
    {
        private readonly RestClient _restClient;
        private readonly BiovationConfigurationManager _biovationConfigurationManager;
        public VehicleRepository(RestClient restClient, BiovationConfigurationManager biovationConfigurationManager)
        {
            _restClient = restClient;
            _biovationConfigurationManager = biovationConfigurationManager;
        }

        public async Task<ResultViewModel<PagingResult<Vehicle>>> GetVehicle(int vehicleId, string token = default)
        {
            var restRequest = new RestRequest("Queries/v2/Vehicle", Method.GET);
            restRequest.AddQueryParameter(nameof(vehicleId), vehicleId.ToString());
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel<PagingResult<Vehicle>>>(restRequest);
            return requestResult.Data;
        }

        public async Task<ResultViewModel> InsertVehicle(Vehicle vehicle, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/Vehicle", Method.POST);
            restRequest.AddJsonBody(vehicle);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }
        public async Task<ResultViewModel> ModifyVehicle(Vehicle vehicle, string token = default)
        {
            var restRequest = new RestRequest("Commands/v2/Vehicle", Method.PUT);
            restRequest.AddJsonBody(vehicle);
            token ??= _biovationConfigurationManager.DefaultToken;
            restRequest.AddHeader("Authorization", token);
            var requestResult = await _restClient.ExecuteAsync<ResultViewModel>(restRequest);
            return requestResult.Data;
        }
    }
}