namespace Dtos.Quiz;



public class SubmitVocabularyAnswerDto
{
    public int VocabularyId { get; set; }
    public int QuizAttemptId { get; set; }
    public string Answer { get; set; } = string.Empty;

}