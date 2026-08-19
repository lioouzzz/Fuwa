namespace Dtos.Lessons;

public class LessonDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int LessonNumber { get; set; }

    public string? Description { get; set; }

}
