using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TimeTracking.API.Controllers;
using TimeTracking.API.Dtos;
using TimeTracking.API.Models;
using TimeTracking.API.Repositories.Interfaces;
using Xunit;

namespace TimeTracking.Tests;

public class TimeTrackingTests
{
    private readonly Mock<ITimeEntryRepository> _mockRepo;
    private readonly Mock<ILogger<TimeEntryController>> _mockLogger;
    private readonly TimeEntryController _controller;

    public TimeTrackingTests()
    {
        _mockRepo = new Mock<ITimeEntryRepository>();
        _mockLogger = new Mock<ILogger<TimeEntryController>>();
        _controller = new TimeEntryController(_mockRepo.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenTotalHoursExceed24()
    {
        var dto = new TimeEntryDto
        {
            EmployeeName = "Workaholic",
            Date = DateTime.Today,
            HoursWorked = 5
        };

        _mockRepo.Setup(r => r.GetTotalHoursForDayAsync(dto.EmployeeName, dto.Date, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(20);

        var result = await _controller.Create(dto, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Contains("exceed 24", badRequest.Value?.ToString());

        _mockRepo.Verify(r => r.CreateAsync(It.IsAny<TimeEntry>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Create_ReturnsCreated_WhenHoursAreValid()
    {
        var dto = new TimeEntryDto { EmployeeName = "Good User", Date = DateTime.Today, HoursWorked = 8 };

        _mockRepo.Setup(r => r.GetTotalHoursForDayAsync(dto.EmployeeName, dto.Date, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(0);

        _mockRepo.Setup(r => r.CreateAsync(It.IsAny<TimeEntry>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(100);

        var result = await _controller.Create(dto, CancellationToken.None);

        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(100, ((TimeEntry)createdResult.Value!).Id);
    }
}