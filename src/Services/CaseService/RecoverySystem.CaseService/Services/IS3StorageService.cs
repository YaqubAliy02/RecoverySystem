namespace RecoverySystem.CaseService.Services;

public interface IS3StorageService
{
    Task<string> UploadFileAsync(IFormFile file, string keyPrefix = "");
}
