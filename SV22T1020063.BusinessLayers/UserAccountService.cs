using SV22T1020063.DataLayers.Interfaces;
using SV22T1020063.DataLayers.SQLServer;
using SV22T1020063.Models.Security;

namespace SV22T1020063.BusinessLayers
{
    public enum AccountTypes
    {
        Employee,
        Customer
    }

    public static class UserAccountService
    {
        private static readonly IUserAccountRepository employeeAccountDB;
        private static readonly IUserAccountRepository customerAccountDB;

        static UserAccountService()
        {
            employeeAccountDB = new EmployeeAccountRepository(Configuration.ConnectionString);
            customerAccountDB = new CustomerAccountRepository(Configuration.ConnectionString);
        }

        public static async Task<UserAccount?> AuthorizeAsync(AccountTypes accountType, string userName, string password)
        {
            if (accountType == AccountTypes.Employee)
                return await employeeAccountDB.Authorize(userName, password);
            else
                return await customerAccountDB.Authorize(userName, password);
        }

        public static async Task<bool> ChangePasswordAsync(AccountTypes accountType, string userName, string password)
        {
            if (accountType == AccountTypes.Employee)
                return await employeeAccountDB.ChangePassword(userName, password);
            else
                return await customerAccountDB.ChangePassword(userName, password);
        }
    }
}
