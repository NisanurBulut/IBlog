﻿using Medusa.WebUI.ApiServices.Interfaces;
using Medusa.WebUI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Medusa.WebUI.ApiServices.Concrete
{
    public class BlogApiService : IBlogApiService
    {
        private HttpClient _httpClient;
        public BlogApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:55315/api/blog");
        }
        public async Task<List<BlogListModel>> GetAllAsync()
        {
            var responseMessage = await _httpClient.GetAsync(_httpClient.BaseAddress+ "/GetAllBlogs");
            if (responseMessage.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<List<BlogListModel>>(await responseMessage.Content.ReadAsStringAsync());
                return result ?? new List<BlogListModel>();
            }
            return null;
        }

        public async Task<BlogListModel> GetByIdAsync(int id)
        {
            var responseMessage = await _httpClient.GetAsync(_httpClient.BaseAddress + "/GetBlogById");
            if (responseMessage.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<BlogListModel>(await responseMessage.Content.ReadAsStringAsync());
                return result ?? new BlogListModel();
            }
            return null;
        }
    }
}