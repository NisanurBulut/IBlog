﻿using Medusa.WebUI.ApiServices.Interfaces;
using Medusa.WebUI.Extensions;
using Medusa.WebUI.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Medusa.WebUI.ApiServices.Concrete
{
    public class BlogApiService : IBlogApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpcontextAccessor; // Sessiona erişmek için kullanıcaz
        public BlogApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpcontextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:55315/api/blog");
        }
        public async Task<List<BlogListModel>> GetAllAsync()
        {
            var responseMessage = await _httpClient.GetAsync(_httpClient.BaseAddress + "/GetAllBlogs");
            if (responseMessage.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<List<BlogListModel>>(await responseMessage.Content.ReadAsStringAsync());
                return result ?? new List<BlogListModel>();
            }
            return null;
        }

        public async Task<List<BlogListModel>> GetAllByCategoryIdAsync(int id)
        {
            var responseMessage = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/GetAllByCategoryId?id={id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<List<BlogListModel>>
                    (await responseMessage.Content.ReadAsStringAsync());
                return result ?? new List<BlogListModel>();
            }
            return null;
        }

        public async Task<BlogListModel> GetByIdAsync(int id)
        {
            var responseMessage = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/GetBlogById?id={id}");
            if (responseMessage.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<BlogListModel>(await responseMessage.Content.ReadAsStringAsync());
                return result ?? new BlogListModel();
            }
            return null;
        }
        public async Task AddAsync(BlogAddModel model)
        {
            MultipartFormDataContent formDataContent = new MultipartFormDataContent();

            if (model.Image != null)
            {
                var bytes = await System.IO.File.ReadAllBytesAsync(model.Image.FileName);

                var user = _httpcontextAccessor.HttpContext.Session.GetObject<AppUserViewModel>("activeUser");
                model.AppUserId = user.Id;

                // resimi byte'a çeviricez
                ByteArrayContent byteContent = new ByteArrayContent(bytes);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue(model.Image.ContentType);

                formDataContent.Add(byteContent, nameof(BlogAddModel.Image), model.Image.FileName);

                formDataContent.Add(new StringContent(model.AppUserId.ToString()), nameof(BlogAddModel.AppUserId));

                formDataContent.Add(new StringContent(model.ShortDescription), nameof(BlogAddModel.ShortDescription));

                formDataContent.Add(new StringContent(model.LongDescription), nameof(BlogAddModel.LongDescription));

                formDataContent.Add(new StringContent(model.Title), nameof(BlogAddModel.Title));

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _httpcontextAccessor.HttpContext.Session.GetString("token"));

                await _httpClient.PostAsync($"{_httpClient.BaseAddress}/CreateBlog", formDataContent);

            }
        }
    }
}
