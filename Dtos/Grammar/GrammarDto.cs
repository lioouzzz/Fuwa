using Dtos.GrammarExample;
namespace Dtos.Grammar;

public class GrammarDto
{
    public int Id { get; set; }

    public int LessonNumber { get; set; }

    public string GrammarName { get; set; } = string.Empty;

    public string GrammarChineseName { get; set; } = string.Empty;

    public List<GrammarExampleDto> GrammarExamples { get; set; } = new();
}
