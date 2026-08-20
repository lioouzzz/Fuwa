using Dtos.Quiz;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

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
}