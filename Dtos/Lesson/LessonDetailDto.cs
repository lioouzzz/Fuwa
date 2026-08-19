using System.ComponentModel.DataAnnotations;
using Dtos.Grammar;
using Dtos.Vocabulary;

namespace Dtos.Lessons;

public class LessonDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int LessonNumber { get; set; }
    public string? Description { get; set; }

    public List<VocabularyDto> Vocabularies { get; set; }
    public List<GrammarDto> Grammars { get; set; }

}