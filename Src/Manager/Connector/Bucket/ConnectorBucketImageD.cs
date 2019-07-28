using Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace BLL.Connector.Bucket
{
    public class ConnectorBucketImageD : ConnectorBucket
    {
        public ConnectorBucketImageD(IOptions<BucketOptions> optoins) : base(optoins)
        {
        }
    }
}
