using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Dtos.Lessons;
using Helper;

namespace Controllers;

[ApiController]
[Route("api/[controller]")]

public class LessonsController : ControllerBase
{
    private readonly ILessonService _lessonService;

    public LessonsController(ILessonService lessonService)
    {
        _lessonService = lessonService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllLessons()
    {
        var lessons = await _lessonService.GetAllLessonsAsync();

        return Ok(lessons);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLessonById(int id)
    {
        var lesson = await _lessonService.GetLessonByIdAsync(id);

        if (lesson == null)
        {
            return NotFound();
        }

        return Ok(lesson);
    }

    [HttpPost]
    public async Task<IActionResult> CreateLesson([FromBody] CreateLessonDto dto)
    {
        var lesson = await _lessonService.CreateLessonAsync(dto);

        if (!lesson.Success)
        {
            if (lesson.apiResultStatus == ApiResultStatus.Conflict)
            {
                return Conflict(new
                {
                    message = lesson.Message
                });
            }

            if (lesson.apiResultStatus == ApiResultStatus.NotFound)
            {
                return NotFound(new
                {
                    message = lesson.Message
                });
            }
        }
        // CreatedAtAction 方法會回傳 201 Created 狀態碼，並在 Location 標頭中包含新資源的 URI
        // 用哪個方法找 | 要用哪個 ID 找 |  剛新增的東西
        return CreatedAtAction(nameof(GetLessonById), new { id = lesson.Data.Id }, lesson);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLesson([FromRoute] int id, [FromBody] UpdateLessonDto dto)
    {
        var result = await _lessonService.UpdateLessonAsync(id, dto);

        if (!result.Success)
        {
            if (result.apiResultStatus == ApiResultStatus.Conflict)
            {
                return Conflict(new
                {
                    message = result.Message
                });
            }

            if (result.apiResultStatus == ApiResultStatus.NotFound)
            {
                return NotFound(new
                {
                    message = result.Message
                });
            }
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLesson(int id)
    {
        var result = await _lessonService.DeleteLessonAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }


}