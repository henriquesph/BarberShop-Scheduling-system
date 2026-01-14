using BarberShop.Domain.Entities;

namespace BarberShop.Domain.Interfaces
{
    public interface IAppointmentRepository
    {
        bool IsSlotTaken(DateTime date, string barberName);
    }
}
