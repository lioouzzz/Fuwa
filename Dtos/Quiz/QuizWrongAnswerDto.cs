namespace Dtos.Quiz;


public class QuizWrongAnswerBookDto
{
    public int LessonId { get; set; }
    public VocabularyQuizType Type { get; set; }

    public int VocabularyId { get; set; }

    public string Question { get; set; } = string.Empty;


    public string CorrectAnswer { get; set; } = string.Empty;

    public int WrongVocabularyCount { get; set; }


}