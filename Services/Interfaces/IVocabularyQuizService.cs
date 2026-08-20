using Dtos.Quiz;
using Helper;
namespace Services.Interfaces;


public interface IVocabularyQuizService
{
    Task<ServiceResult<VocabularyQuizDto>> GenerateQuestion(int lessonId);
    Task<ServiceResult<bool>> SubmitVocabularyAnswer(SubmitVocabularyAnswerDto dto);
}