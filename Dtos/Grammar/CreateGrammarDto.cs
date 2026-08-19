using System.ComponentModel.DataAnnotations;


namespace Dtos.Grammar;

public class CreateGrammarDto
{

    public int LessonNumber { get; set; }
    public string GrammarName { get; set; } = string.Empty;
    public string GrammarChineseName { get; set; } = string.Empty;
}

