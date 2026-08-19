using System.ComponentModel.DataAnnotations;


namespace Dtos.Grammar;

public class UpdateGrammarDto
{
    public int LessonNumber { get; set; }
    public string GrammarName { get; set; } = string.Empty;
    public string GrammarChineseName { get; set; } = string.Empty;
}