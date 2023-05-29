using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MyWideIO.API.Exceptions;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Services.Interfaces;
using SixLabors.ImageSharp.Formats;
using System;

namespace MyWideIO.API.Services
{
    public class AzureBlobImageStorageService : IImageStorageService
    {
        private readonly BlobContainerClient _blobContainerClient;
        private const string ContainerName = "blob1";

        public AzureBlobImageStorageService(BlobServiceClient blobServiceClient)
        {
            _blobContainerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
            if (!_blobContainerClient.Exists())
            {
                _blobContainerClient = blobServiceClient.CreateBlobContainer(ContainerName);
            }
            _blobContainerClient.SetAccessPolicy(PublicAccessType.BlobContainer);
        }

        public async Task RemoveImageAsync(string fileName)
        {
            var blobClient = _blobContainerClient.GetBlobClient(fileName);

            var response = await blobClient.DeleteAsync();
            if (response.IsError)
            {
                throw new UserException("Image removal error");
            }
        }

        public string GetImageBase64(string fileName)
        {
            BlobClient blobClient = _blobContainerClient.GetBlobClient(fileName);

            using var br = new BinaryReader(blobClient.OpenRead());
            var file = br.ReadBytes((int)br.BaseStream.Length);
            return Convert.ToBase64String(file);
        }

        public async Task<ImageModel> UploadImageAsync(string base64Image, string id)
        {
            byte[] buffer;
            try
            {
                buffer = Convert.FromBase64String(base64Image);
            }
            catch (FormatException e)
            {
                throw new UserException(e.Message);
            }

            IImageFormat format;
            using (MemoryStream ms = new(buffer))
            {
                try
                {
                    format = await Image.DetectFormatAsync(ms);
                }
                catch (InvalidImageContentException e)
                {
                    throw new UserException(e.Message);
                }
                catch (UnknownImageFormatException e)
                {
                    throw new UserException(e.Message);
                }

            }

            string fileName = id + "." + format.Name.ToLower();
            var binaryData = new BinaryData(buffer);
            var blobClient = _blobContainerClient.GetBlobClient(fileName);
            var response = (await blobClient.UploadAsync(binaryData, true)).GetRawResponse();
            if (response.IsError)
            {
                throw new UserException("Image upload error");
            }

            response = (await blobClient.SetHttpHeadersAsync(new BlobHttpHeaders
            {
                ContentType = format.DefaultMimeType,
            })).GetRawResponse();
            if (response.IsError)
                throw new UserException("Image upload error");
            return new ImageModel
            {
                Url = blobClient.Uri.AbsoluteUri,
                FileName = fileName
            };
        }

        public async Task<ImageModel> UploadImageAsync(Stream stream, string fileName)
        {
            IImageFormat format;

            try
            {
                format = await Image.DetectFormatAsync(stream);
            }
            catch (InvalidImageContentException e)
            {
                throw new UserException(e.Message);
            }
            catch (UnknownImageFormatException e)
            {
                throw new UserException(e.Message);
            }
            stream.Position = 0;
            var blobClient = _blobContainerClient.GetBlobClient(fileName);
            var response = (await blobClient.UploadAsync(stream, true)).GetRawResponse();
            if (response.IsError)
            {
                throw new UserException("Image upload error");
            }

            response = (await blobClient.SetHttpHeadersAsync(new BlobHttpHeaders
            {
                ContentType = format.DefaultMimeType,
            })).GetRawResponse();
            if (response.IsError)
                throw new UserException("Image upload error");
            return new ImageModel
            {
                Url = blobClient.Uri.AbsoluteUri,
                FileName = fileName
            };

        }
    }
}
