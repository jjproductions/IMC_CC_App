using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;
using Azure.Storage.Blobs.Models;
using Azure.Storage;
using Azure.Storage.Blobs;
using IMC_CC_App.DTO;

namespace IMC_CC_App.Data
{
    public class ImagesStoreContext
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        public ImagesStoreContext(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        // public async Task<string> UploadImage(int rptId, IFormFile receiptImage, string name)
        public async Task<string> UploadImage(int rptId, string name)
        {
            _logger.Information($"Start-DBContext_Images-UploadImage {name} for report {rptId}");
            Console.WriteLine($"Start-DBContext_Images-UploadImage {name} for report {rptId}");
            string response = string.Empty;


            try
            {
                var credential = new StorageSharedKeyCredential(_configuration["ImageContainerStorageAccount"], _configuration["ImageContainerConnectionString"]);
                var blobEndpoint = $"https://{_configuration["ImageContainerStorageAccount"]}.blob.core.windows.net";
                var blobServiceClient = new BlobServiceClient(new Uri(blobEndpoint), credential);
                BlobContainerClient _blobContainerClient = blobServiceClient.GetBlobContainerClient(_configuration["ImageContainerName"]);

                var blobClient = _blobContainerClient.GetBlobClient($"{rptId}/{name}");


                _logger.Information($"Uploading image {name} to {blobClient.Uri}");
                Console.WriteLine($"Uploading image {name} to {blobClient.Uri}");
                // using (var stream = receiptImage.OpenReadStream())
                // {
                //     await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = receiptImage.ContentType });
                // }

                // response = blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.Error("Error uploading image: {ex}", ex);
            }

            return response;
        }

        public async Task<BlobData> DownloadImage(string name)
        {
            var credential = new StorageSharedKeyCredential(_configuration["ImageContainerStorageAccount"], _configuration["ImageContainerConnectionString"]);
            var blobEndpoint = $"https://{_configuration["ImageContainerStorageAccount"]}.blob.core.windows.net";
            var blobServiceClient = new BlobServiceClient(new Uri(blobEndpoint), credential);
            BlobContainerClient _blobContainerClient = blobServiceClient.GetBlobContainerClient(_configuration["ImageContainerName"]);

            BlobClient blobClient = _blobContainerClient.GetBlobClient(name);
            var blobDownloadInfo = await blobClient.DownloadAsync();

            BlobData response = new BlobData
            {
                Name = name,
                Url = blobClient.Uri.ToString(),
                ContentType = blobDownloadInfo.Value.Details.ContentType,
                Content = blobDownloadInfo.Value.Content
            };

            return response;
        }
    }

}