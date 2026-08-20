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



    [HttpGet]
    public async Task<IActionResult> GenerateQuestion(int lessonId)
    {
        var result = await _quizservice.GenerateQuestion(lessonId);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }


    [HttpPost]
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

        return Ok(new
        {
            Data = result
        });
    }
}