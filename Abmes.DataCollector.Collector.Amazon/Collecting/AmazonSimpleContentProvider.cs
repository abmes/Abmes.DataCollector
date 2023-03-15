using Abmes.DataCollector.Collector.Common.Collecting;
using Abmes.DataCollector.Common.Amazon.Configuration;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abmes.DataCollector.Collector.Amazon.Collecting
{
    public class AmazonSimpleContentProvider : ISimpleContentProvider
    {
        private const string S3LocationPrefix = "s3://";
        private readonly IAmazonS3 _amazonS3;

        public AmazonSimpleContentProvider(
            IAmazonS3 amazonS3)
        {
            _amazonS3 = amazonS3;
        }

        public async Task<byte[]?> GetContentAsync(string uri, CancellationToken cancellationToken)
        {
            if (!uri.StartsWith(S3LocationPrefix))
            {
                return null;
            }

            var locationParts = uri[S3LocationPrefix.Length..].Split("/");
            var bucketName = locationParts.First();
            var key = string.Join("/", locationParts.Skip(1));

            var request = new GetObjectRequest { BucketName = bucketName, Key = key };
            var response = await _amazonS3.GetObjectAsync(request, cancellationToken);

            using var ms = new MemoryStream();

            response.ResponseStream.CopyTo(ms);
            return ms.ToArray();
        }
    }
}
