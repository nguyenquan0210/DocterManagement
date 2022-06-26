using DoctorManagement.ViewModels.Catalog.Post;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoctorManagement.Application.Common
{
    public interface IStorageService
    {
        string GetFileUrl(string fileName);

        Task SaveFileAsync(Stream mediaBinaryStream, string fileName);
        Task SaveFileImgAsync(Stream mediaBinaryStream, string fileName, string folderName);

        Task DeleteFileAsync(string fileName);

        string GetFileUrls(string fileName, string folderName);

        Task SaveFileAsyncs(Stream mediaBinaryStream, string fileName, string folderName);
        Task DeleteFileAsyncs(string fileName, string folderName);
        string GetFilePostUrl(string fileName);

        Task SaveFilePostAsync(Stream mediaBinaryStream, string fileName);

        Task DeleteFilePostAsync(string fileName);
    }
}
