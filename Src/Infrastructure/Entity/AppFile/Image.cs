using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using System;
using System.Collections.Generic;

namespace Infrastructure.Entity.AppFile
{
    public class MRImage
    {
        public string Key { get; set; }
        public string Url { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public string ContentType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;

        [BsonDictionaryOptions(DictionaryRepresentation.Document)]
        public Dictionary<string, string> Metadata { get; set; }
    }
}
