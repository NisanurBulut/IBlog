﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Medusa.WebUI.ApiServices.Interfaces
{
    interface IImageApiService
    {
        Task<string> GetBlogImageByIdAsync(int id);
    }
}
