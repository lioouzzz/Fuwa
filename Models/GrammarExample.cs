using System.ComponentModel.DataAnnotations;


namespace Models
{
    public class GrammarExample
    {
        [Key]
        public int Id { get; set; }
        public int GrammarId { get; set; }
        public string Japanese { get; set; } = string.Empty;
        public string? Translation { get; set; }
        public Grammar Grammar { get; set; } = null!;

    }
}

