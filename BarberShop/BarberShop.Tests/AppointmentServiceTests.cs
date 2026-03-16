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

        //[Fact]
        //public void GetAvailableSlots_ShouldNotReturnBookedTime()
        //{
        //    // Arrange
        //    var testDate = DateTime.Now.AddDays(1).Date.AddHours(10); // 10:00 AM tomorrow
        //    var appointment = new Appointment
        //    {
        //        CustomerName = "Test",
        //        BarberName = "Jack",
        //        AppointmentDate = testDate,
        //        IsCanceled = false
        //    };

        //    // 1. Manually add it to the repository or ensure the service saves it
        //    _service.CreateAppointment(appointment);

        //    // Act
        //    var availableSlots = _service.GetAvailableSlots(testDate.Date);

        //    // Assert
        //    var allAppointments = _repository.GetAll();
        //    // We expect 10:00 AM to be MISSING from the list
        //    Assert.DoesNotContain(testDate, availableSlots);
        //}

        //[Fact]
        //public void GetAvailableSlots_ShouldNotReturnBookedTime()
        //{


        //    // 1. Arrange
        //    var testDate = new DateTime(2026, 3, 7, 10, 0, 0); // 10:00 AM
        //    var appointment = new Appointment
        //    {
        //        CustomerName = "Test",
        //        BarberName = "Jack",
        //        AppointmentDate = testDate,
        //        IsCanceled = false
        //    };

        //    // 2. "Train" the Repository Mock
        //    // Tell the mock: "When someone calls GetAll(), return a list containing this appointment"
        //    _mockRepo.Setup(r => r.GetAll())
        //                               .Returns(new List<Appointment> { appointment });

        //    // 3. Act
        //    var availableSlots = _service.GetAvailableSlots(testDate.Date);

        //    // 4. Assert
        //    Assert.DoesNotContain(testDate, availableSlots);
        //}


        [Fact]
        public void GetAvailableSlots_ShouldNotReturnBookedTime()
        {
            // 1. Arrange
            // We create a specific date and time (March 7th, 2026 at 10:00 AM)
            var testDate = new DateTime(2026, 3, 7, 10, 0, 0);

            var bookedAppointment = new Appointment
            {
                Id = 1,
                CustomerName = "Henrique",
                BarberName = "Jack",
                AppointmentDate = testDate,
                IsCanceled = false
            };

            // We create a list that represents our "Fake Database"
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