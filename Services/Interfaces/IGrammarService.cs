using Dtos.Grammar;
using Helper;
namespace Services.Interfaces;

public interface IGrammarService
{
    Task<List<GrammarDto>> GetAllGrammarsAsync();
    Task<GrammarDto?> GetGrammarByIdAsync(int id);
    Task<ServiceResult<GrammarDto>> CreateGrammarAsync(CreateGrammarDto dto);
    Task<ServiceResult<bool>> UpdateGrammarAsync(int id, UpdateGrammarDto dto);

    Task<bool> DeleteGrammarAsync(int id);
}