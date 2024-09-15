using Amazon.S3;
using Amazon.S3.Model;

namespace MemoryBox_API.Services;

public class R2Service
{
    private readonly IAmazonS3 _r2Client;
    private readonly string _bucketName;

    public R2Service(IConfiguration configuration)
    {
        var accessKey = configuration["AWS:S3:AccessKey"];
        var secretKey = configuration["AWS:S3:SecretKey"];
        var endpoint = configuration["AWS:S3:Endpoint"];
        _bucketName = configuration["AWS:S3:BucketName"] 
                      ?? throw new ArgumentNullException(nameof(configuration), "AWS:S3:BucketName is not configured");

        var config = new AmazonS3Config
        {
            ServiceURL = endpoint,
            ForcePathStyle = true
        };

        _r2Client = new AmazonS3Client(accessKey, secretKey, config);
    }

    public async Task<(string PresignedUrl, string ImageUrl)> GeneratePresignedUrl(string fileName, string fileType)
    {
        var key = $"{Guid.NewGuid()}-{fileName}";
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = key,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddMinutes(15),
        };

        var presignedUrl = await _r2Client.GetPreSignedURLAsync(request);
        var imageUrl = $"{_r2Client.Config.ServiceURL}/{_bucketName}/{key}";

        return (presignedUrl, imageUrl);
    }
}