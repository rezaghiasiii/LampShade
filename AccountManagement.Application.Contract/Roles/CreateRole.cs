using System.Collections.Generic;
using _0_Framework.Infrastructure;

namespace AccountManagement.Application.Contract.Roles
{
    public class CreateRole
    {
        public string Name { get; set; }
        public List<int> Permissions { get; set; }
    }
}