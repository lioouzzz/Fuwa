using Dtos.GrammarExample;
using Helper;
namespace Services.Interfaces;

public interface IGrammarExampleService
{
    Task<List<GrammarExampleDto>> GetAllGrammarExampleAsync();
    Task<GrammarExampleDto> GetGrammarExampleByIdAsync(int id);
    Task<ServiceResult<GrammarExampleDto>> CreateGrammarExample(CreateGrammarExampleDto dto);
    Task<ServiceResult<bool>> UpdateGrammarExample(int id, UpdateGrammarExampleDto dto);
    Task<bool> DeleteGrammarExample(int id);
}