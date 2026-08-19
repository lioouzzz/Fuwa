using System.ComponentModel.DataAnnotations;

namespace Dtos.Lessons;

public class CreateLessonDto
{
    public string Title { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "課程必須大於 0")]
    public int LessonNumber { get; set; }

    public string? Description { get; set; }

}