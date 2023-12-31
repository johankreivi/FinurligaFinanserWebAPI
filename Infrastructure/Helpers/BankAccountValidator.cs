﻿namespace Infrastructure.Helpers
{
    public static class BankAccountValidator
    {
        public static bool ValidateBankAccountName(string bankAccountName)
        {
            if (bankAccountName == null) return false;
            if (bankAccountName.Length < 3) return false;
            if (bankAccountName.Length > 30) return false;
            if (char.IsWhiteSpace(bankAccountName[0])) return false;
            if (bankAccountName.Contains("  ")) return false;

            return true;
        }

        public static bool ValidateUserAccountId(int userAccountId)
        {
            if (userAccountId <= 0) return false;
            return true;
        }
    }
}
