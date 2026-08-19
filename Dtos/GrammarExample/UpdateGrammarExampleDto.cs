namespace Dtos.GrammarExample;

public class UpdateGrammarExampleDto
{
    public int GrammarId { get; set; }
    public string Japanese { get; set; } = string.Empty;
    public string? Translation { get; set; }

}
