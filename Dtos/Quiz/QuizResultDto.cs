namespace Dtos.Quiz;



public class QuizResultDto
{
    public int QuizAttemptId { get; set; }
    public int LessonNumber { get; set; }
    public VocabularyQuizType QuizType { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectCount { get; set; }
    public int WrongCount { get; set; }
    public double Accuracy { get; set; }
    public int DurationSeconds { get; set; }
}