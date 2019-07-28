using System;

namespace Infrastructure.Entity.AppFile
{
    public class File
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string Type { get; set; }
        public long Length { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreatedBy { get; set; }
    }
}
