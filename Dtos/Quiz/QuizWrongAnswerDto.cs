namespace Dtos.Quiz;


public class QuizWrongAnswerDto
{
    public int LessonId { get; set; }
    public int QuizAttemptId { get; set; }

    public VocabularyQuizType Type { get; set; }

    public int VocabularyId { get; set; }

    public string UserAnswer { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;


}