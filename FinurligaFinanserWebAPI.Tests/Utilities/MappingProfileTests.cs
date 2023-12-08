using AutoMapper;
using Entity;
using FinurligaFinanserWebAPI.DtoModels.BankAccountDTOs;
using FinurligaFinanserWebAPI.DtoModels.TransactionDTOs;
using FinurligaFinanserWebAPI.DtoModels.UserAccountDTOs;
using FinurligaFinanserWebAPI.Utilities;

namespace FinurligaFinanserWebAPI.Tests.Utilities
{
    [TestFixture]
    public class MappingProfileTests
    {
        private IMapper _mapper;
        [SetUp]
        public void Setup()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            _mapper = configuration.CreateMapper();
        }
        [Test]
        public void UserAccount_To_UserAccountConfirmationDTO()
        {
            string userName = "TestUser";
            string firstName = "John";
            string lastName = "Doe";
            byte[] passwordSalt = {  };
            string passwordHash = "someHashedPassword";

            var userAccount = new UserAccount(userName, firstName, lastName, passwordSalt, passwordHash);

            var confirmationDTO = _mapper.Map<UserAccountConfirmationDTO>(userAccount);

            Assert.That(confirmationDTO.Message, Is.EqualTo("User account TestUser successfully created."));
        }
        [Test]
        public void LoginUserDTO_To_UserAccount()
        {
            var loginUserDTO = new LoginUserDTO
            {
                UserName = "testuser",
                Password = "testpassword"
            };

            var userAccount = _mapper.Map<UserAccount>(loginUserDTO);

            Assert.That(userAccount.UserName, Is.EqualTo(loginUserDTO.UserName));
            Assert.IsNotNull(userAccount.PasswordSalt);
            Assert.IsNotNull(userAccount.PasswordHash);
            Assert.That(userAccount.FirstName, Is.EqualTo(string.Empty));
            Assert.That(userAccount.LastName, Is.EqualTo(string.Empty));
        }
        [Test]
        public void UserAccount_To_UserAccountDetailsDTO()
        {
            string userName = "TestUser";
            string firstName = "John";
            string lastName = "Doe";
            byte[] passwordSalt = { };
            string passwordHash = "someHashedPassword";

            var userAccount = new UserAccount(userName, firstName, lastName, passwordSalt, passwordHash);

            var detailsDTO = _mapper.Map<UserAccountDetailsDTO>(userAccount);

            Assert.That(detailsDTO.Id, Is.EqualTo(userAccount.Id));
            Assert.That(detailsDTO.FirstName, Is.EqualTo(userAccount.FirstName));
            Assert.That(detailsDTO.LastName, Is.EqualTo(userAccount.LastName));
        }
        [Test]
        public void Transaction_To_DepositConfirmationDTO()
        {
            var transaction = new Transaction(123321, 133123, 100, "asd");           

            // Act
            var depositConfirmationDTO = _mapper.Map<DepositConfirmationDTO>(transaction);

            // Assert
            Assert.That(depositConfirmationDTO.ReceivingAccountNumber, Is.EqualTo(transaction.ReceivingAccountNumber));
            Assert.That(depositConfirmationDTO.Amount, Is.EqualTo(transaction.Amount));
        }


    }
}
