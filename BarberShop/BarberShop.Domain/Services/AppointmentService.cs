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
            //if (!IsUpToAMonth(appointment)) return false;
            if (!IsWithinBusinessHours(appointment.AppointmentDate)) return false;
            if (!IsOnThirtyMinuteInterval(appointment.AppointmentDate)) return false;

            if (_repository.IsSlotTaken(appointment.AppointmentDate, appointment.BarberName))
                return false;

            _repository.Save(appointment);
            return true;
        }

        //private bool IsValidData(Appointment app) =>
        //    app.AppointmentDate >= DateTime.Now && !string.IsNullOrWhiteSpace(app.CustomerName);

        //private bool IsValidData(Appointment app) =>
        //    app.AppointmentDate >= DateTime.Now &&
        //    app.AppointmentDate <= DateTime.Now.AddDays(30) &&
        //    !string.IsNullOrWhiteSpace(app.CustomerName);

        //private bool IsUpToAMonth(Appointment app) =>
        //    app.AppointmentDate <= DateTime.Now.AddDays(30);

        //private bool IsValidData(Appointment app, DateTime now) =>
        //    app.AppointmentDate >= now &&
        //    app.AppointmentDate <= now.AddDays(30) &&
        //    !string.IsNullOrWhiteSpace(app.CustomerName);

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

        //public List<DateTime> GetAvailableSlots(DateTime date)
        //{
        //    var startHour = 9;
        //    var endHour = 18;

        //    var bookedAppointments = _repository.GetAll()
        //        .Where(a => a.AppointmentDate == date.Date && !a.IsCanceled)
        //        .Select(a => a.AppointmentDate)
        //        .ToList();

        //    var availableSlots = new List<DateTime>();

        //    // 2. Generate slots every 30 minutes from start to end
        //    for (var slot = date.Date.AddHours(startHour); slot < date.Date.AddHours(endHour); slot = slot.AddMinutes(30))
        //    {
        //        // 3. Only add the slot if it's not already booked
        //        //if (!bookedAppointments.Contains(slot))
        //        if (!bookedAppointments.Any(a => a.Ticks == slot.Ticks))
        //        {
        //            availableSlots.Add(slot);
        //        }
        //    }
        //    return availableSlots;
        //}

        public List<DateTime> GetAvailableSlots(DateTime date)
        {
            // 1. Setup the business window (09:00 - 18:00)
            var start = date.Date.AddHours(9);
            var end = date.Date.AddHours(18);

            // 2. Fetch all active appointments for that day from the Repository
            // We use .ToList() to bring the data into memory for comparison
            var bookedTimes = _repository.GetAll()
                .Where(a => a.AppointmentDate.Date == date.Date && !a.IsCanceled)
                .Select(a => a.AppointmentDate)
                .ToList();

            var availableSlots = new List<DateTime>();

            // 3. Iterate in 30-minute steps
            for (var slot = start; slot < end; slot = slot.AddMinutes(30))
            {
                // 4. The "Safety" Check: 
                // We use .Any() to check if ANY booked time matches our slot's Ticks.
                // This avoids issues with TimeZones or Milliseconds.
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
