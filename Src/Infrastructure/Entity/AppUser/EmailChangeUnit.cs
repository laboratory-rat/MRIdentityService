using Infrastructure.Entity.Enum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MRApiCommon.Infrastructure.Database;
using MRApiCommon.Infrastructure.Interface;
using System;

namespace Infrastructure.Entity.AppUser
{
    public class EmailChangeUnit : MREntity, IMREntity
    {
        public string UserId { get; set; }
        public string OldEmail { get; set; }
        public string NewEmail { get; set; }
        public string Token { get; set; }
        public DateTime Expired { get; set; }
        [BsonRepresentation(BsonType.String)]
        public EmailChangeResult Status { get; set; }
    }
}
