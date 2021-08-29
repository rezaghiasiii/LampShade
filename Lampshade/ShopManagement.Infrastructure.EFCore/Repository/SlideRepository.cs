﻿using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using _0_Framework.Infrastructure;
using Microsoft.EntityFrameworkCore;
using ShopManagement.Application.Contracts.Slide;
using ShopManagement.Domain.SliderAgg;

namespace ShopManagement.Infrastructure.EFCore.Repository
{
    public class SlideRepository :RepositoryBase<long,Slide>, ISlideRepository
    {
        private readonly ShopContext _context;
        public SlideRepository(ShopContext context) : base(context)
        {
            _context = context;
        }

        public EditSlide GetDetails(long id)
        {
            return _context.Slides.Select(x => new EditSlide()
            {
                Id = x.Id,
                Picture = x.Picture,
                PictureAlt = x.PictureAlt,
                PictureTitle = x.PictureTitle,
                Heading = x.Heading,
                Title = x.Title,
                Text = x.Text,
                BtnText = x.BtnText,
                Link = x.Link
            }).FirstOrDefault(x=>x.Id==id);
        }

        public List<SlideViewModel> GetList()
        {
            return _context.Slides.Select(x => new SlideViewModel()
            {
                Id = x.Id,
                Picture = x.Picture,
                Title = x.Title,
                Heading = x.Heading,
                IsRemoved = x.IsRemoved,
                CreationDate = x.CreationDate.ToString(CultureInfo.InvariantCulture)
            }).OrderByDescending(x => x.Id).ToList();
        }
    }
}