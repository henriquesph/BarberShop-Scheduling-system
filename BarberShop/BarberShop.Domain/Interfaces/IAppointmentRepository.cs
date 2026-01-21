using BarberShop.Domain.Entities;

namespace BarberShop.Domain.Interfaces
{
    public interface IAppointmentRepository
    {
        bool IsSlotTaken(DateTime date, string barberName);
        void Save(Appointment appointment);
        IEnumerable<Appointment> GetAll();
        Appointment? GetById(int id);
        void Update(Appointment appointment);
    }
}
