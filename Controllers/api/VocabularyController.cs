using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Dtos.Vocabulary;
using Helper;
namespace Controllers;

[ApiController]
[Route("api/[controller]")]

public class VocabularyController : ControllerBase
{
    private readonly IVocabularyService _vocabularyService;

    public VocabularyController(IVocabularyService vocabularyService)
    {
        _vocabularyService = vocabularyService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllVocabularies()
    {
        var result = await _vocabularyService.GetAllVocabulariesAsync();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVocabularyById(int id)
    {
        var result = await _vocabularyService.GetVocabularyByIdAsync(id);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateVocabulary([FromBody] CreateVocabularyDto dto)
    {
        var result = await _vocabularyService.CreateVocabularyAsync(dto);

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
        return CreatedAtAction(nameof(GetVocabularyById), new { id = result.Data.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateVocabulary([FromRoute] int id, [FromBody] UpdateVocabularyDto dto)
    {
        var result = await _vocabularyService.UpdateVocabularyAsync(id, dto);


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
    public async Task<IActionResult> DeleteVocabulary([FromRoute] int id)
    {
        var result = await _vocabularyService.DeleteVocabularyAsync(id);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}