using AC_Jobs_API_Domian_Layer.Models;
using AC_Jobs_API_Service_Layer.IService;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO.Compression;
using System.Security.Claims;

namespace AC_Jobs_API_Service_Layer.Services
{
    public class FileService : IFileService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _env;

        public long LoggedInUserCompanyId;
        public long LoggedInUserId;
        public string uploadsPath;
        public string apiUrl;
        public string path;

        public FileService(
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            IHostEnvironment env)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _env = env;

            ClaimsIdentity user = (ClaimsIdentity)_httpContextAccessor.HttpContext.User.Identity;
            LoggedInUserCompanyId = Convert.ToUInt32(user.FindFirst("CompanyId").Value.ToString());
            LoggedInUserId = Convert.ToUInt32(user.FindFirst("UserId").Value.ToString());
            uploadsPath = _configuration.GetValue<string>("UploadsPath");
            apiUrl = _configuration.GetValue<string>("ApiUrl");
            path = Path.Combine(Directory.GetCurrentDirectory(), uploadsPath);
        }

        public async Task<FileDTO> AddFileAsync(IFormFile requestFile)
        {
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(requestFile.FileName);
            var filePath = Path.Combine(path, uniqueFileName);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var derivedFile = new FileDTO();

            derivedFile.FileName = uniqueFileName;
            derivedFile.ActualName = requestFile.Name;
            derivedFile.FileSize = requestFile.Length;
            derivedFile.ContentType = requestFile.ContentType;
            derivedFile.FileExtension = Path.GetExtension(requestFile.FileName);
            derivedFile.FileUrl = GenerateUrl(uniqueFileName);
            derivedFile.FilePath = filePath;

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await requestFile.CopyToAsync(stream);
            }
            return derivedFile;
        }

        public async Task<IReadOnlyList<FileDTO>> AddFilesAsync(List<IFormFile> requestFiles)
        {
            var filesAdded = new List<FileDTO>() { };
            foreach (var requestFile in requestFiles)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(requestFile.FileName);
                var filePath = Path.Combine(path, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await requestFile.CopyToAsync(stream);
                }

                var derivedFile = new FileDTO();
                derivedFile.FileName = uniqueFileName;
                derivedFile.ActualName = requestFile.Name;
                derivedFile.FileSize = CalculateFileSize(requestFile.Length);
                derivedFile.ContentType = requestFile.ContentType;
                derivedFile.FileExtension = Path.GetExtension(requestFile.FileName);
                derivedFile.FileUrl = GenerateUrl(uniqueFileName);
                derivedFile.FilePath = filePath;
                filesAdded.Add(derivedFile);
            }
            return filesAdded;
        }

        public async Task<bool> DeleteFileByIdAsync(string FilePath)
        {
            try
            {
                File.Delete(FilePath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteFilesByIdsAsync(List<string> FilePaths)
        {
            try
            {
                foreach (var FilePath in FilePaths)
                {
                    File.Delete(FilePath);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IActionResult> DownloadFiles(List<string> filePaths)
        {
            string tempDir = Path.Combine(path, "TempDownload");
            string zipFilePath = string.Empty;
            string zipFileName = string.Empty;
            try
            {
                var memory = new MemoryStream();

                if (filePaths.Count == 1)
                {
                    string file = filePaths.Single();
                    string fileName = Path.GetFileName(file);
                    string fullFilePath = Path.Combine(path, fileName);
                    if (File.Exists(fullFilePath))
                    {
                        using (var stream = new FileStream(fullFilePath, FileMode.Open))
                        {
                            await stream.CopyToAsync(memory);
                        }
                        memory.Position = 0;

                        return new FileStreamResult(memory, "application/octet-stream")
                        {
                            FileDownloadName = Path.GetFileName(fullFilePath)
                        };
                    }
                    else
                    {
                        return new NotFoundObjectResult(new { message = "File Not Found" });
                    }
                }

                if (!Directory.Exists(tempDir))
                {
                    Directory.CreateDirectory(tempDir);
                }

                foreach (var filePath in filePaths)
                {
                    string fileName = Path.GetFileName(filePath);
                    string fullFilePath = Path.Combine(path, fileName);

                    if (File.Exists(fullFilePath))
                    {
                        string destinationPath = Path.Combine(tempDir, fileName);
                        File.Copy(fullFilePath, destinationPath, true);
                    }
                    else
                    {
                        continue;
                    }
                }

                zipFileName = $"DownloadFiles_{DateTime.Now:yyyyMMddHHmmss}.zip";
                zipFilePath = Path.Combine(path, zipFileName);

                ZipFile.CreateFromDirectory(tempDir, zipFilePath);

                using (var stream = new FileStream(zipFilePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                return new FileStreamResult(memory, "application/zip")
                {
                    FileDownloadName = zipFileName
                };
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                await Task.Run(() =>
                {
                    if (Directory.Exists(tempDir))
                    {
                        Directory.Delete(tempDir, true);
                    }

                    if (File.Exists(zipFilePath))
                    {
                        File.Delete(zipFilePath);
                    }

                });
            }
        }

        #region HelperMethod
        public string GenerateUrl(string uniqueName)
        {
            string path = uploadsPath;
            char[] delimiter = { '/' };

            string[] parts = path.Split(delimiter);

            string[] remainingParts = parts.Skip(1).ToArray();

            string remainingString = string.Join("/", remainingParts) + '/';
            var fileUrl = apiUrl + remainingString + uniqueName;
            return fileUrl;
        }
        private long CalculateFileSize(long fileSizeInBytes)
        {
            const double bytesInMegabyte = 1024 * 1024;
            return (long)Math.Round((double)fileSizeInBytes / bytesInMegabyte, 2);
        }

        #endregion
    }
}
