using Entity;
using Infrastructure.Repositories;
using Moq;
using FinurligaFinanserWebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using FinurligaFinanserWebAPI.DtoModels.UserAccountDTOs;
using Infrastructure.Enums;
using Microsoft.AspNetCore.Http;

namespace FinurligaFinanserWebAPI.Tests.Controllers
{
    public class UserAccountControllerTests
    {        
        private UserAccountDTO _userAccountDto;

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

            _sut = new UserAccountController( _mockRepository.Object, _mockLogger.Object, _mockMapper.Object);

            _userAccountDto = new UserAccountDTO
            {
                UserName = "HasseAro",
                FirstName = "Hasse",
                LastName = "Aro",
                Password = "Efterlyst1!"
            };
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(2)]
        [TestCase(4)]
        public async Task GetAll_ShouldReturnOkResultAndListOfUserAccount(int take)
        {
            var userAccounts = take > 0 ? SeedUserAccounts(take) : new List<UserAccount>();

            _mockRepository.Setup(x => x.GetAllUserAccountsAsync(take)).ReturnsAsync(userAccounts);

            var result = await _sut.GetAll(take);

            Assert.That(result.Result, Is.InstanceOf(typeof(OkObjectResult)));

            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value, Is.Not.Null);
            Assert.That(okResult.Value, Is.InstanceOf(typeof(List<UserAccount>)));

            var resultValue = (List<UserAccount>)okResult.Value;
            Assert.Multiple(() =>
            {
                Assert.That(resultValue, Is.Not.Null);
                if (resultValue is not null) Assert.That(userAccounts, Has.Count.EqualTo(resultValue.Count));
            });
        }

        [Test]
        [TestCase("", "ValidPassword1!", typeof(BadRequestObjectResult))]
        [TestCase(null, "ValidPassword1!", typeof(BadRequestObjectResult))]
        [TestCase("ValidUsername", "", typeof(BadRequestObjectResult))]
        [TestCase("ValidUsername", null, typeof(BadRequestObjectResult))]
        public async Task Login_UserNameOrPasswordIsNullOrEmpty_ReturnBadRequest(string userName, string password, Type expectedType)
        {
            var testLoginUser = new LoginUserDTO()
            {
                UserName = userName,
                Password = password
            };

            var result = await _sut.Login(testLoginUser);
            Assert.That(result.Result, Is.InstanceOf(expectedType));
        }

        [Test]
        [TestCase("ValidUsername", "ValidPassword1!", typeof(OkObjectResult), true)]
        [TestCase("ValidUsername", "ValidPassword1!", typeof(UnauthorizedObjectResult), false)]
        public async Task Login_GetsAuthorized_ReturnOk_Else_ReturnUnauthorized(string userName, string password, Type exepectedType, bool isAuthorized) 
        {
            _mockRepository.Setup(x => x.AuthorizeUserLogin(userName, password)).ReturnsAsync(isAuthorized);

            LoginUserDTO loginDto = new() { UserName = userName, Password = password };

            var result = await _sut.Login(loginDto);

            Assert.That(result.Result, Is.InstanceOf(exepectedType));
        }

        private static List<UserAccount> SeedUserAccounts(int count)
        {
            var userAccounts = new List<UserAccount>();

            for (int i = 0; i < count; i++)
            {
                userAccounts.Add(new UserAccount($"UserAccount{i}", $"FirstName{i}", $"LastName{i}", new byte[32], $"hash{i}"));
            }

            return userAccounts;
        }

        //////////
        ///

        [Test]
        public async Task CreateUserAccount_ShouldReturnConfirmation_WhenUserIsCreated()
        {
            // Arrange
            var userAccount = new UserAccount("HasseAro", "Hasse", "Aro", Array.Empty<byte>(), "");
            var confirmationDto = new UserAccountConfirmationDTO { Id = 1, UserName = "HasseAro", Message = "Account created" };

            _mockRepository.Setup(repo => repo.CreateUserAccount(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((userAccount, UserValidationStatus.Valid));

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
            var userAccont = new UserAccount(_userAccountDto.UserName, _userAccountDto.FirstName, _userAccountDto.LastName, Array.Empty<byte>(), "");
            _mockRepository.Setup(x => x.CreateUserAccount(_userAccountDto.UserName, _userAccountDto.FirstName, _userAccountDto.LastName, _userAccountDto.Password))
                           .ReturnsAsync((userAccont, UserValidationStatus.NotValid_UserName_Already_Taken));

            // Act
            var result = await _sut.CreateUserAccount(_userAccountDto);

            // Assert
            Assert.That(result.Result, Is.InstanceOf(typeof(BadRequestObjectResult)));

            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.That(badRequestResult, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(badRequestResult.Value, Is.InstanceOf(typeof(string)));
                Assert.That(badRequestResult.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
            });
        }


        [Test]
        public async Task CreateUserAccount_ShouldReturnInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _mockRepository.Setup(x => x.CreateUserAccount(_userAccountDto.UserName, _userAccountDto.FirstName, _userAccountDto.LastName, _userAccountDto.Password)).ThrowsAsync(new Exception());

            // Act
            var result = await _sut.CreateUserAccount(_userAccountDto);


            // Assert
            Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
            var objectResult = result.Result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task GetOneUser_ReturnsOk_WhenUserExists()
        {
            // Arrange
            var fakeUser = new UserAccount("HasseAro", "Hasse", "Aro", Array.Empty<byte>(), "");
            _mockRepository.Setup(repo => repo.GetOneUser(It.IsAny<int>())).ReturnsAsync(fakeUser);
            var dto = new UserAccountConfirmationDTO();
            _mockMapper.Setup(mapper => mapper.Map<UserAccountConfirmationDTO>(fakeUser)).Returns(dto);

            // Act
            var result = await _sut.GetOneUser(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(okResult.StatusCode, Is.EqualTo(200));
                Assert.That(okResult.Value, Is.InstanceOf<UserAccountConfirmationDTO>());
            });
        }

        [Test]
        public async Task GetOneUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetOneUser(It.IsAny<int>())).ReturnsAsync((UserAccount)null!);

            // Act
            var result = await _sut.GetOneUser(1);

            // Assert
            var notFoundResult = result.Result as NotFoundResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task GetOneUser_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetOneUser(It.IsAny<int>())).ThrowsAsync(new Exception());

            // Act
            var result = await _sut.GetOneUser(1);

            // Assert
            var objectResult = result.Result as ObjectResult;
            Assert.That(objectResult, Is.Not.Null);
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task GetUserDetails_ReturnsOk_WhenUserExists()
        {
            // Arrange
            var fakeUser = new UserAccount("HasseAro", "Hasse", "Aro", Array.Empty<byte>(), "") {Id = 1 };
            _mockRepository.Setup(repo => repo.GetUserDetails(It.IsAny<int>())).ReturnsAsync(fakeUser);
            _mockMapper.Setup(mapper => mapper.Map<UserAccountDetailsDTO>(fakeUser)).Returns(new UserAccountDetailsDTO{
                Id = 1, FirstName = "Hasse", LastName = "Aro"});

            // Act
            var result = await _sut.GetUserDetails(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(okResult.StatusCode, Is.EqualTo(200));
                Assert.That(okResult.Value, Is.InstanceOf<UserAccountDetailsDTO>());
            });
        }

        [Test]
        public async Task GetUserAccountByBankAccountNumber_ReturnsOk_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var fakeUser = new UserAccount("HasseAro", "Hasse", "Aro", Array.Empty<byte>(), "") { Id = userId };
            var bankAccountNumber = 123456789;
            var fakeBankAccount = new BankAccount(bankAccountNumber, "Privatkonto", fakeUser.Id);
            _mockRepository.Setup(repo => repo.GetUserAccountByBankAccountNumber(It.IsAny<int>())).ReturnsAsync(userId);
            _mockRepository.Setup(x => x.GetUserDetails(userId)).ReturnsAsync(fakeUser);
            _mockMapper.Setup(mapper => mapper.Map<UserAccountDetailsDTO>(fakeUser)).Returns(new UserAccountDetailsDTO
            {
                Id = 1,
                FirstName = "Hasse",
                LastName = "Aro"
            });

            // Act
            var result = await _sut.GetUserAccountByBankAccountNumber(bankAccountNumber);

            // Assert
            Assert.That(result.Result, Is.InstanceOf(typeof(OkObjectResult)));

            var okResult = result.Result as OkObjectResult;

            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value, Is.InstanceOf(typeof(UserAccountDetailsDTO)));

            var resultValue = (UserAccountDetailsDTO)okResult.Value;

            Assert.That(resultValue, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(resultValue.Id, Is.EqualTo(userId));
                Assert.That(resultValue.FirstName, Is.EqualTo(fakeUser.FirstName));
                Assert.That(resultValue.LastName, Is.EqualTo(fakeUser.LastName));
            });
        }

        [Test]
        public async Task GetUserAccountByBankAccountNumber_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var bankAccountNumber = 123456789;
            _mockRepository.Setup(repo => repo.GetUserAccountByBankAccountNumber(It.IsAny<int>())).ReturnsAsync(0);

            // Act
            var result = await _sut.GetUserAccountByBankAccountNumber(bankAccountNumber);

            // Assert
            Assert.That(result.Result, Is.InstanceOf(typeof(NotFoundObjectResult)));

            var notFoundResult = result.Result as NotFoundObjectResult;
            Assert.That(notFoundResult, Is.Not.Null);
            Assert.That(notFoundResult.Value, Is.EqualTo("Hittade inget user account som matchade bankkontonumret"));
        }
    }

}
