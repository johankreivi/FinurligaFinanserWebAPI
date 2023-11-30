
using AutoMapper;
using FinurligaFinanserWebAPI.Controllers;
using FinurligaFinanserWebAPI.DtoModels.BankAccountDTOs;
using Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace FinurligaFinanserWebAPI.Tests.Controllers
{
    [TestFixture]
    public class BankAccountControllerTests
    {
        private Mock<IBankAccountRepository> _bankAccountRepositoryMock;
        private Mock<ILogger<BankAccountController>> _mockLogger;
        private Mock<IMapper> _mockMapper;

        private BankAccountController _sut;

        [SetUp]
        public void Setup()
        {
            _bankAccountRepositoryMock = new Mock<IBankAccountRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<BankAccountController>>();
            _sut = new BankAccountController( _bankAccountRepositoryMock.Object, _mockLogger.Object, _mockMapper.Object );

        }
       
    }
}
