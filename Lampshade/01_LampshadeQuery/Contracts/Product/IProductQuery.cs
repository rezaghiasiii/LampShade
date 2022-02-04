﻿using System.Collections.Generic;
using ShopManagement.Application.Contracts.Order;

namespace _01_LampshadeQuery.Contracts.Product
{
    public interface IProductQuery
    {
        List<ProductQueryModel> GetLatestArrivals();
        List<ProductQueryModel> Search(string value);
        ProductQueryModel GetProductDetailsBy(string slug);
        List<CartItem> CheckInventoryStatus(List<CartItem> cartItems);
    }
}