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

        public AppointmentServiceTests()
        {
            _mockRepo = new Mock<IAppointmentRepository>();
            _service = new AppointmentService(_mockRepo.Object);
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
            var appointment = new Appointment
            {
                CustomerName = "",
                AppointmentDate = DateTime.Now.AddDays(1)
            };
            var result = _service.CreateAppointment(appointment);

            Assert.False(result);
        }

        [Fact]
        public void CreateAppointment_ShouldReturnFalse_WhenBarberIsBusy()
        {
            // 1. Arrange
            // We create a specific date (Tomorrow at 10:00 AM)
            var appointmentDate = DateTime.Now.AddDays(1).Date.AddHours(10);
            var barber = "Jack";

            var existingAppointment = new Appointment
            {
                Id = 1,
                CustomerName = "Existing Client",
                BarberName = barber,
                AppointmentDate = appointmentDate,
                IsCanceled = false
            };

            _mockRepo.Setup(r => r.GetAll()).Returns(new List<Appointment> { existingAppointment });

            var newAppointment = new Appointment
            {
                CustomerName = "Jhon",
                BarberName = barber,
                AppointmentDate = appointmentDate,
            };

            // 2. Act
            var result = _service.CreateAppointment(newAppointment);

            // 3. Assert
            Assert.False(result);
        }

        [Fact]
        public void CreateAppointment_ShouldReturnFalse_WhenDateAfterOneMonth()
        {
            var dateAfterOneMonth = DateTime.Now.AddDays(32);
            var appointment = new Appointment
            {
                CustomerName = "Alice",
                BarberName = "Jack",
                AppointmentDate = dateAfterOneMonth,
            };
            var result = _service.CreateAppointment(appointment);
            Assert.False(result);
        }


        [Fact]
        public void CreateAppointment_ShouldReturnFalse_WhenTimeIsAfterClosing()
        {
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
            // Arrange:
            var tightSchedule = DateTime.Now.AddHours(1);
            var appointment = new Appointment { Id = 1, AppointmentDate = tightSchedule };

            _mockRepo.Setup(r => r.GetById(1)).Returns(appointment);

            // Act
            var result = _service.CancelAppointment(1);

            //Assert
            Assert.False(result);
        }


        [Fact]
        public void GetAvailableSlots_ShouldNotReturnBookedTime()
        {
            // 1. Arrange
            var testDate = new DateTime(2026, 3, 7, 10, 0, 0);

            var bookedAppointment = new Appointment
            {
                Id = 1,
                CustomerName = "Henrique",
                BarberName = "Jack",
                AppointmentDate = testDate,
                IsCanceled = false
            };

            var fakeDatabase = new List<Appointment> { bookedAppointment };

            // We tell the Mock: "When the Service asks for GetAll(), give it this list"
            _mockRepo.Setup(r => r.GetAll()).Returns(fakeDatabase);

            // 2. Act
            // We ask for slots for that specific day (March 7th, 2026)
            var availableSlots = _service.GetAvailableSlots(testDate.Date);

            // 3. Assert
            // Verify that 10:00 AM is NOT in the list of available slots
            Assert.DoesNotContain(testDate, availableSlots);

            // Optional: Verify that 09:00 AM IS in the list (to ensure the loop works)
            var nineAm = new DateTime(2026, 3, 7, 9, 0, 0);
            Assert.Contains(nineAm, availableSlots);
        }
    }
}