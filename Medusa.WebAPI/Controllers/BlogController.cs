﻿using AutoMapper;
using Medusa.Business.Interface;
using Medusa.Business.Tools;
using Medusa.DataTransferObject;
using Medusa.Entities;
using Medusa.WebAPI.CustomFilters;
using Medusa.WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Medusa.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : BaseController
    {
        private readonly IBlogService _blogService;
        private readonly ICommentService _commentService;
        private readonly IMapper _mapper;
        private readonly IFacadeTool _facadeTool;
        public BlogController(IBlogService blogService, ICommentService commentService, IMapper mapper, IFacadeTool facadeTool)
        {
            this._mapper = mapper;
            this._blogService = blogService;
            _commentService = commentService;
            _facadeTool = facadeTool;
        }
        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> GetAllBlogs()
        {
            if (_facadeTool.MemoryCache.TryGetValue("blogList", out List<BlogDto> list))
            {
                return Ok(list);
            }
            var blogList = _mapper.Map<List<BlogDto>>(await _blogService.GetAllSortedByPostedTimeAsync());
            _facadeTool.MemoryCache.Set("blogList", blogList, new MemoryCacheEntryOptions()
            {
                AbsoluteExpiration = DateTime.Now.AddDays(1),
                Priority=CacheItemPriority.Normal
            });
            return Ok(blogList);
        }
        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string s)
        {
            return Ok(_mapper.Map<List<BlogDto>>(await _blogService.SearchBlogAsync(s)));
        }

        [HttpGet("[action]")]
        [ServiceFilter(typeof(ValidIdModel<BlogEntity>))]
        public async Task<IActionResult> GetBlogById(int id)
        {
            return Ok(_mapper.Map<BlogEntity, BlogDto>(await _blogService.FindByIdAsync(id)));
        }

        [HttpPost("[action]")]
        [Authorize]
        [ValidModel]
        public async Task<IActionResult> CreateBlog([FromForm] BlogAddModel model)
        {
            var uploadModel = await UploadFile(model.Image);
            if (uploadModel.UploadState == Enums.UploadState.success)
            {
                await _blogService.AddAsync(_mapper.Map<BlogAddModel, BlogEntity>(model));
                return Created("", model);
            }
            else if (uploadModel.UploadState == Enums.UploadState.notexists)
            {
                await _blogService.AddAsync(_mapper.Map<BlogAddModel, BlogEntity>(model));
                return Created("", model);
            }
            else
            {
                return BadRequest(uploadModel.ErrorMessage);
            }
        }


        [HttpPut("[action]")]
        [Authorize]
        [ValidModel]
        public async Task<IActionResult> UpdateBlog([FromForm] BlogUpdateModel model)
        {
            var uploadModel = await UploadFile(model.Image);

            if (uploadModel.UploadState == Enums.UploadState.success)
            {
                model.ImagePath = uploadModel.NewName;
                await _blogService.UpdateAsync(_mapper.Map<BlogUpdateModel, BlogEntity>(model));
                return NoContent();
            }
            else if (uploadModel.UploadState == Enums.UploadState.notexists)
            {
                await _blogService.UpdateAsync(_mapper.Map<BlogUpdateModel, BlogEntity>(model));
                return NoContent();
            }
            else
            {
                return BadRequest(uploadModel.ErrorMessage);
            }
        }

        [HttpDelete("[action]")]
        [Authorize]
        [ServiceFilter(typeof(ValidIdModel<BlogEntity>))]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            await _blogService.RemoveAsync(await _blogService.FindByIdAsync(id));
            return NoContent();
        }

        [HttpPost("[action]")]
        [ValidModel]
        public async Task<IActionResult> AddToCategory(CategoryBlogDto model)
        {
            await _blogService.AddToCategoryAsync(model);
            return Created("", model);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RemoveFromCategory(CategoryBlogDto model)
        {
            await _blogService.RemoveFromCategoryAsync(model);
            return NoContent();
        }
        [HttpGet("[action]")]
        [ServiceFilter(typeof(ValidIdModel<CategoryEntity>))]
        public async Task<IActionResult> GetAllByCategoryId(int id)
        {
            return Ok(await _blogService.GetAllByCategoryIdAsync(id));
        }
        [HttpGet("[action]")]
        [ServiceFilter(typeof(ValidIdModel<BlogEntity>))]
        public async Task<IActionResult> GetCategoriesByBlogIdAsync(int id)
        {
            return Ok(_mapper.Map<List<CategoryDto>>(await _blogService.GetCategoriesByBlogIdAsync(id)));
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetLastSizeBlogAsync(int size)
        {
            return Ok(_mapper.Map<List<BlogDto>>(await _blogService.GetLastSizeAsync(size)));
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetComments(int blogId, int? parentCommentId)
        {
            return Ok(_mapper.Map<List<CommentListDto>>(await _commentService.GetAllWithSubCommentsAsync(blogId, parentCommentId)));
        }
        [HttpPost("[action]")]
        [ValidModel]
        public async Task<IActionResult> AddComment(CommentAddDto model)
        {
            var entity = _mapper.Map<CommentEntity>(model);
            await _commentService.AddAsync(_mapper.Map<CommentEntity>(model));
            return Created("", model);
        }

    }
}
