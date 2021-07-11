using Biovation.Domain;
using DataAccessLayerCore.Repositories;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Biovation.Repository.Sql.v2
{
    public class VehicleRepository
    {
        private readonly GenericRepository _repository;

        public VehicleRepository(GenericRepository repository)
        {
            _repository = repository;
        }


        public ResultViewModel ModifyVehicle(Vehicle vehicle, int nestingLevel = 4)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", vehicle.Id),
                new SqlParameter("@ColorId", vehicle.Color.Code),
                new SqlParameter("@VehicleModelId", vehicle.Model.Id),
                new SqlParameter("@LicensePlateId", vehicle.Plate.EntityId)
            };

            return _repository.ToResultList<ResultViewModel>("ModifyVehicle", parameters, fetchCompositions: nestingLevel != 0).Data.FirstOrDefault();
        }

        public ResultViewModel InsertVehicle(Vehicle vehicle, int nestingLevel = 4)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ColorId", vehicle.Color.Code),
                new SqlParameter("@VehicleModelId", vehicle.Model.Id),
                new SqlParameter("@LicensePlateId", vehicle.Plate.EntityId)
            };

            return _repository.ToResultList<ResultViewModel>("InsertVehicle", parameters, fetchCompositions: nestingLevel != 0).Data.FirstOrDefault();
        }
        public Task<ResultViewModel<PagingResult<Vehicle>>> GetVehicle(int vehicleId, int nestingLevel = 4)
        {
            return Task.Run(() =>
            {
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Id", vehicleId),
                };

                return _repository
                    .ToResultList<ResultViewModel<PagingResult<Vehicle>>>("SelectVehicleById", parameters,
                        fetchCompositions: nestingLevel != 0).Data.FirstOrDefault();
            });
        }

    }
}
