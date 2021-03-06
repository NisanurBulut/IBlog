﻿using Medusa.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Medusa.DataAccess.Interface
{
    public interface IBlogDal:IGenericDal<BlogEntity>
    {
        Task<List<BlogEntity>> GetLastSizeAsync(int size);
        Task<List<BlogEntity>> GetAllByCategoryIdAsync(int id);
        Task<List<CategoryEntity>> GetCategoriesByBlogIdAsync(int blogId);
    }
}
