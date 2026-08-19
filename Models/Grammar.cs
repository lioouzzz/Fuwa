using System.ComponentModel.DataAnnotations;


namespace Models
{
    public class Grammar
    {
        [Key]
        public int Id { get; set; }
        public int LessonId { get; set; }
        public string GrammarName { get; set; } = string.Empty;
        public string GrammarChineseName { get; set; } = string.Empty;
        //詞性

        public Lesson Lesson { get; set; } = null!;

        public List<GrammarExample> GrammarExamples { get; set; } = new();

    }
}
