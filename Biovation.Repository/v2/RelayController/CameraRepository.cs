using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Biovation.Domain;
using DataAccessLayerCore.Extentions;
using DataAccessLayerCore.Repositories;

namespace Biovation.Repository.Sql.v2.RelayController
{
    public class CameraRepository
    {
        private readonly GenericRepository _repository;

        public CameraRepository(GenericRepository repository)
        {
            _repository = repository;
        }

        public ResultViewModel CreateCamera(Camera camera)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@" + nameof(camera.Code), SqlDbType.Int) {Value = camera.Code},
                new SqlParameter("@" + nameof(camera.Name), SqlDbType.NVarChar) { Value = camera.Name ?? string.Empty },
                new SqlParameter("@" + nameof(camera.ConnectionInfo.Ip), SqlDbType.NVarChar) { Value = camera.ConnectionInfo.Ip.ToString()},
                new SqlParameter("@" + nameof(camera.ConnectionInfo.Port), SqlDbType.Int) { Value = camera.ConnectionInfo.Port},
                new SqlParameter("@" + nameof(camera.ConnectionInfo.UserName), SqlDbType.NVarChar) { Value = camera.ConnectionInfo.UserName},
                new SqlParameter("@" + nameof(camera.ConnectionInfo.Password), SqlDbType.NVarChar) { Value = camera.ConnectionInfo.Password},
                new SqlParameter("@" + nameof(camera.ConnectionInfo.MacAddress), SqlDbType.NVarChar) { Value = camera.ConnectionInfo.MacAddress ?? string.Empty},
                new SqlParameter("@" + nameof(camera.Brand) + nameof(camera.Brand.Code), SqlDbType.NVarChar){Value = camera.Brand.Code},
                new SqlParameter("@" + nameof(camera.Description), SqlDbType.NVarChar) { Value = camera.Description ?? string.Empty },
                new SqlParameter("@" + nameof(camera.Active), SqlDbType.Bit) { Value = camera.Active },
                new SqlParameter("@" + nameof(camera.HardwareVersion), SqlDbType.NVarChar) { Value = camera.HardwareVersion ?? string.Empty},
                new SqlParameter("@" + nameof(camera.SerialNumber), SqlDbType.NVarChar) { Value = camera.SerialNumber ?? string.Empty},
                new SqlParameter("@" + nameof(camera.ConnectionUrl), SqlDbType.NVarChar) { Value = camera.ConnectionUrl },
                new SqlParameter("@" + nameof(camera.LiveStreamUrl), SqlDbType.NVarChar) { Value = camera.LiveStreamUrl },
                new SqlParameter("@" + nameof(camera.Model) + nameof(camera.Model.Id), SqlDbType.Int){Value = camera.Model.Id},
            };
            return _repository.ToResultList<ResultViewModel>("InsertCamera", parameters).Data.FirstOrDefault();
        }

        public ResultViewModel UpdateCamera(Camera camera)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@" + nameof(camera.Id), SqlDbType.Int) {Value = camera.Id},
                new SqlParameter("@" + nameof(camera.Code), SqlDbType.Int) {Value = camera.Code},
                new SqlParameter("@" + nameof(camera.Name), SqlDbType.NVarChar) { Value = camera.Name },
                new SqlParameter("@" + nameof(camera.ConnectionInfo.Ip), SqlDbType.NVarChar) { Value = camera.ConnectionInfo.Ip.ToString()},
                new SqlParameter("@" + nameof(camera.ConnectionInfo.Port), SqlDbType.Int) { Value = camera.ConnectionInfo.Port},
                new SqlParameter("@" + nameof(camera.ConnectionInfo.UserName), SqlDbType.NVarChar) { Value = camera.ConnectionInfo.UserName},
                new SqlParameter("@" + nameof(camera.ConnectionInfo.Password), SqlDbType.NVarChar) { Value = camera.ConnectionInfo.Password},
                new SqlParameter("@" + nameof(camera.ConnectionInfo.MacAddress), SqlDbType.NVarChar) { Value = camera.ConnectionInfo.MacAddress},
                new SqlParameter("@" + nameof(camera.Brand) + nameof(camera.Brand.Code), SqlDbType.NVarChar){Value = camera.Brand.Code},
                new SqlParameter("@" + nameof(camera.Description), SqlDbType.NVarChar) { Value = camera.Description },
                new SqlParameter("@" + nameof(camera.Active), SqlDbType.Bit) { Value = camera.Active },
                new SqlParameter("@" + nameof(camera.HardwareVersion), SqlDbType.NVarChar) { Value = camera.HardwareVersion },
                new SqlParameter("@" + nameof(camera.SerialNumber), SqlDbType.NVarChar) { Value = camera.SerialNumber },
                new SqlParameter("@" + nameof(camera.ConnectionUrl), SqlDbType.NVarChar) { Value = camera.ConnectionUrl },
                new SqlParameter("@" + nameof(camera.LiveStreamUrl), SqlDbType.NVarChar) { Value = camera.LiveStreamUrl },
                new SqlParameter("@" + nameof(camera.Model) + nameof(camera.Model.Id), SqlDbType.Int){Value = camera.Model.Id},
            };
            return _repository.ToResultList<ResultViewModel>("UpdateCamera", parameters).Data.FirstOrDefault();

        }

        public ResultViewModel<PagingResult<Camera>> GetCamera(long adminUserId = default, long id = default, uint code = default, string name = default, string ip = default, int port = default,
            string brandCode = default, int modelId = default, int pageNumber = 0, int pageSize = 0, int nestingDepthLevel = 4)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@" + nameof(adminUserId), SqlDbType.BigInt) {Value = adminUserId},
                new SqlParameter("@" + nameof(id), SqlDbType.BigInt) {Value = id},
                new SqlParameter("@" + nameof(code), SqlDbType.Int) {Value = code},
                new SqlParameter("@" + nameof(name), SqlDbType.NVarChar) { Value = name },
                new SqlParameter("@" + nameof(ip), SqlDbType.NVarChar){Value = ip},
                new SqlParameter("@" + nameof(port), SqlDbType.Int) { Value = port},
                new SqlParameter("@" + nameof(brandCode), SqlDbType.NVarChar){Value = brandCode},
                new SqlParameter("@" + nameof(modelId), SqlDbType.Int){Value = modelId},
                new SqlParameter("@" + nameof(pageNumber), SqlDbType.Int) {Value = pageNumber},
                new SqlParameter("@" + nameof(pageSize), SqlDbType.Int) {Value = pageSize},
            };
            return _repository.ToResultList<PagingResult<Camera>>("SelectCamera", parameters,
                    fetchCompositions: nestingDepthLevel != 0, compositionDepthLevel: nestingDepthLevel)
                .FetchFromResultList();
        }

        public ResultViewModel<PagingResult<CameraModel>> GetCameraModel(long id = default, uint manufactureCode = default, string name = default,
            string brandCode = default)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@" + nameof(id), SqlDbType.BigInt) {Value = id},
                new SqlParameter("@" + nameof(name), SqlDbType.NVarChar) { Value = name },
                new SqlParameter("@" + nameof(brandCode), SqlDbType.NVarChar){Value = brandCode},
                new SqlParameter("@" + nameof(manufactureCode), SqlDbType.Int){Value = manufactureCode},
            };
            return _repository.ToResultList<PagingResult<CameraModel>>("SelectCameraModel", parameters, fetchCompositions: true, compositionDepthLevel: 6)
                .FetchFromResultList();
        }
        public ResultViewModel DeleteCamera(long id)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@" + nameof(id), id)
            };

            return _repository.ToResultList<ResultViewModel>("DeleteCamera", parameters).Data.FirstOrDefault();
        }
    }
}
