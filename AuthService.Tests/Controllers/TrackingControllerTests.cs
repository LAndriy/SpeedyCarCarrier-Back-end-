using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using TrackingService.Controllers;
using TrackingService.Data;
using TrackingService.Models;
using System;
using System.Threading.Tasks;
using Xunit;

namespace TrackingService.Tests.Controllers
{
    public class TrackingControllerTests
    {
        private readonly TrackingController _trackingController;
        private readonly Mock<TrackingContext> _contextMock;

        public TrackingControllerTests()
        {
            _contextMock = new Mock<TrackingContext>(new DbContextOptions<TrackingContext>());
            _trackingController = new TrackingController(_contextMock.Object);
        }

        [Fact]
        public async Task GetTracking_Should_Return_Tracking_If_Exists()
        {
            // Arrange
            var orderId = 1;
            var existingTracking = new Tracking { OrderId = orderId, Status = "In transit", UpdatedAt = DateTime.Now };
            _contextMock.Setup(x => x.Trackings.FindAsync(orderId)).ReturnsAsync(existingTracking);

            // Act
            var result = await _trackingController.GetTracking(orderId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Tracking>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedTracking = Assert.IsType<Tracking>(okResult.Value);
            Assert.Equal(orderId, returnedTracking.OrderId);
        }

        [Fact]
        public async Task GetTracking_Should_Return_NotFound_If_Tracking_Not_Exists()
        {
            // Arrange
            var orderId = 1;
            _contextMock.Setup(x => x.Trackings.FindAsync(orderId)).ReturnsAsync((Tracking)null);

            // Act
            var result = await _trackingController.GetTracking(orderId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task PostTracking_Should_Create_New_Tracking()
        {
            // Arrange
            var orderStatus = new OrderStatus { OrderId = 1, Status = "Delivered" };

            // Act
            var result = await _trackingController.PostTracking(orderStatus);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Tracking>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var createdTracking = Assert.IsType<Tracking>(createdAtActionResult.Value);
            Assert.Equal(orderStatus.OrderId, createdTracking.OrderId);
            Assert.Equal(orderStatus.Status, createdTracking.Status);
            Assert.True(DateTime.UtcNow - createdTracking.UpdatedAt < TimeSpan.FromSeconds(1)); // Check if UpdatedAt is approximately current time
        }
    }
}
