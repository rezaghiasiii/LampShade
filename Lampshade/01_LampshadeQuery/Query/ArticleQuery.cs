using System;
using System.Collections.Generic;
using System.Linq;
using _0_Framework.Application;
using _01_LampshadeQuery.Contracts.Article;
using BlogManagement.Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;

namespace _01_LampshadeQuery.Query
{
    public class ArticleQuery : IArticleQuery
    {
        private readonly BlogContext _context;

        public ArticleQuery(BlogContext context)
        {
            _context = context;
        }

        public ArticleQueryModel GetArticleDetails(string slug)
        {
            var article = _context.Articles.Include(x => x.Category).Where(x => x.PublishDate <= DateTime.Now).Select(x =>
                 new ArticleQueryModel()
                 {
                     CategoryName = x.Category.Name,
                     Picture = x.Picture,
                     PictureTitle = x.PictureTitle,
                     PictureAlt = x.PictureAlt,
                     Slug = x.Slug,
                     Description = x.Description,
                     ShortDescription = x.ShortDescription,
                     Keywords = x.Keywords,
                     MetaDescription = x.MetaDescription,
                     CategorySlug = x.Category.Slug,
                     CanonicalAddress = x.CanonicalAddress,
                     PublishDate = x.PublishDate.ToFarsi(),
                     Title = x.Title,
                 }).FirstOrDefault(x => x.Slug == slug);
            if (!string.IsNullOrWhiteSpace(article.Keywords))
                article.KeywordList = article.Keywords.Split(",").ToList();

            return article;
        }

        public List<ArticleQueryModel> LatestArticles()
        {
            return _context.Articles.Include(x => x.Category).Where(x => x.PublishDate <= DateTime.Now).Select(x =>
                new ArticleQueryModel()
                {

                    Title = x.Title,
                    Picture = x.Picture,
                    PictureTitle = x.PictureTitle,
                    PictureAlt = x.PictureAlt,
                    Slug = x.Slug,
                    PublishDate = x.PublishDate.ToFarsi(),
                    ShortDescription = x.ShortDescription,
                }).ToList();
        }
    }
}