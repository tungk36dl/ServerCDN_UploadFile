﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Infrastructure.Common
{
    public class UploadFileHelper
    {
        private const string BASE_PATH = "wwwroot/uploads";

        private const string DEFAULT_UPLOAD_FOLDER = "";


        public static string UploadFile(IFormFile file, string folderName = "")
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return "";
                }
                if (string.IsNullOrEmpty(folderName)) folderName = DEFAULT_UPLOAD_FOLDER;
                var directoryPath = Path.Combine(BASE_PATH, folderName);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(directoryPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                var relativePath = filePath.Replace(BASE_PATH, "").Replace("\\", "/");
                return relativePath;
            }
            catch
            {
                return "";
            }
        }


        public static void RemoveFile(string path)
        {
            string filePath = path.Insert(0, BASE_PATH).Replace("/", "\\");
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

        public static DownloadData GetDownloadData(string path)
        {
            string filePath1 = path.Insert(0, BASE_PATH).Replace("/", "\\");

            string normalizedPath = path
    .TrimStart('\\', '/') // bỏ ký tự \ hoặc / ở đầu
    .Replace("/", Path.DirectorySeparatorChar.ToString())
    .Replace("\\", Path.DirectorySeparatorChar.ToString());

            string filePath = Path.Combine(BASE_PATH, normalizedPath);


            try
            {
                var downloadData = new DownloadData()
                {
                    FileName = Path.GetFileName(filePath),
                    FileBytes = File.ReadAllBytes(filePath)
                };
                return downloadData;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static string GetPhysicalPath(string relativePath)
        {
            relativePath = relativePath.TrimStart('/', '\\');
            var fullPath = Path.Combine(BASE_PATH, relativePath.Replace('/', Path.DirectorySeparatorChar));

            // Bảo vệ chống path traversal
            var basePath = Path.GetFullPath(BASE_PATH);
            fullPath = Path.GetFullPath(fullPath);

            if (!fullPath.StartsWith(basePath))
            {
                throw new InvalidOperationException("Invalid file path");
            }

            return fullPath;
        }
    }

    public class DownloadData
    {
        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }
    }

}
