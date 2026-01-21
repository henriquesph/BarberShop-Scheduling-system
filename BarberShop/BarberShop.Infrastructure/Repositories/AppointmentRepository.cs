using BarberShop.Domain.Entities;
using BarberShop.Domain.Interfaces;
using System.Linq;

namespace BarberShop.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly BarberDbContext _context;

        public AppointmentRepository(BarberDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Appointment> GetAll()
        {
            return _context.Appointments.ToList();
        }

        public Appointment? GetById(int id)
        {
            return _context.Appointments.Find();
        }

        public bool IsSlotTaken(DateTime date, string barberName)
        {
            return _context.Appointments.Any(a =>
            a.AppointmentDate == date &&
            a.BarberName == barberName);
        }

        public void Save(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            _context.SaveChanges();
        }

        public void Update(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            _context.SaveChanges();
        }
    }
}
