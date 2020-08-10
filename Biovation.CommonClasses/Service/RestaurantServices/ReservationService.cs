using Biovation.CommonClasses.Models;
using Biovation.CommonClasses.Repository.RestaurantRepositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Biovation.CommonClasses.Models.RestaurantModels;

namespace Biovation.CommonClasses.Service.RestaurantServices
{
    public class ReservationService
    {
        private readonly ReservationRepository _reservationRepository = new ReservationRepository();

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
