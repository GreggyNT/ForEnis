namespace lab_1.Cassa.Dtos.ResponseDto
{
    public class CommentResponseDto:BaseResponseDto
    {
        public long? StoryId { get; set; }
        public string? Content { get; set; }
    }
}
