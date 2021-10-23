using System.Collections.Generic;
using AccountManagement.Application.Contract.Account;
using AccountManagement.Application.Contract.Roles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ServiceHost.Areas.Administration.Pages.Account.Accounts
{
    public class IndexModel : PageModel
    {
        [TempData] public string Message { get; set; }
        public AccountSearchModel SearchModel { get; set; } 
        public List<AccountViewModel> Accounts { get; set; }
        public SelectList Roles;
        private readonly IRoleApplication _roleApplication;
        private readonly IAccountApplication _accountApplication;

        public IndexModel(IAccountApplication accountApplication, IRoleApplication roleApplication)
        {
            _accountApplication = accountApplication;
            _roleApplication = roleApplication;
        }


        public void OnGet(AccountSearchModel searchModel)
        {
           Accounts = _accountApplication.Search(searchModel);
           Roles = new SelectList(_roleApplication.List(), "Id", "Name");
        }

        public IActionResult OnGetCreate()
        {
            var command = new CreateAccount()
            {
                Roles = _roleApplication.List()
            };
            return Partial("./Create",command );
        }

        public JsonResult OnPostCreate(CreateAccount command)
        {
            var result = _accountApplication.Create(command);
            return new JsonResult(result);
        }

        public IActionResult OnGetEdit(long id)
        {
            var account = _accountApplication.GetDetails(id);
            account.Roles = _roleApplication.List();
            return Partial("./Edit",account);
        }
        public JsonResult OnPostEdit(EditAccount command)
        {
            var result = _accountApplication.Edit(command);
            return new JsonResult(result);
        }
        public IActionResult OnGetChangePassword(long id)
        {
            var command = new ChangePassword() {Id = id};
            return Partial("./ChangePassword", command);
        }
        public JsonResult OnPostChangePassword(ChangePassword command)
        {
            var result = _accountApplication.ChangePassword(command);
            return new JsonResult(result);
        }
    }
}
