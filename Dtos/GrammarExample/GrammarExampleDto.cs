namespace Dtos.GrammarExample;

public class GrammarExampleDto
{
    public int Id { get; set; }
    public int GrammarId { get; set; }
    public string Japanese { get; set; } = string.Empty;
    public string? Translation { get; set; }

}
