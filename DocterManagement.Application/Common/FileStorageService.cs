﻿using DoctorManagement.ViewModels.Catalog.Post;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Common
{
    public class FileStorageService : IStorageService
    {
        private readonly string _userContentFolder;
        private readonly string _postContentFolder;
        private const string IMG_CONTENT_FOLDER_NAME = "img";
        private const string USER_CONTENT_FOLDER_NAME = "user-content";
        private const string POST_CONTENT_FOLDER_NAME = "post-content";

        public FileStorageService(IWebHostEnvironment webHostEnvironment)
        {
            _userContentFolder = Path.Combine(webHostEnvironment.WebRootPath, Path.Combine(IMG_CONTENT_FOLDER_NAME , USER_CONTENT_FOLDER_NAME));
            _postContentFolder = Path.Combine(webHostEnvironment.WebRootPath, POST_CONTENT_FOLDER_NAME);

        }


        public string GetFileUrl(string fileName)
        {
            return $"/img/{USER_CONTENT_FOLDER_NAME}/{fileName}";
        }

        public async Task SaveFileAsync(Stream mediaBinaryStream, string fileName)
        {
            var filePath = Path.Combine(_userContentFolder, fileName);
            using var output = new FileStream(filePath, FileMode.Create);
            await mediaBinaryStream.CopyToAsync(output);
        }
        public async Task<ImagesVm> SaveFileImgAsync(Stream mediaBinaryStream, string fileName)
        {
            var filePath = Path.Combine(_userContentFolder, fileName);
            using var output = new FileStream(filePath, FileMode.Create);
            await mediaBinaryStream.CopyToAsync(output);
            return new ImagesVm()
            {
                FileUrl = filePath,
                Container = USER_CONTENT_FOLDER_NAME
            };
        }

        public async Task DeleteFileAsync(string fileName)
        {
            var filePath = Path.Combine(_userContentFolder, fileName);
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }
        }

        public string GetFilePostUrl(string fileName)
        {
            return $"/{POST_CONTENT_FOLDER_NAME}/{fileName}";
        }

        public async Task SaveFilePostAsync(Stream mediaBinaryStream, string fileName)
        {
            var filePath = Path.Combine(_postContentFolder, fileName);
            using var output = new FileStream(filePath, FileMode.Create);
            await mediaBinaryStream.CopyToAsync(output);
        }

        public async Task DeleteFilePostAsync(string fileName)
        {
            var filePath = Path.Combine(_postContentFolder, fileName);
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }
        }
    }
}