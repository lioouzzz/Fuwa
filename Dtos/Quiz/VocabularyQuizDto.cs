namespace Dtos.Quiz;


public class VocabularyQuizDto
{
    public int VocabularyId { get; set; }
    public string Question { get; set; } = string.Empty;

    //public VocabularyQuizType Type { get; set; }

    public List<QuizOptionDto> Options { get; set; } = new();
}