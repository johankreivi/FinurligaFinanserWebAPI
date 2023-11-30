namespace Infrastructure.Helpers
{
    public static class BankAccountGenerator
    {
        public static int Generate() => int.Parse(Math.Abs(Guid.NewGuid().GetHashCode()).ToString().Substring(0, 10));       
    }
}
