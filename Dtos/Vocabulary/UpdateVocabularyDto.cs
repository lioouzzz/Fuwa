
namespace Dtos.Vocabulary;

public class UpdateVocabularyDto
{
    public int LessonNumber { get; set; }
    public string JapaneseName { get; set; } = string.Empty;
    public string KanaName { get; set; } = string.Empty;
    public string ChineseName { get; set; } = string.Empty;
    //詞性
    public string PartOfSpeech { get; set; } = string.Empty;

}