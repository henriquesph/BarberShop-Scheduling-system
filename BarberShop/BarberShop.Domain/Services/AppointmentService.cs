using BarberShop.Domain.Entities;
using BarberShop.Domain.Interfaces;

namespace BarberShop.Domain.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repository;

        public AppointmentService(IAppointmentRepository repository)
        {
            _repository = repository;
        }
        public bool CreateAppointment(Appointment appointment)
        {
            if (appointment.AppointmentDate < DateTime.Now)
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(appointment.CustomerName))
            {
                return false;
            }
            var hour = appointment.AppointmentDate.Hour;
            if(hour < 9 || hour >= 18)
            {
                return false;
            }

            if(_repository.IsSlotTaken(appointment.AppointmentDate, appointment.BarberName))
            {
                return false;
            }

            return true;
        }
    }
}
