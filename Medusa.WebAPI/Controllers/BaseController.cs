﻿using Medusa.WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Medusa.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        public async Task<UploadModel> UploadFile(IFormFile formFile, string contentType)
        {
            UploadModel model = new UploadModel();
            if (formFile != null)
            {
                if (formFile.ContentType != contentType)
                {
                    model.ErrorMessage = "Uygunsuz dosya tipi";
                    model.UploadState = Enums.UploadState.error;
                }
                else
                {
                    var newName = Guid.NewGuid() + Path.GetExtension(formFile.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/" + newName);
                    var stream = new FileStream(path, FileMode.Create);

                    await formFile.CopyToAsync(stream);
                    model.NewName = newName;
                    model.UploadState = Enums.UploadState.success;
                }
            }
            else
            {
                model.UploadState = Enums.UploadState.notexists;
                model.ErrorMessage = "Dosya bulunamadı";
            }
            return model;
        }
    }
}