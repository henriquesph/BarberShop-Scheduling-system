using BarberShop.Domain.Entities;
using BarberShop.Domain.Interfaces;
using BarberShop.Domain.Services;
using Xunit;
using Moq; 


namespace BarberShop.Tests
{
    public class AppointmentServiceTests
    {
        private readonly Mock<IAppointmentRepository> _mockRepo;
        private readonly AppointmentService _service;

        public AppointmentServiceTests() {
            _mockRepo = new Mock<IAppointmentRepository>();
            _service = new AppointmentService( _mockRepo.Object);
        }

        [Fact]
        public void CreateAppointment_ShouldReturnFalse_WhenDateIsInThePast()
        {
            // Arrange
            var pastAppointment = new Appointment
            {
                CustomerName = "Jhon Doe",
                BarberName = "Jack The Ripper",
                AppointmentDate = DateTime.Now.AddDays(-1),
            };
        
            // Act
            var result = _service.CreateAppointment(pastAppointment);

            // Assert
            Assert.False(result);


        }

        [Fact]
        public void CreateAppointment_ShouldReturnFalse_WhenCustumerNameIsEmpty()
        {
            var appointment = new Appointment { 
                CustomerName = "",
                AppointmentDate = DateTime.Now.AddDays(1)
            };
            var result = _service.CreateAppointment(appointment);

            Assert.False(result);
        }

        [Fact]
        public void CreateAppointment_ShouldReturnFalse_WhenBarberIsBusy()
        {
            var appointmentDate = DateTime.Now.AddDays(1);
            var barber = "Jack";

            _mockRepo.Setup(r => r.IsSlotTaken(appointmentDate, barber)).Returns(true);

            var newAppointment = new Appointment
            {
                CustomerName = "Jhon",
                BarberName = barber,
                AppointmentDate = appointmentDate,
            };

            // Act
            var result = _service.CreateAppointment(newAppointment);

            Assert.False(result);
        }

        [Fact]
        public void CreateAppointment_ShouldReturnFalse_WhenTimeIsAfterClosing()
        {
            // Arrange: Create a date for tomorrow at 8 PM (20:00)
            var tomorrow = DateTime.Now.AddDays(1);
            var lateDate = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 20, 0, 0);

            var appointment = new Appointment
            {
                CustomerName = "Alice",
                BarberName = "Jack",
                AppointmentDate = lateDate,
            };

            // Act
            var result = _service.CreateAppointment(appointment);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CreateAppointment_ShouldReturnFalse_WhenTimeIsNot_30MinutesInverval()
        {
            var tomorrow = DateTime.Now.AddDays(1);
            var invalidDate = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 16, 15, 0);

            var appointment = new Appointment
            {
                CustomerName = "Alice",
                BarberName = "Jack",
                AppointmentDate = invalidDate,
            };

            // Act
            var result = _service.CreateAppointment(appointment);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CancelAppointment_ShouldReturnFalse_WhenDateIsPast()
        {
            // Arrange
            var pastDate = DateTime.Now.AddDays(-1);
            var appointment = new Appointment { Id = 1, AppointmentDate = pastDate };

            _mockRepo.Setup(r => r.GetById(1)).Returns(appointment);

            // Act
            var result = _service.CancelAppointment(-1);

            // Assert
            Assert.False(result);
        }


        [Fact]
        public void CancelAppointment_ShouldReturnFalse_WhenNoticeIsLessTwoHours()
        {
            // Arrange: Appointment is 1 hour from now
            var tightSchedule = DateTime.Now.AddHours(1);
            var appointment = new Appointment { Id = 1, AppointmentDate = tightSchedule };

            _mockRepo.Setup(r => r.GetById(1)).Returns(appointment);

            // Act
            var result = _service.CancelAppointment(1);

            //Assert
            Assert.False(result);
        }
    }
}
