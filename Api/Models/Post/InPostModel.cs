using Api.Models.Attach;

namespace Api.Models.Post
{
    public class InPostModel
    {
        public Guid? AuthorId { get; set; }
        public string? Text { get; set; }
        public List<InFileMetaModel> Contents { get; set; } = new List<InFileMetaModel>();

    }
}
