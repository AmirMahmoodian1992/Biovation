using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.Domain;
using Biovation.Domain.RestaurantModels;
using Biovation.Repository.RestaurantRepositories;

namespace Biovation.Service.Sql.v1.RestaurantServices
{
    public class ReservationService
    {
        private readonly ReservationRepository _reservationRepository;

        public ReservationService(ReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        public Task<List<Reservation>> GetReservations(int reservationId = default, int deviceId = default)
        {
            return Task.Run(() => _reservationRepository.GetReservations(reservationId, deviceId));
        }

        public Task<ResultViewModel> AddReservations(List<Reservation> reservations)
        {
            return Task.Run(() => _reservationRepository.AddReservations(reservations));
        }

        public Task<ResultViewModel> ModifyReservation(Reservation reservation)
        {
            return Task.Run(() => _reservationRepository.ModifyReservation(reservation));
        }
    }
}
