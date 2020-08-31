using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Biovation.Domain;
using Biovation.Repository.v2;
using DataAccessLayerCore.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Biovation.Data.Commands.Controllers.v2
{

    //[ApiVersion("1.0")]
    [Route("biovation/api/queries/v2/[controller]")]
    public class DeviceGroupController : Controller
    {
        private readonly GenericRepository _repository;

        public DeviceGroupController(GenericRepository repository)
        {
            _repository = repository;
        }


        /// <summary>
        /// <En>Get the device info from database.</En>
        /// <Fa>اطلاعات یک گروه را از دیتابیس دریافت میکند.</Fa>
        /// </summary>
        /// <param name="deviceGroupId">کد گروه</param>
        /// <returns></returns>


        private readonly DeviceGroupRepository _deviceGroupRepository;


        public DeviceGroupController(DeviceGroupRepository deviceGroupRepository)
        {
            _deviceGroupRepository = deviceGroupRepository;
        }
        public ResultViewModel ModifyDeviceGroup(DeviceGroup deviceGroup)
        {
            var parameters = new List<SqlParameter> {
                new SqlParameter("@Id",deviceGroup.Id),
                new SqlParameter("@Name",deviceGroup.Name),
                new SqlParameter("@Description",deviceGroup.Description),
                new SqlParameter("@Devices", JsonConvert.SerializeObject(deviceGroup.Devices.Select(device => new {device.DeviceId} )))
                };

            return _repository.ToResultList<ResultViewModel>("InsertDeviceGroup", parameters).Data.FirstOrDefault();
        }

        public ResultViewModel ModifyDeviceGroupMember(string node, int groupId)
        {
            var parameters = new List<SqlParameter> {
                new SqlParameter("@DeviceGroupMember",node),
                new SqlParameter("@DeviceGroupId",groupId)
            };
            return _repository.ToResultList<ResultViewModel>("InsertDeviceGroupMember", parameters).Data.FirstOrDefault();
        }

        public ResultViewModel DeleteDeviceGroup(int id)
        {
            var parameters = new List<SqlParameter> {
                new SqlParameter("@GroupId",id)
                };

            return _repository.ToResultList<ResultViewModel>("DeleteGroupDevice", parameters).Data.FirstOrDefault();
        }

        public ResultViewModel DeleteDeviceGroupMember(uint id)
        {
            var parameters = new List<SqlParameter> {
                new SqlParameter("@GroupId",id)
                };

            return _repository.ToResultList<ResultViewModel>("DeleteGroupDeviceMemeber", parameters).Data.FirstOrDefault();
        }


    }
}
