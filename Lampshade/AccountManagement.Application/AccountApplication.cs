﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using _0_Framework.Application;
using AccountManagement.Application.Contract.Account;
using AccountManagement.Domain.AccountAgg;

namespace AccountManagement.Application
{
    public class AccountApplication : IAccountApplication
    {
        private readonly IFileUploader _fileUploader;
        private readonly IAccountRepository _accountRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuthHelper _authHelper;
        public AccountApplication(IAccountRepository accountRepository, IPasswordHasher passwordHasher, IFileUploader fileUploader, IAuthHelper authHelper)
        {
            _accountRepository = accountRepository;
            _passwordHasher = passwordHasher;
            _fileUploader = fileUploader;
            _authHelper = authHelper;
        }

        public OperationResult Create(CreateAccount command)
        {
            var operation = new OperationResult();

            if (_accountRepository.Exists(x => x.Username == command.Username || x.Mobile == command.Mobile))
                return operation.Failed(ApplicationMessages.DuplicatedRecord);
            var path = $"profilePhotos";
            var picturePath = _fileUploader.Upload(command.ProfilePhoto, path);
            var password = _passwordHasher.Hash(command.Password);
            var account = new Account(command.FullName, command.Username, password, command.Mobile, command.RoleId,
                picturePath);
            _accountRepository.Create(account);
            _accountRepository.SaveChanges();
            return operation.Succeeded();
        }

        public OperationResult Edit(EditAccount command)
        {
            var operation = new OperationResult();
            var account = _accountRepository.Get(command.Id);
            if (account == null)
                return operation.Failed(ApplicationMessages.RecordNotFound);

            if (_accountRepository.Exists(x => (x.Username == command.Username || x.Mobile == command.Mobile) && x.Id != command.Id))
                return operation.Failed(ApplicationMessages.DuplicatedRecord);
            var path = $"profilePhotos";
            var picturePath = _fileUploader.Upload(command.ProfilePhoto, path);
            account.Edit(command.FullName, command.Username, command.Mobile ,command.RoleId,
                picturePath);
            _accountRepository.SaveChanges();
            return operation.Succeeded();
        }

        public OperationResult ChangePassword(ChangePassword command)
        {
            var operation = new OperationResult();
            var account = _accountRepository.Get(command.Id);
            if (account == null)
                operation.Failed(ApplicationMessages.RecordNotFound);

            if (command.Password != command.RePassword)
                return operation.Failed(ApplicationMessages.PasswordNotMatch);

            var password = _passwordHasher.Hash(command.Password);

            account.ChangePassword(password);
            _accountRepository.SaveChanges();
            return operation.Succeeded();

        }

        public OperationResult Login(Login command)
        {
            var operation = new OperationResult();
            var account = _accountRepository.GetBy(command.UserName);
            if (account == null)
                return operation.Failed(ApplicationMessages.WrongUserPass);

            (bool verified, bool needsUpgrade) result = _passwordHasher.Check(account.Password, command.Password);
            if(result.verified)
                return operation.Failed(ApplicationMessages.WrongUserPass);

            var authViewModel = new AuthViewModel(account.Id,
                account.RoleId, account.FullName,
                account.Username);
            
            _authHelper.Signin(authViewModel);
            return operation.Succeeded();

        }

        public List<AccountViewModel> Search(AccountSearchModel searchModel)
        {
            return _accountRepository.Search(searchModel);
        }

        public EditAccount GetDetails(long id)
        {
            return _accountRepository.GetDetails(id);
        }

        public void Logout()
        {
            _authHelper.SignOut();
        }
    }
}
