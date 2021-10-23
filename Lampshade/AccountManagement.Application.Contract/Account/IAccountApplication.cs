﻿using System.Collections.Generic;
using _0_Framework.Application;

namespace AccountManagement.Application.Contract.Account
{
    public interface IAccountApplication
    {
        OperationResult Create(CreateAccount command);
        OperationResult Edit(EditAccount command);
        OperationResult ChangePassword(ChangePassword command);
        OperationResult Login(Login command);
        List<AccountViewModel> Search(AccountSearchModel searchModel);
        EditAccount GetDetails(long id);
        void Logout();
    }
}