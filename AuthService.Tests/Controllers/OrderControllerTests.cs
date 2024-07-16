using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using System.Threading.Tasks;
using Xunit;

namespace OrderService.Tests.Controllers
{
    public class OrderControllerTests
    {
        private readonly OrderController _orderController;
        private readonly OrderContext _context;

        public OrderControllerTests()
        {
            var options = new DbContextOptionsBuilder<OrderContext>()
                .UseInMemoryDatabase(databaseName: "OrderTestDb")
                .Options;
            _context = new OrderContext(options);
            _orderController = new OrderController(_context);
        }

        [Fact]
        public async Task CreateOrder_Should_Add_New_Order()
        {
            // Arrange
            var order = new Order
            {
                Brand = "Toyota",
                Model = "Corolla",
                ProdYear = 2020,
                PickupLocation = "Warsaw",
                DeliveryLocation = "Krakow",
                DeliveryDate = System.DateTime.Now.AddDays(1),
                Comments = "Handle with care"
            };

            // Act
            var result = await _orderController.CreateOrder(order);
            var createdOrder = await _context.Orders.FirstOrDefaultAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(order, okResult.Value);
            Assert.NotNull(createdOrder);
            Assert.Equal("Toyota", createdOrder.Brand);
            Assert.Equal("Pending", createdOrder.Status);
        }

        [Fact]
        public async Task GetOrder_Should_Return_Order_If_Exists()
        {
            // Arrange
            var order = new Order
            {
                Brand = "Toyota",
                Model = "Corolla",
                ProdYear = 2020,
                PickupLocation = "Warsaw",
                DeliveryLocation = "Krakow",
                DeliveryDate = System.DateTime.Now.AddDays(1),
                Comments = "Handle with care"
            };
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderController.GetOrder(order.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOrder = Assert.IsType<Order>(okResult.Value);
            Assert.Equal(order.Id, returnedOrder.Id);
        }

        [Fact]
        public async Task GetOrder_Should_Return_NotFound_If_Order_Does_Not_Exist()
        {
            // Act
            var result = await _orderController.GetOrder(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
