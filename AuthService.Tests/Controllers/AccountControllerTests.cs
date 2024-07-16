using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using AuthService.Controllers;
using AuthService.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace AuthService.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AccountController _accountController;

        public AccountControllerTests()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var userClaimsPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(_userManagerMock.Object, contextAccessorMock.Object, userClaimsPrincipalFactoryMock.Object, null, null, null, null);

            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(config => config["Jwt:Key"]).Returns("verySecretKey12345");
            _configurationMock.Setup(config => config["Jwt:Issuer"]).Returns("https://localhost");

            _accountController = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task Register_Should_Return_Ok_When_User_Is_Successfully_Created()
        {
            // Arrange
            var model = new RegisterModel { Email = "test@example.com", Password = "Password123!" };
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _accountController.Register(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User registered successfully", ((dynamic)okResult.Value).Message);
        }

        [Fact]
        public async Task Register_Should_Return_BadRequest_When_User_Creation_Fails()
        {
            // Arrange
            var model = new RegisterModel { Email = "test@example.com", Password = "Password123!" };
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "User creation failed" }));

            // Act
            var result = await _accountController.Register(model);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorList = Assert.IsType<IdentityError[]>(badRequestResult.Value);
            Assert.Contains(errorList, error => error.Description == "User creation failed");
        }

        [Fact]
        public async Task Login_Should_Return_Ok_With_Token_When_Login_Is_Successful()
        {
            // Arrange
            var model = new LoginModel { Email = "test@example.com", Password = "Password123!" };
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };

            _signInManagerMock.Setup(x => x.PasswordSignInAsync(model.Email, model.Password, false, false)).ReturnsAsync(SignInResult.Success);
            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email)).ReturnsAsync(user);

            // Act
            var result = await _accountController.Login(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(((dynamic)okResult.Value).Token);
        }

        [Fact]
        public async Task Login_Should_Return_Unauthorized_When_Login_Fails()
        {
            // Arrange
            var model = new LoginModel { Email = "test@example.com", Password = "Password123!" };
            _signInManagerMock.Setup(x => x.PasswordSignInAsync(model.Email, model.Password, false, false)).ReturnsAsync(SignInResult.Failed);

            // Act
            var result = await _accountController.Login(model);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }
    }
}
