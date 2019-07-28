using Infrastructure.Enum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Infrastructure.Entity.AppProvider
{
    public class ProviderUser
    {
        public string UserId { get; set; }

        public string UserEmail { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreatedBy { get; set; }

        [BsonRepresentation(BsonType.String)]
        public List<ProviderUserRole> Roles { get; set; }
    }
}
