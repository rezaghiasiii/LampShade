using System;
using System.Collections.Generic;
using System.Linq;
using _0_Framework.Application;
using _01_LampshadeQuery.Contracts.Article;
using _01_LampshadeQuery.Contracts.Comment;
using _01_LampshadeQuery.Contracts.Product;
using BlogManagement.Infrastructure.EFCore;
using CommentManagement.Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;

namespace _01_LampshadeQuery.Query
{
    public class ArticleQuery : IArticleQuery
    {
        private readonly BlogContext _context;
        private readonly CommentContext _commentContext;
        public ArticleQuery(BlogContext context, CommentContext commentContext)
        {
            _context = context;
            _commentContext = commentContext;
        }

        public ArticleQueryModel GetArticleDetails(string slug)
        {
            var article = _context.Articles.Include(x => x.Category).Where(x => x.PublishDate <= DateTime.Now).Select(x =>
                 new ArticleQueryModel()
                 {
                     Id = x.Id,
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

            var comments = _commentContext.Comments.Include(x=>x.Parent).Where(x => x.Type == CommentType.Article && x.IsConfirmed && !x.IsCanceled && x.OwnerRecordId == article.Id).Select(x => new CommentQueryModel()
            {
                Id = x.Id,
                Name = x.Name,
                Message = x.Message,
                CreationDate = x.CreationDate.ToFarsi(),
                ParentId = x.ParentId
            }).OrderByDescending(x => x.Id).ToList();

            foreach (var comment in comments.Where(comment => comment.ParentId>0))
            {
                comment.ParentName = comments.FirstOrDefault(x => x.Id == comment.ParentId)?.Name;
            }

            article.Comments = comments;
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