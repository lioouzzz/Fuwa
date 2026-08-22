using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Lesson
    {
        [Key]
        public int Id { get; set; }
        public int LessonNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public List<Grammar> Grammars { get; set; } = new();
        public List<Vocabulary> Vocabularies { get; set; } = new();
        public List<QuizAttempt> QuizAttempts { get; set; } = new();
    }
}
