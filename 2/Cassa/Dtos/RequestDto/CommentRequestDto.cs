namespace lab_1.Cassa.Dtos.RequestDto
{
    public class CommentRequestDto : BaseRequestDto
    {
        public long? StoryId { get; set; }
        public string? Content { get; set; }
    }
}
