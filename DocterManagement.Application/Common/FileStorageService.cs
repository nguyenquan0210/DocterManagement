using DoctorManagement.ViewModels.Catalog.Post;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
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
        private readonly string _contentFolder;
        private const string IMG_CONTENT_FOLDER_NAME = "img";
        private const string USER_CONTENT_FOLDER_NAME = "user-content";
        private const string POSTS_CONTENT_FOLDER_NAME = "posts-content";
        private const string CLINIC_CONTENT_FOLDER_NAME = "clinic-content";
        private const string CLINICS_CONTENT_FOLDER_NAME = "clinics-content";

        private readonly IConfiguration _configuration;


        public FileStorageService(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _userContentFolder = Path.Combine(webHostEnvironment.WebRootPath, Path.Combine(IMG_CONTENT_FOLDER_NAME , USER_CONTENT_FOLDER_NAME));
            _postContentFolder = Path.Combine(webHostEnvironment.WebRootPath, POSTS_CONTENT_FOLDER_NAME);
            _contentFolder = Path.Combine(webHostEnvironment.WebRootPath, IMG_CONTENT_FOLDER_NAME);
            _configuration = configuration;
        }


        public string GetFileUrl(string fileName)
        {
            return $"/{IMG_CONTENT_FOLDER_NAME}/{USER_CONTENT_FOLDER_NAME}/{fileName}";
        }
        public string GetFileUrls(string fileName, string folderName)
        {
            return $"/{IMG_CONTENT_FOLDER_NAME}/{folderName}/{fileName}";
        }
        public async Task SaveFileAsyncs(Stream mediaBinaryStream, string fileName, string folderName)
        {
            var filePath = Path.Combine(Path.Combine(_contentFolder, folderName), fileName);
            using var output = new FileStream(filePath, FileMode.Create);
            await mediaBinaryStream.CopyToAsync(output);
        }
        public async Task DeleteFileAsyncs(string fileName, string folderName)
        {
            var filePath = Path.Combine(Path.Combine(_contentFolder, folderName), fileName);
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }
        }
        public async Task SaveFileAsync(Stream mediaBinaryStream, string fileName)
        {
            var filePath = Path.Combine(_userContentFolder, fileName);
            using var output = new FileStream(filePath, FileMode.Create);
            await mediaBinaryStream.CopyToAsync(output);
        }
        public async Task SaveFileImgAsync(Stream mediaBinaryStream, string fileName, string folderName)
        {
            var filePath = Path.Combine(Path.Combine(_contentFolder, folderName), fileName);
            using var output = new FileStream(filePath, FileMode.Create);
            await mediaBinaryStream.CopyToAsync(output);
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
            return $"/{IMG_CONTENT_FOLDER_NAME}/{POSTS_CONTENT_FOLDER_NAME}/{fileName}";
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