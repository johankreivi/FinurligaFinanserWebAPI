using Entity;
using FinurligaFinanserWebAPI.DtoModels;
using FinurligaFinanserWebAPI.Controllers;
using Infrastructure.Repositories;
using Moq;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using static Infrastructure.Repositories.UserAccountRepository;

namespace FinurligaFinanserWebAPI.Tests.Controllers
{
    [TestFixture]
    public class CreateUserAccountControllerTests
    {
        private UserAccountDto _userAccountDto;                

        private Mock<IUserAccountRepository> _mockRepository;
        private Mock<ILogger<UserAccountController>> _mockLogger;
        private Mock<IMapper> _mockMapper;

        private UserAccountController _sut;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IUserAccountRepository>();
            _mockLogger = new Mock<ILogger<UserAccountController>>(); 
            _mockMapper = new Mock<IMapper>();

            _sut = new UserAccountController(_mockRepository.Object, _mockLogger.Object, _mockMapper.Object);

            _userAccountDto = new UserAccountDto
            {
                UserName = "HasseAro",
                FirstName = "Hasse",
                LastName = "Aro",
                Password = "Efterlyst1!"
            };
        }

        [Test]
        public async Task CreateUserAccount_ShouldReturnConfirmation_WhenUserIsCreated()
        {
            // Arrange
            var userAccount = new UserAccount("HasseAro", "Hasse", "Aro", Array.Empty<byte>(), "");
            var confirmationDto = new UserAccountConfirmationDTO { Id = 1, UserName = "HasseAro", Message = "Account created" };

            _mockRepository.Setup(repo => repo.CreateUserAccount(It.IsAny<UserAccount>()))
                .ReturnsAsync(userAccount);

            _mockMapper.Setup(mapper => mapper.Map<UserAccountConfirmationDTO>(It.IsAny<UserAccount>()))
                .Returns(confirmationDto);

            // Act
            var result = await _sut.CreateUserAccount(_userAccountDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<ActionResult<UserAccountConfirmationDTO>>());

            var createdAtActionResult = result.Result as CreatedAtActionResult;
            Assert.That(createdAtActionResult, Is.Not.Null);

            var returnValue = createdAtActionResult.Value as UserAccountConfirmationDTO;
            Assert.That(returnValue, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(returnValue.UserName, Is.EqualTo("HasseAro"));
                Assert.That(createdAtActionResult.StatusCode, Is.EqualTo(StatusCodes.Status201Created));
            });
        }

        [Test]
        public async Task CreateUserAccount_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _sut.ModelState.AddModelError("UserName", "Username is required"); 

            // Act
            var result = await _sut.CreateUserAccount(_userAccountDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            var actionResult = result.Result as BadRequestObjectResult; 
            Assert.That(actionResult, Is.Not.Null);
            Assert.That(actionResult.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));

        }

        [Test]
        public async Task CreateUserAccount_ShouldReturnBadRequest_WhenUsernameAlreadyExists()
        {
            // Arrange
            string expectedErrorMessage = "Användarnamnet är upptaget. Välj ett annat namn.";
            _mockRepository.Setup(repo => repo.CreateUserAccount(It.IsAny<UserAccount>()))
                .ThrowsAsync(new UserNameAlreadyExistsException(expectedErrorMessage));

            // Act
            var result = await _sut.CreateUserAccount(_userAccountDto);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(badRequestResult.Value, Is.EqualTo(expectedErrorMessage));
                Assert.That(badRequestResult.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            });
        }
    }
}
