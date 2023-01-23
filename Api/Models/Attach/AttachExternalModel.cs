﻿namespace Api.Models.Attach
{
    public class AttachExternalModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public string? ContentLink { get; set; } = null!;
    }
}
