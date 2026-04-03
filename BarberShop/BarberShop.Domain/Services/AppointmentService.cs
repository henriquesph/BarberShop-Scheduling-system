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

            var available = GetAvailableSlots(appointment.AppointmentDate.Date);
            if (!available.Any(slot => slot.Ticks == appointment.AppointmentDate.Ticks))
                return false;


            _repository.Save(appointment);
            return true;
        }

        private bool IsValidData(Appointment app)
        {
            var now = DateTime.Now;

            return app.AppointmentDate >= now &&
                   app.AppointmentDate <= now.AddDays(30) &&
                   !string.IsNullOrWhiteSpace(app.CustomerName);
        }

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

            if (appointment == null)
                return false;

            if (appointment.AppointmentDate < DateTime.Now)
                return false;

            TimeSpan timeUntilAppointment = appointment.AppointmentDate - DateTime.Now;
            if (timeUntilAppointment.TotalHours < 2)
            {
                return false;
            }

            appointment.IsCanceled = true;
            _repository.Update(appointment);
            return true;
        }

        public List<DateTime> GetAvailableSlots(DateTime date)
        {
            var start = date.Date.AddHours(9);
            var end = date.Date.AddHours(18);
            var bookedTimes = _repository.GetAll()
                .Where(a => a.AppointmentDate.Date == date.Date && !a.IsCanceled)
                .Select(a => a.AppointmentDate)
                .ToList();

            var availableSlots = new List<DateTime>();

            for (var slot = start; slot < end; slot = slot.AddMinutes(30))
            {
                bool isAlreadyBooked = bookedTimes.Any(booked => booked.Ticks == slot.Ticks);

                if (!isAlreadyBooked)
                {
                    availableSlots.Add(slot);
                }
            }
            return availableSlots;
        }
    }
}
