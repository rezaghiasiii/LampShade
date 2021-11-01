using System.Collections.Generic;
using _0_Framework.Infrastructure;

namespace ShopManagement.Infrastructure.Configuration.Permissions
{
    public class ShopPermissionExposer : IPermissionExposer
    {
        public Dictionary<string, List<PermissionDto>> Expose()
        {
            return new Dictionary<string, List<PermissionDto>>
            {
                {
                    "Product", new List<PermissionDto>

                    {
                        new PermissionDto(10, "ListProducts"),
                        new PermissionDto(11, "SearchProducts"),
                        new PermissionDto(12, "CreateProduct"),
                        new PermissionDto(13, "EditProduct"),
                    }

                },

                {
                    "ProductCategory", new List<PermissionDto>
                    {
                        new PermissionDto(20, "SearchProductCategory"),
                        new PermissionDto(21, "ListProductCategory"),
                        new PermissionDto(22, "CreateProductCategory"),
                        new PermissionDto(23, "EditProductCategory"),
                    }
                }
            };
        }
    }
}