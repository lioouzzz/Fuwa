namespace Dtos.Quiz;



public class StartQuizRequestDto
{
    public int LessonId { get; set; }
    public VocabularyQuizType QuizType { get; set; }

    public int TotalQuestions { get; set; }

}