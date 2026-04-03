using BarberShop.Domain.Entities;

namespace BarberShop.Domain.Interfaces
{
    public interface IAppointmentRepository
    {
        void Save(Appointment appointment);
        IEnumerable<Appointment> GetAll();
        Appointment? GetById(int id);
        void Update(Appointment appointment);
    }
}
