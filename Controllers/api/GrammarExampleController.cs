using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Dtos.Vocabulary;
using Dtos.Grammar;
using Helper;
namespace Controllers;

using Dtos.GrammarExample;

[ApiController]
[Route("api/[controller]")]

public class GrammarExampleController : ControllerBase
{
    private readonly IGrammarExampleService _grammarExampleService;

    public GrammarExampleController(IGrammarExampleService igrammarExampleService)
    {
        _grammarExampleService = igrammarExampleService;
    }


    [HttpGet]
    public async Task<IActionResult> GetAllGrammarExamples()
    {
        var result = await _grammarExampleService.GetAllGrammarExampleAsync();

        return Ok(result);

    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAllGrammarExampleById([FromRoute] int id)
    {
        var result = await _grammarExampleService.GetGrammarExampleByIdAsync(id);

        if (result == null)
        {
            return NotFound(new
            {
                StatusCode = 404,
                message = "找不到指定的文法例句"
            });
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGrammarExample([FromBody] CreateGrammarExampleDto dto)
    {
        var result = await _grammarExampleService.CreateGrammarExample(dto);

        if (!result.Success)
        {
            if (!result.Success)
            {
                if (result.apiResultStatus == ApiResultStatus.Conflict)
                {
                    return Conflict(new { message = result.Message });
                }

                if (result.apiResultStatus == ApiResultStatus.NotFound)
                {
                    return NotFound(new { message = result.Message });
                }

                return BadRequest(new { message = result.Message });
            }
        }

        return CreatedAtAction(nameof(GetAllGrammarExampleById), new { id = result.Data.Id }, result);

    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGrammarExample([FromRoute] int id, [FromBody] UpdateGrammarExampleDto dto)
    {
        var result = await _grammarExampleService.UpdateGrammarExample(id, dto);

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
    public async Task<IActionResult> DeleteGrammarExample([FromRoute] int id)
    {
        var result = await _grammarExampleService.DeleteGrammarExample(id);

        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
}