using Dtos.Quiz;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Helper;
[ApiController]
[Route("api/[controller]")]
public class VocabularyQuizController : ControllerBase
{
    private readonly IVocabularyQuizService _quizservice;
    public VocabularyQuizController(IVocabularyQuizService service)
    {
        _quizservice = service;
    }

    [HttpPost("start")]
    public async Task<IActionResult> StartQuiz(StartQuizRequestDto dto)
    {
        var result = await _quizservice.StartQuizAsync(dto);

        if (!result.Success)
        {
            if (result.apiResultStatus == ApiResultStatus.Conflict)
            {
                return Conflict(new
                {
                    Success = false,
                    message = result.Message

                });
            }

            if (result.apiResultStatus == ApiResultStatus.NotFound)
            {
                return NotFound(new
                {
                    Success = false,
                    message = result.Message

                });
            }

            if (result.apiResultStatus == ApiResultStatus.Validation)
            {
                return NotFound(new
                {
                    Success = false,
                    message = result.Message

                });
            }

        }

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GenerateQuestion(int quizAttemptId)
    {
        var result = await _quizservice.GenerateVocabularyQuestion(quizAttemptId);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }


    [HttpPost("answer")]
    public async Task<IActionResult> SubmitAnswer(SubmitVocabularyAnswerDto dto)
    {
        var result = await _quizservice.SubmitVocabularyAnswer(dto);

        if (!result.Success)
        {
            if (result.apiResultStatus == ApiResultStatus.Conflict)
            {
                return Conflict(new
                {
                    Success = false,
                    message = result.Message

                });
            }

            if (result.apiResultStatus == ApiResultStatus.NotFound)
            {
                return NotFound(new
                {
                    Success = false,
                    message = result.Message

                });
            }

            if (result.apiResultStatus == ApiResultStatus.Validation)
            {
                return NotFound(new
                {
                    Success = false,
                    message = result.Message

                });
            }

        }

        return Ok(result);
    }
}