using BarberShop.Domain.Entities;

namespace BarberShop.Domain.Interfaces
{
    public interface IAppointmentService
    {
        bool CreateAppointment(Appointment appointment);
    }
}
