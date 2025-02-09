using IMC_CC_App.DTO;

namespace IMC_CC_App.Interfaces
{
    public interface IImage
    {
        Task<string> UploadImages(int rptId, IFormFile Images);

        Task<BlobData> DownloadImage(string name);
    }
}