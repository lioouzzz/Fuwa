using System.ComponentModel.DataAnnotations;


namespace Models
{
    public class Vocabulary
    {
        [Key]
        public int Id { get; set; }
        public int LessonId { get; set; }
        public string JapanenseName { get; set; } = string.Empty;
        public string KanaName { get; set; } = string.Empty;
        public string ChineseName { get; set; } = string.Empty;

        //詞性
        public string PartOfSpeech { get; set; } = string.Empty;

        public Lesson Lesson { get; set; } = null!;
        public List<QuizAnswer> QuizAnswers = new();
    }
}
