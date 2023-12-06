using Microsoft.Extensions.Logging;

namespace Infrastructure.Helpers
{
    public static class BankAccountGenerator
    {
        public static int Generate()
        {
            var random = new Random();

            int number = random.Next(1000000000, 2147483647);
            return number;
        }
    }
}
