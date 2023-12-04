using Microsoft.Extensions.Logging;

namespace Infrastructure.Helpers
{
    public static class BankAccountGenerator
    {
        public static int Generate()
        {
            int hashCode = Guid.NewGuid().GetHashCode();
            string hashCodeString = Math.Abs(hashCode).ToString();

            // Pad the string if it's shorter than 10 characters
            if (hashCodeString.Length < 10)
            {
                hashCodeString = hashCodeString.PadRight(10, '0');
            }
            
            if(int.Parse(hashCodeString.Substring(0, 10)) >= 2147483647)
            {
                Generate();
            }

            return int.Parse(hashCodeString.Substring(0, 10));
        }
    }
}
