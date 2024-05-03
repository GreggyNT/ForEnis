﻿namespace lab_1.Cassa.Entities;

public partial class TblComment:TblBase
{
    
    public TblComment()
    {
    }

    public TblComment(long id, long storyId, string content)
    {
        Id = id;
        StoryId = storyId;
        Content = content;
    }
    

    public long StoryId { get; set; }

    public string Content { get; set; } = null!;

    public string Country = "Belarus";

}
