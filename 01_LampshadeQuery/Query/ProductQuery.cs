using System;
using System.Collections.Generic;
using System.Linq;
using _0_Framework.Application;
using _01_LampshadeQuery.Contracts.Comment;
using _01_LampshadeQuery.Contracts.Product;
using CommentManagement.Infrastructure.EFCore;
using DiscountManagement.Infrastructure.EFCore;
using InventoryManagement.Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using ShopManagement.Application.Contracts.Order;
using ShopManagement.Domain.ProductPictureAgg;
using ShopManagement.Infrastructure.EFCore;

namespace _01_LampshadeQuery.Query
{
    public class ProductQuery : IProductQuery
    {
        private readonly ShopContext _context;
        private readonly InventoryContext _inventoryContext;
        private readonly DiscountContext _discountContext;
        private readonly CommentContext _commentContext;
        public ProductQuery(ShopContext context, InventoryContext inventoryContext, DiscountContext discountContext, CommentContext commentContext)
        {
            _context = context;
            _inventoryContext = inventoryContext;
            _discountContext = discountContext;
            _commentContext = commentContext;
        }

        public List<ProductQueryModel> GetLatestArrivals()
        {
            var inventory = _inventoryContext.Inventory.Select(x => new
            {
                x.ProductId,
                x.UnitPrice
            }).ToList();
            var discounts = _discountContext.CustomerDiscounts
                .Where(x => x.StartDate < DateTime.Now && x.EndDate > DateTime.Now)
                .Select(x => new { x.ProductId, x.DiscountRate, x.EndDate }).ToList();
            var products = _context.Products.Include(x => x.Category).Select(x => new ProductQueryModel()
            {
                Id = x.Id,
                Name = x.Name,
                Category = x.Category.Name,
                Picture = x.Picture,
                PictureAlt = x.PictureAlt,
                PictureTitle = x.PictureTitle,
                Slug = x.Slug
            }).AsNoTracking().OrderByDescending(x => x.Id).Take(6).ToList();

            foreach (var product in products)
            {
                var inventoryProduct = inventory.FirstOrDefault(x => x.ProductId == product.Id);
                if (inventoryProduct == null) continue;
                {
                    var price = inventoryProduct.UnitPrice;
                    product.Price = price.ToMoney();

                    var discount = discounts.FirstOrDefault(x => x.ProductId == product.Id);
                    if (discount == null) continue;
                    var discountRate = discount.DiscountRate;
                    product.DiscountRate = discountRate;
                    product.DiscountExpireDate = discount.EndDate.ToDiscountFormat();
                    product.HasDiscount = discountRate > 0;
                    var discountAmount = Math.Round((price * discountRate) / 100);
                    product.PriceWithDiscount = (price - discountAmount).ToMoney();
                }
            }

            return products;
        }

        public List<ProductQueryModel> Search(string value)
        {
            var inventory = _inventoryContext.Inventory.Select(x => new { x.ProductId, x.UnitPrice }).ToList();

            var discounts = _discountContext.CustomerDiscounts.Where(x => x.StartDate < DateTime.Now && x.EndDate > DateTime.Now).Select(x => new { x.DiscountRate, x.ProductId, x.EndDate }).ToList();

            var query = _context.Products.Include(x => x.Category).Select(product => new ProductQueryModel()
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category.Name,
                ShortDescription = product.ShortDescription,
                Picture = product.Picture,
                PictureAlt = product.PictureAlt,
                PictureTitle = product.PictureTitle,
                Slug = product.Slug
            }).AsNoTracking();

            if (!string.IsNullOrWhiteSpace(value))
                query = query.Where(x => x.Name.Contains(value) || x.ShortDescription.Contains(value));

            var products = query.OrderByDescending(x => x.Id).ToList();

            foreach (var product in products)
            {
                var inventoryProduct = inventory.FirstOrDefault(x => x.ProductId == product.Id);
                if (inventoryProduct == null) continue;
                {
                    var price = inventoryProduct.UnitPrice;
                    product.Price = price.ToMoney();
                    var discount = discounts.FirstOrDefault(x => x.ProductId == product.Id);
                    if (discount == null) continue;
                    var discountRate = discount.DiscountRate;
                    product.DiscountRate = discountRate;
                    product.DiscountExpireDate = discount.EndDate.ToDiscountFormat();
                    product.HasDiscount = discountRate > 0;
                    var discountAmount = Math.Round((price * discountRate) / 100);
                    product.PriceWithDiscount = (price - discountAmount).ToMoney();
                }
            }
            return products;
        }

        public ProductQueryModel GetProductDetailsBy(string slug)
        {
            var inventory = _inventoryContext.Inventory.Select(x => new
            {
                x.ProductId,
                x.UnitPrice,
                x.InStock
            }).ToList();


            var discounts = _discountContext.CustomerDiscounts
                .Where(x => x.StartDate < DateTime.Now && x.EndDate > DateTime.Now)
                .Select(x => new { x.ProductId, x.DiscountRate, x.EndDate }).ToList();
            var product = _context.Products.Include(x => x.Category).Include(x => x.ProductPictures).Select(x => new ProductQueryModel()
            {
                Id = x.Id,
                Name = x.Name,
                Category = x.Category.Name,
                Picture = x.Picture,
                PictureAlt = x.PictureAlt,
                PictureTitle = x.PictureTitle,
                Slug = x.Slug,
                ShortDescription = x.ShortDescription,
                CategorySlug = x.Category.Slug,
                Code = x.Code,
                Keywords = x.Keywords,
                MetaDescription = x.MetaDescription,
                Description = x.Description,
                Pictures = MapProductPictures(x.ProductPictures),
            }).AsNoTracking().FirstOrDefault(x => x.Slug == slug);

            if (product == null)
                return new ProductQueryModel();

            var inventoryProduct = inventory.FirstOrDefault(x => x.ProductId == product.Id);
            if (inventoryProduct != null)
            {
                product.IsInStock = inventoryProduct.InStock;
                var price = inventoryProduct.UnitPrice;
                product.Price = price.ToMoney();
                product.DoublePrice = price;
                var discount = discounts.FirstOrDefault(x => x.ProductId == product.Id);
                if (discount != null)
                {
                    var discountRate = discount.DiscountRate;
                    product.DiscountRate = discountRate;
                    product.HasDiscount = discountRate > 0;
                    product.DiscountExpireDate = discount.EndDate.ToDiscountFormat();
                    var discountAmount = Math.Round((price * discountRate) / 100);
                    product.PriceWithDiscount = (price - discountAmount).ToMoney();
                }

            }

            product.Comments = _commentContext.Comments.Where(x => x.Type == CommentType.Product && x.IsConfirmed && !x.IsCanceled && x.OwnerRecordId == product.Id).Select(x => new CommentQueryModel()
            {
                Id = x.Id,
                Name = x.Name,
                Message = x.Message
            }).OrderByDescending(x => x.Id).ToList();


            return product;
        }

        public List<CartItem> CheckInventoryStatus(List<CartItem> cartItems)
        {
            var inventory = _inventoryContext.Inventory.ToList();
            foreach (var cartItem in cartItems.Where(item => inventory.Any(x => x.ProductId == item.Id && x.InStock)))
            {
                var itemInventory = inventory.Find(x => x.ProductId == cartItem.Id);
                if (itemInventory != null) cartItem.IsInStock = itemInventory.CalculateCurrentCount() >= cartItem.Count;
            }

            return cartItems;
        }

        private static List<ProductPictureQueryModel> MapProductPictures(List<ProductPicture> pictures)
        {
            return pictures.Select(x => new ProductPictureQueryModel()
            {
                IsRemoved = x.IsRemoved,
                Picture = x.Picture,
                PictureAlt = x.PictureAlt,
                PictureTitle = x.PictureTitle,
                ProductId = x.ProductId
            }).Where(x => !x.IsRemoved).ToList();
        }
    }
}