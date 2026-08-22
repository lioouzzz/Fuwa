using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class QuizAttempt
    {
        [Key]
        public int Id { get; set; }
        public int LessonId { get; set; }
        public VocabularyQuizType QuizType { get; set; }
        public int TotalQuestions { get; set; }

        public int CorrectCount { get; set; }

        public DateTime StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public Lesson Lesson { get; set; } = null!;
        public List<QuizAnswer> QuizAnswers { get; set; } = new();
    }
}