using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Dtos.Vocabulary;
using Dtos.Grammar;
using Helper;
namespace Controllers;

[ApiController]
[Route("api/[controller]")]

public class GrammarController : ControllerBase
{
    private readonly IGrammarService _grammarService;

    public GrammarController(IGrammarService grammarService)
    {
        _grammarService = grammarService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllGrammars()
    {
        var grammars = await _grammarService.GetAllGrammarsAsync();

        return Ok(grammars);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetGrammarById([FromRoute] int id)
    {
        var grammar = await _grammarService.GetGrammarByIdAsync(id);

        if (grammar == null)
        {
            return NotFound(new
            {
                statusCode = 404,
                message = "找不到指定的文法"
            });
        }
        return Ok(grammar);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGrammar([FromBody] CreateGrammarDto dto)
    {
        var grammar = await _grammarService.CreateGrammarAsync(dto);

        if (!grammar.Success)
        {
            if (grammar.apiResultStatus == ApiResultStatus.Conflict)
            {
                return Conflict(new { message = grammar.Message });
            }

            if (grammar.apiResultStatus == ApiResultStatus.NotFound)
            {
                return NotFound(new { message = grammar.Message });
            }

            return BadRequest(new { message = grammar.Message });
        }

        return CreatedAtAction(nameof(GetGrammarById), new { id = grammar.Data.Id }, grammar);

    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGrammar([FromRoute] int id, [FromBody] UpdateGrammarDto dto)
    {
        var result = await _grammarService.UpdateGrammarAsync(id, dto);


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
    public async Task<IActionResult> DeleteGrammar([FromRoute] int id)
    {
        var result = await _grammarService.DeleteGrammarAsync(id);

        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
}