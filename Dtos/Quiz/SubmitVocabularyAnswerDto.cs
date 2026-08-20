namespace Dtos.Quiz;



public class SubmitVocabularyAnswerDto
{
    public int VocabularyId { get; set; }

    //public VocabularyQuizType Type { get; set; }
    public string Anser { get; set; } = string.Empty;

}