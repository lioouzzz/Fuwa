namespace Dtos.Quiz;

public class QuizAttemptWrongAnswerDto
{
    public int VocabularyId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string UserAnswer { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;

}