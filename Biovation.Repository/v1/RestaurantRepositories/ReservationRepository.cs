using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Biovation.Domain;
using Biovation.Domain.RestaurantModels;
using DataAccessLayerCore.Repositories;
using Newtonsoft.Json;

namespace Biovation.Repository.Sql.v1.RestaurantRepositories
{
    public class ReservationRepository
    {
        private readonly GenericRepository _repository;

        public ReservationRepository(GenericRepository repository)
        {
            _repository = repository;
        }

        public List<Reservation> GetReservations(int reservationId = default, int deviceId = default)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@reservationId", reservationId),
                new SqlParameter("@deviceId", deviceId)
            };

            return _repository.ToResultList<Reservation>("SelectReservations", parameters, fetchCompositions: true, compositionDepthLevel: 3).Data;
        }

        public ResultViewModel ModifyReservation(Reservation reservation)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", reservation.Id),
                new SqlParameter("@UserId", reservation.User.Id),
                new SqlParameter("@FoodId", reservation.Food.Id),
                new SqlParameter("@MealId", reservation.Meal.Id),
                new SqlParameter("@RestaurantId", reservation.Restaurant.Id),
                new SqlParameter("@Count", reservation.Count),
                new SqlParameter("@ReservationDate", reservation.ReserveTime)
            };

            return _repository.ToResultList<ResultViewModel>("ModifyReservation", parameters).Data.FirstOrDefault();
        }

        public ResultViewModel AddReservations(List<Reservation> reservations)
        {
            var serializedObj = JsonConvert.SerializeObject(reservations);
            var reservationsDataTable = JsonConvert.DeserializeObject<DataTable>(serializedObj);

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@reservationTable", reservationsDataTable)
            };

            return _repository.ToResultList<ResultViewModel>("InsertReservationsBatch", parameters).Data.FirstOrDefault();
        }
    }
}
