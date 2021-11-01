using System.Collections.Generic;
using _0_Framework.Infrastructure;

namespace AccountManagement.Application.Contract.Roles
{
    public class EditRole :CreateRole
    {
        public long Id { get; set; }
        public List<PermissionDto> MapedPermissions { get; set; }
    }
}