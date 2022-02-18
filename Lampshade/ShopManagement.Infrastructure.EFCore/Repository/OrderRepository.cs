﻿using System.Collections.Generic;
using System.Linq;
using _0_Framework.Application;
using _0_Framework.Infrastructure;
using AccountManagement.Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using ShopManagement.Application.Contracts;
using ShopManagement.Application.Contracts.Order;
using ShopManagement.Domain.OrderAgg;

namespace ShopManagement.Infrastructure.EFCore.Repository
{
    public class OrderRepository : RepositoryBase<long,Order>,IOrderRepository
    {
        private readonly ShopContext _shopContext;
        private readonly AccountContext _accountContext;
        public OrderRepository(ShopContext context, ShopContext shopContext, AccountContext accountContext) : base(context)
        {
            _shopContext = shopContext;
            _accountContext = accountContext;
        }

        public double GetAmountBy(long id)
        {
            var order = _shopContext.Orders.Select(x => new { x.PayAmount, x.Id }).FirstOrDefault(x => x.Id == id);
            if (order != null)
                return order.PayAmount;
            return 0;
        }

        public List<OrderViewModel> Search(OrderSearchModel searchModel)
        {

            var accounts = _accountContext.Accounts.Select(x => new
            {
                x.Id, x.FullName
            }).ToList();
            var query = _shopContext.Orders.Select(x => new OrderViewModel()
            {
                Id = x.Id,
                AccountId = x.AccountId,
                DiscountAmount = x.DiscountAmount,
                IsCanceled = x.IsCanceled,
                IsPaid = x.IsPaid,
                IssueTrackingNo = x.IssueTrackingNo,
                PayAmount = x.PayAmount,
                PaymentMethodId = x.PaymentMethod,
                RefId = x.RefId,
                TotalAmount = x.TotalAmount,
                CreationDate = x.CreationDate.ToFarsi()
            });

            query = query.Where(x => x.IsCanceled == searchModel.IsCanceled);

            if (searchModel.AccountId > 0)
            {
                query = query.Where(x => x.AccountId == searchModel.AccountId);
            }

            var orders = query.OrderByDescending(x => x.Id).ToList();

            foreach (var order in orders)
            {
                order.AccountFullName = accounts.FirstOrDefault(x => x.Id == order.AccountId)?.FullName;
                order.PaymentMethod = PaymentMethod.GetBy(order.PaymentMethodId).Name;
            }

            return orders;
        }

        public List<OrderItemViewModel> GetItems(long orderId)
        {

            var products = _shopContext.Products.Select(x => new { x.Id, x.Name }).ToList();

            var order = _shopContext.Orders.FirstOrDefault(x => x.Id == orderId);
            if (order == null)
                return new List<OrderItemViewModel>();
            var items = order.Items.Select(x => new OrderItemViewModel()
            {
                Id = x.Id,
                Count = x.Count,
                DiscountRate = x.DiscountRate,
                OrderId = x.OrderId,
                ProductId = x.ProductId,
                UnitPrice = x.UnitPrice
            }).ToList();

            foreach (var item in items)
            {
                item.Product = products.FirstOrDefault(x => x.Id == item.ProductId)?.Name;
            }


            return items;
        }
    }
}