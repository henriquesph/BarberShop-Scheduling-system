using BarberShop.Domain.Entities;

namespace BarberShop.Domain.Interfaces
{
    public interface IAppointmentService
    {
        bool CreateAppointment(Appointment appointment);
        IEnumerable<Appointment> GetAllAppointments();
        bool CancelAppointment(int id);
        List<DateTime> GetAvailableSlots(DateTime date);
    }
}