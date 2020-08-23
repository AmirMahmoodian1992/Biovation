using Biovation.CommonClasses.Manager;
using Biovation.Domain;
using DataAccessLayerCore.Repositories;
using RestSharp;

namespace Biovation.Repository.API.v2
{
    public class DeviceRepository
    {
        private readonly GenericRepository _repository;

        private readonly RestClient _restClient;
        public DeviceRepository(GenericRepository repository)
        {
            _repository = repository;
            _restClient = (RestClient)new RestClient($"http://localhost:{BiovationConfigurationManager.BiovationWebServerPort}/biovation/dataFlow/queries/v2").UseSerializer(() => new RestRequestJsonSerializer());
        }

        /// <summary>
        /// <En>Get all devices from database.</En>
        /// <Fa>اطلاعات تمامی دستگاه ها را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <returns></returns>
        /// 

        //public ResultViewModel<DeviceBasicInfo> GetDevice(long id, int adminUserId = 0)
        //{
           
        //}
        public ResultViewModel<PagingResult<DeviceBasicInfo>> GetDevices(long adminUserId = 0, int groupId = 0, uint code = 0,
            int brandId = 0, string name = null, int modelId = 0, int typeId = 0, int pageNumber = default, int PageSize = default)
        {
            var restRequest = new RestRequest("Devices", Method.GET);
            var requestResult = _restClient.ExecuteAsync<ResultViewModel<PagingResult<DeviceBasicInfo>>>(restRequest);
            return requestResult.Result.Data;
        }

        //public ResultViewModel AddDevice(DeviceBasicInfo device)
        //{
            

        //}

        //public PagingResult<Lookup> GetDeviceBrands(int code = default, string name = default, int pageNumber = default, int PageSize = default)
        //{
           
        //public PagingResult<DeviceModel> GetDeviceModels(long id=0,string brandId = default, string name = default, int pageNumber = default, int PageSize = default)
        //{

          
        //}

        //public ResultViewModel DeleteDevice(uint deviceId)
        //{
            
        //}


        //public ResultViewModel DeleteDevices(List<int> deviceIds)
        //{
           
                
            
        //}

        //public ResultViewModel AddDeviceModel(DeviceModel deviceModel)
        //{
           
        //}

        //public ResultViewModel ModifyDeviceBasicInfoByID(DeviceBasicInfo device)
        //{
            
        //}

        //public AuthModeMap GetBioAuthModeWithDeviceId(int deviceId, int authMode)
        //{
            
        //}

        //public DateTime GetLastConnectedTime(uint deviceId)
        //{
            
        //}

        //public ResultViewModel AddNetworkConnectionLog(DeviceBasicInfo device)
        //{
            
        //}

        //public Task<List<User>> GetAuthorizedUsersOfDevice(int deviceId)
        //{
           
        //}
    }
}