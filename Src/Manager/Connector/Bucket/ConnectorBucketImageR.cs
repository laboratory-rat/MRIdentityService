using Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace BLL.Connector.Bucket
{
    public class ConnectorBucketImageR : ConnectorBucket
    {
        public ConnectorBucketImageR(IOptions<BucketOptions> optoins) : base(optoins) { }
    }
}
