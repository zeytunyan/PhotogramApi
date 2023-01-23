﻿namespace DAL.Entities
{
    public class Attach
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public long Size { get; set; }

        public Guid AuthorId { get; set; }
        public virtual User? Author { get; set; }
    }
}
