using IMC_CC_App.Data;
using IMC_CC_App.DTO;
using IMC_CC_App.Interfaces;
using ILogger = Serilog.ILogger;
using Azure.Storage.Blobs;
using Azure.Storage;
using Azure.Storage.Blobs.Models;
using System.Text.RegularExpressions;

namespace IMC_CC_App.Services
{
    public class ImageService : IImage
    {
        private readonly ImagesStoreContext _context;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public ImageService(ImagesStoreContext context, ILogger logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public Task<BlobData> DownloadImage(string name)
        {
            throw new System.NotImplementedException();
        }

        // Move the Upload details to ImageStoreContext.cs ***
        public async Task<string> UploadImages(int rptId, IFormFile Images)
        {
            string imageName = $"{Guid.NewGuid()}_{Images.FileName}";
            _logger.Warning($"Uploading image {imageName} to report {rptId}");
            //_logger.Warning($"Uploading image - ImageContainerStorageAccount:{_configuration["ImageContainerStorageAccount"]} - ImageContainerConnectionString: {_configuration["ImageContainerConnectionString"]} - ImageContainerName: {_configuration["ImageContainerName"]}");
            // return await _context.UploadImage(rptId, Images, imageName);
            string response = string.Empty;
            string? storageAccount = _configuration["ImageContainerStorageAccount"];
            string? connectionString = _configuration["ImageContainerConnectionString"];
            string? containerName = _configuration["ImageContainerName"];
            var match = connectionString != null ? Regex.Match(connectionString, @"AccountKey=([^;]+)") : null;
            string? accountKey = match?.Groups[1].Value ?? null;

            try
            {
                if (storageAccount == null || connectionString == null || containerName == null || accountKey == null)
                {
                    _logger.Error("Error uploading image: storageAccount, connectionString or containerName is null");
                    return response;
                }

                var credential = new StorageSharedKeyCredential(storageAccount, accountKey);
                //_logger.Warning($"Uploading image - credential: {credential.AccountName}");
                var blobEndpoint = $"https://{storageAccount}.blob.core.windows.net";
                //_logger.Warning($"Uploading image - blobEndpoint: {blobEndpoint}");
                var blobServiceClient = new BlobServiceClient(new Uri(blobEndpoint), credential);
                BlobContainerClient _blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

                var blobClient = _blobContainerClient.GetBlobClient($"{rptId}/{imageName}");


                _logger.Information($"Uploading image {imageName} to {blobClient.Uri}");
                //Console.WriteLine($"Uploading image {imageName} to {blobClient.Uri}");
                using (var stream = Images.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = Images.ContentType });
                }

                response = blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.Error("Error uploading image: {ex}", ex);
            }

            return response;
        }

    }

}