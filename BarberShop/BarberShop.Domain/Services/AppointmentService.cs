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
            if (!IsValidData(appointment)) return false;
            if (!IsWithinBusinessHours(appointment.AppointmentDate)) return false;
            if (!IsOnThirtyMinuteInterval(appointment.AppointmentDate)) return false;

            if (_repository.IsSlotTaken(appointment.AppointmentDate, appointment.BarberName))
                return false;

            _repository.Save(appointment);
            return true;
        }

        private bool IsValidData(Appointment app) =>
            app.AppointmentDate >= DateTime.Now && !string.IsNullOrWhiteSpace(app.CustomerName);

        private bool IsWithinBusinessHours(DateTime date) =>
            date.Hour >= 9 && date.Hour < 18;

        private bool IsOnThirtyMinuteInterval(DateTime date) =>
            date.Minute == 0 || date.Minute == 30;

        public IEnumerable<Appointment> GetAllAppointments()
        {
            return _repository.GetAll();
        }

        public bool CancelAppointment(int id)
        {
            var appointment = _repository.GetById(id);

            if (appointment == null) return false;

            if(appointment.AppointmentDate < DateTime.Now) return false;

            appointment.IsCanceled = true;

            _repository.Update(appointment);
            return true;
        }
    }
}
