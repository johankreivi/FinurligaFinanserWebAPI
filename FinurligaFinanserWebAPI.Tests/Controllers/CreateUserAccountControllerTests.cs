using Entity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Helpers;

namespace FinurligaFinanserWebAPI.Tests.Controllers
{
    [TestFixture]
    public class CreateUserAccountControllerTests
    {
        private UserAccount _userAccount;
        private byte[] _passwordSalt;
        private string _passwordHash;

        [SetUp]
        public void Setup()
        {
            
           // _passwordSalt = GenerateSalt(); // Ersätt med en faktisk metod för att generera salt.
            //_passwordHash = HashPassword("somePassword", _passwordSalt); // Ersätt med en faktisk metod för att hasha lösenordet.

            _userAccount = new UserAccount("HasseAro", "Hasse", "Aro", _passwordSalt, _passwordHash);
        }       
        
    }
}
