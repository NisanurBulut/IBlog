﻿using Medusa.WebUI.ApiServices.Interfaces;
using Medusa.WebUI.Filters;
using Medusa.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
namespace Medusa.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogController : Controller
    {
        private readonly IBlogApiService _blogApiService;

        public BlogController(IBlogApiService blogApiService)
        {
            this._blogApiService = blogApiService;
        }
        [JwtAuthorize]
        public async Task<IActionResult> Index()
        {
            return View(await _blogApiService.GetAllAsync());
        }
        [HttpGet]
        public IActionResult CreateBlog()
        {
            return View(new BlogAddModel());
        }
        [HttpPost]
        public async Task<IActionResult> CreateBlog(BlogAddModel item)
        {
            if (ModelState.IsValid)
            {
                await _blogApiService.AddAsync(item);
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> UpdateBlog(int id)
        {
            var blog = await _blogApiService.GetByIdAsync(id);
            var blogUpdate = new BlogUpdateModel
            {
                Id= blog.Id,
                Title = blog.Title,
                LongDescription=blog.LongDescription,
                ShortDescription = blog.ShortDescription
            };
            return View(blogUpdate);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateBlog(BlogUpdateModel model)
        {
            if (ModelState.IsValid)
            {
                await _blogApiService.UpdateAsync(model);
                return RedirectToAction("Index");
            }
            return View();
        }
        
        public async Task<IActionResult> DeleteBlog(int id)
        {
            await _blogApiService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}