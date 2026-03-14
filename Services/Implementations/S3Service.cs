using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using TechnicalTest.Api.Services.Interfaces;

namespace TechnicalTest.Api.Services.Implementations;

public class S3Service : IS3Service
{
    private readonly IAmazonS3 _s3;
    private readonly IConfiguration _configuration;
    private readonly ILogger<S3Service> _logger;

    public S3Service(IAmazonS3 s3, IConfiguration configuration, ILogger<S3Service> logger)
    {
        _s3 = s3;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> CreateBucketAsync(string bucketName)
    {
        try
        {
            var exists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3, bucketName);
            if (!exists)
            {
                await _s3.PutBucketAsync(new PutBucketRequest
                {
                    BucketName = bucketName
                });
                _logger.LogInformation("Bucket {BucketName} created successfully", bucketName);
            }

            var policy = @"{
                ""Version"": ""2012-10-17"",
                ""Statement"": [
                    {
                        ""Sid"": ""PublicReadGetObject"",
                        ""Effect"": ""Allow"",
                        ""Principal"": ""*"",
                        ""Action"": ""s3:GetObject"",
                        ""Resource"": ""arn:aws:s3:::" + bucketName + @"/*""
                    }
                ]
            }";

            await _s3.PutBucketPolicyAsync(new PutBucketPolicyRequest
            {
                BucketName = bucketName,
                Policy = policy
            });

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating bucket {BucketName}", bucketName);
            throw;
        }
    }
    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        try
        {
            var key = $"{Guid.NewGuid()}/{fileName}";

            var request = new PutObjectRequest
            {
                BucketName = _configuration["Minio:BucketName"],
                Key = key,
                InputStream = fileStream,
                ContentType = contentType
            };

            await _s3.PutObjectAsync(request);
            return key;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file {FileName}", fileName);
            throw;
        }
    }

    public async Task<Stream> DownloadFileAsync(string fileKey)
    {
        try
        {
            var request = new GetObjectRequest
            {
                BucketName = _configuration["Minio:BucketName"],
                Key = fileKey
            };

            var response = await _s3.GetObjectAsync(request);
            return response.ResponseStream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file {FileKey}", fileKey);
            throw;
        }
    }

    public async Task DeleteFileAsync(string fileKey)
    {
        try
        {
            var request = new DeleteObjectRequest
            {
                BucketName = _configuration["Minio:BucketName"],
                Key = fileKey
            };

            await _s3.DeleteObjectAsync(request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {FileKey}", fileKey);
            throw;
        }
    }

    public string GetUri(string fileKey)
    {
        var endpoint = _configuration["Minio:Endpoint"] ?? "http://localhost:9000";
        var bucketName = _configuration["Minio:BucketName"];

        return $"{endpoint}/{bucketName}/{fileKey}";
    }
}
