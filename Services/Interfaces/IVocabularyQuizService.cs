using Dtos.Quiz;
using Helper;
namespace Services.Interfaces;


public interface IVocabularyQuizService
{
    Task<ServiceResult<StartQuizAnswerResponseDto>> StartQuizAsync(StartQuizRequestDto dto);
    Task<ServiceResult<VocabularyQuizDto>> GenerateVocabularyQuestion(int quizAttemptId);
    Task<ServiceResult<QuizResultDto>> GetVocabularyQuizResult(int quizAttemptId);

    Task<ServiceResult<bool>> SubmitVocabularyAnswer(SubmitVocabularyAnswerDto dto);
}