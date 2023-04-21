﻿namespace MyWideIO.API.Services.Interfaces
{
    public interface IImageService
    {
        /// <summary>
        /// Uploads a image and returns url to that image
        /// </summary>
        /// <param name="base64image">base64-encoded image</param>
        /// <param name="fileName">name for the image file</param>
        /// <returns>uploaded image's url and filename</returns>
        public Task<(string url, string fileName)> UploadImageAsync(string base64image, string fileName);
        /// <summary>
        /// Removes the image file
        /// </summary>
        /// <param name="fileName">image file name</param>
        /// <returns></returns>
        public Task RemoveImageAsync(string fileName);
        public string GetImageBase64(string fileName);
    }
}
