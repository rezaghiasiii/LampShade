﻿namespace ShopManagement.Domain.Service
{
    public interface IShopAccountAcl
    {
        (string name, string mobile) GetAccountBy(long id);
    }
}