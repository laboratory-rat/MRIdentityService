using Amazon.S3;
using Amazon.S3.Model;
using Infrastructure.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MRApiCommon.Exception;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BLL.Connector.Bucket
{
    public abstract class ConnectorBucket
    {
        protected readonly BucketOptions _options;
        protected AmazonS3Client _client => new AmazonS3Client(_options.AccessKey,
                                                               _options.SecretKey,
                                                               Amazon.RegionEndpoint.GetBySystemName(_options.Region));
        protected const string METADATA_PREFIX = "x-amz-meta-";
        public ConnectorBucket(IOptions<BucketOptions> optoins)
        {
            _options = optoins.Value;
        }

        /// <summary>
        /// Upload object to bucket
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="content">Byte content</param>
        /// <param name="contentType">File type</param>
        /// <param name="metadata">Metadata</param>
        /// <returns></returns>
        public async Task<BucketConnectorResponse> UploadWithName(string fileName, byte[] content, string contentType, Dictionary<string, string> metadata)
        {
            metadata = metadata ?? new Dictionary<string, string>();
            metadata.Add("name", fileName);
            return await Upload(CompilePath(fileName + "_" + Guid.NewGuid().ToString()).Replace(" ", "_"), content, contentType, metadata);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="content"></param>
        /// <param name="contentType"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public async Task<BucketConnectorResponse> Upload(string key, byte[] content, string contentType, Dictionary<string, string> metadata)
        {
            using (var client = _client)
            {
                PutObjectResponse response = null;
                using (var stream = new MemoryStream(content))
                {
                    var request = new PutObjectRequest
                    {
                        BucketName = _options.Bucket,
                        Key = key,
                        InputStream = stream,
                        ContentType = contentType,
                        CannedACL = S3CannedACL.PublicRead
                    };

                    request.Metadata.Add(nameof(contentType).ToUpperInvariant(), contentType);
                    if (metadata != null)
                    {
                        foreach (var kvp in metadata)
                        {
                            request.Metadata.Add(kvp.Key, kvp.Value);
                        }
                    }

                    response = await client.PutObjectAsync(request);
                }

                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new MRException<BucketConnectorResponse>(-1, "Save file unsuccess");
                }

                return new BucketConnectorResponse
                {
                    Key = key,
                    Url = CompileUrl(key),
                };
            }
        }


        /// <summary>
        /// Read object`s metadata
        /// </summary>
        /// <param name="key">Object key</param>
        /// <returns></returns>
        public async Task<BucketConnectorReadResponse> Get(string key)
        {
            var result = new BucketConnectorReadResponse
            {
                Key = key,
                Metadata = new Dictionary<string, string>()
            };

            using (var client = _client)
            {
                var bucketResponse = await client.GetObjectAsync(new GetObjectRequest
                {
                    BucketName = _options.Bucket,
                    Key = key
                });

                if (bucketResponse.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new MRException<BucketConnectorResponse>(-1, "Can  not upload object from bucket");
                }

                using (var ms = new MemoryStream())
                {
                    await bucketResponse.ResponseStream.CopyToAsync(ms);
                    result.Content = ms.GetBuffer();
                }

                foreach (var mKey in bucketResponse.Metadata.Keys)
                {
                    result.Metadata.Add(mKey.Replace(METADATA_PREFIX, string.Empty), bucketResponse.Metadata[mKey]);
                }
            }
            return result;
        }

        /// <summary>
        /// Delete object from bucket
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> Delete(string key)
        {
            using (var client = _client)
            {
                var request = new DeleteObjectRequest
                {
                    BucketName = _options.Bucket,
                    Key = key
                };

                var response = await client.DeleteObjectAsync(request);
            }

            return true;
        }

        protected string CompilePath(string rand)
            => string.IsNullOrWhiteSpace(_options.BasePath)
            ? rand
            : _options.BasePath + "/" + rand;
        public string CompileUrl(string key) => $"https://s3.amazonaws.com/{_options.Bucket}/{key}";
    }

    public class BucketConnectorResponse
    {
        public string Key { get; set; }
        public string Url { get; set; }
    }

    public class BucketConnectorReadResponse
    {
        public string Key { get; set; }
        public byte[] Content { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}
