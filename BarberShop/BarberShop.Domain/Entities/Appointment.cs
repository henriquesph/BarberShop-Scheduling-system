using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberShop.Domain.Entities
{
    public class Appointment
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string BarberName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public bool IsCanceled { get; set; } = false;
    }
}
