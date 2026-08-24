using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class QuizAnswer
    {
        [Key]
        public int Id { get; set; }
        public int QuizAttemptId { get; set; }
        public int VocabularyId { get; set; }

        public string UserAnswer { get; set; } = string.Empty;

        public bool IsCorrect { get; set; }

        public QuizAttempt QuizAttempt { get; set; } = null!;
        public Vocabulary Vocabulary { get; set; } = null!;
    }
}