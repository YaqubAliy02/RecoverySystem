using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using RecoverySystem.CaseService.Services;

public class S3StorageService : IS3StorageService
{
    private readonly string _bucketName;
    private readonly IAmazonS3 _s3Client;

    public S3StorageService(IConfiguration config)
    {
        _bucketName = config["AWS:BucketName"];
        _s3Client = new AmazonS3Client(
            config["AWS:AccessKey"],
            config["AWS:SecretKey"],
            RegionEndpoint.GetBySystemName(config["AWS:Region"])
        );
    }

    public async Task<string> UploadFileAsync(IFormFile file, string keyPrefix = "")
    {
        var fileKey = $"{keyPrefix}/{Guid.NewGuid()}_{file.FileName}";
        using var newMemoryStream = new MemoryStream();
        await file.CopyToAsync(newMemoryStream);

        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = newMemoryStream,
            Key = fileKey,
            BucketName = _bucketName,
            ContentType = file.ContentType
        };

        var fileTransferUtility = new TransferUtility(_s3Client);
        await fileTransferUtility.UploadAsync(uploadRequest);

        return $"https://{_bucketName}.s3.amazonaws.com/{fileKey}";
    }
}
