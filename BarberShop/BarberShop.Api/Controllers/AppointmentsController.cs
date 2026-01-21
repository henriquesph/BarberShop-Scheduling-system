using BarberShop.Domain.Entities;
using BarberShop.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BarberShop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _service;

    public AppointmentsController(IAppointmentService service)
    {
        _service = service;
    }

    [HttpPost]
    public IActionResult Create(Appointment appointment)
    {
        var success = _service.CreateAppointment(appointment);

        if (!success)
        {
            return BadRequest("Invalid appointment. Please check the date, time, or availability.");
        }

        return Ok("Appointment scheduled successfully!");
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var appointments = _service.GetAllAppointments();
        return Ok(appointments);
    }

    [HttpPut("{id}cancel")]
    public IActionResult Cancel(int id)
    {
        var success = _service.CancelAppointment(id);
        if (!success)
            return BadRequest("Could not cancel appointment");

        return Ok();
    }
}
