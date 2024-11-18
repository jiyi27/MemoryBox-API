# MemoryBox-API


## Installation

Configure your appsettings.json file with the following:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost; database=your-database; user=root; password=password"
  },
  "Jwt": {
    "Key": "your-jwt-key",
    "Issuer": "Test.com"
  },
  "AWS": {
    "S3": {
      "AccessKey": "your-access-key",
      "SecretKey": "your-secret-key",
      "BucketName": "your-bucket-name",
      "Endpoint": "your endpoint"
    }
  }
}
```