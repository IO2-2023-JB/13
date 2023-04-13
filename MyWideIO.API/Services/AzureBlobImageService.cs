using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MyWideIO.API.Exceptions;
using MyWideIO.API.Services.Interfaces;
using SixLabors.ImageSharp.Formats;

namespace MyWideIO.API.Services
{
    public class AzureBlobImageService : IImageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainerClient;
        private readonly string containerName = "blob1";

        public AzureBlobImageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            if (!_blobContainerClient.Exists())
            {
                _blobContainerClient = _blobServiceClient.CreateBlobContainer(containerName);
            }
            _blobContainerClient.SetAccessPolicy(PublicAccessType.BlobContainer);
        }

        public Task RemoveImageAsync(string fileName)
        {
            throw new NotImplementedException();
        }

        public async Task<(string url, string fileName)> UploadImageAsync(string base64image, string id)
        {
            byte[] buffer;
            try
            {
                buffer = Convert.FromBase64String(base64image);
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
                    format = Image.DetectFormat(ms);
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
            BinaryData binaryData = new(buffer);
            BlobClient blobClient = _blobContainerClient.GetBlobClient(fileName);
            Response response;
            response = (await blobClient.UploadAsync(binaryData, true)).GetRawResponse();
            if (response.IsError)
            {
                throw new UserException("Image upload error");
            }

            response = (await blobClient.SetHttpHeadersAsync(new BlobHttpHeaders
            {
                ContentType = format.DefaultMimeType,
            })).GetRawResponse();
            return response.IsError ? throw new UserException("Image upload error") : ((string url, string fileName))(blobClient.Uri.AbsoluteUri, fileName);
        }
    }
}
