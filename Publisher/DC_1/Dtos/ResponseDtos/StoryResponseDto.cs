namespace lab_1.Dtos.ResponseDtos
{
    public class StoryResponseDto:BaseResponseDto
    {
        public long? AuthorId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }

    }
}
