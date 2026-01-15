using Microsoft.AspNetCore.Mvc;
using TimeTracking.API.Dtos;
using TimeTracking.API.Models;
using TimeTracking.API.Repositories.Interfaces;

namespace TimeTracking.API.Controllers
{
    [Route("api/time-entries")]
    [ApiController]
    public class TimeEntryController(ITimeEntryRepository timeEntryRepo, 
        ILogger<TimeEntryController> logger) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? employee, CancellationToken ct)
        {
            var entries = await timeEntryRepo.GetAllAsync(employee, ct);
            return Ok(entries);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var entry = await timeEntryRepo.GetByIdAsync(id, ct);
            if (entry == null) 
            {
                return NotFound();
            }
            return Ok(entry);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TimeEntryDto request, CancellationToken ct)
        {
            var currentTotal = await timeEntryRepo.GetTotalHoursForDayAsync(request.EmployeeName, request.Date, ct);

            if (currentTotal + request.HoursWorked > 24)
            {
                logger.LogWarning("Overtime alert: {User} tried to log > 24h", request.EmployeeName);
                return BadRequest($"Total hours for {request.Date:d} cannot exceed 24. Current: {currentTotal}");
            }

            var entry = new TimeEntry
            {
                EmployeeName = request.EmployeeName,
                Date = request.Date,
                HoursWorked = request.HoursWorked,
                Description = request.Description
            };

            var id = await timeEntryRepo.CreateAsync(entry, ct);
            entry.Id = id;

            return CreatedAtAction(nameof(GetById), new { id = entry.Id }, entry);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TimeEntryDto request, CancellationToken ct)
        {
            var existingEntry = await timeEntryRepo.GetByIdAsync(id, ct);
            if (existingEntry == null) return NotFound();

            var currentTotal = await timeEntryRepo.GetTotalHoursForDayAsync(request.EmployeeName, request.Date, ct);

            decimal projectedHours;
            if (existingEntry.Date.Date == request.Date.Date && existingEntry.EmployeeName == request.EmployeeName)
            {
                projectedHours = currentTotal - existingEntry.HoursWorked + request.HoursWorked;
            }
            else
            {
                projectedHours = currentTotal + request.HoursWorked;
            }

            if (projectedHours > 24)
            {
                return BadRequest("Total hours cannot exceed 24.");
            }

            existingEntry.EmployeeName = request.EmployeeName;
            existingEntry.Date = request.Date;
            existingEntry.HoursWorked = request.HoursWorked;
            existingEntry.Description = request.Description;

            await timeEntryRepo.UpdateAsync(existingEntry, ct);

            return NoContent();
        }

    }
}
