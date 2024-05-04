using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Service_Layer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AC_Jobs_API_Service_Layer.IService
{
    public interface IFileService
    {
        Task<FileDTO> AddFileAsync(IFormFile file);
        Task<IReadOnlyList<FileDTO>> AddFilesAsync(List<IFormFile> files);
        Task<bool> DeleteFileByIdAsync(string filePath);
        Task<bool> DeleteFilesByIdsAsync(List<string> filePaths);
        Task<IActionResult> DownloadFiles(List<string> filePaths);
    }
}
