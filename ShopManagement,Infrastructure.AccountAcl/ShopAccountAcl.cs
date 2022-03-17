using System;
using AccountManagement.Application.Contract.Account;
using ShopManagement.Domain.Service;

namespace ShopManagement_Infrastructure.AccountAcl
{
    public class ShopAccountAcl : IShopAccountAcl
    {
        private readonly IAccountApplication _accountApplication;

        public ShopAccountAcl(IAccountApplication accountApplication)
        {
            _accountApplication = accountApplication;
        }

        public (string name, string mobile) GetAccountBy(long id)
        {
            var account = _accountApplication.GetAccountBy(id);
            return (account.FullName, account.Mobile);
        }
    }
}
