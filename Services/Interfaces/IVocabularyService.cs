
using Dtos.Vocabulary;
using Helper;
namespace Services.Interfaces;



public interface IVocabularyService
{
    Task<List<VocabularyDto>> GetAllVocabulariesAsync();
    Task<VocabularyDto?> GetVocabularyByIdAsync(int id);
    Task<ServiceResult<VocabularyDto>> CreateVocabularyAsync(CreateVocabularyDto dto);
    Task<ServiceResult<bool>> UpdateVocabularyAsync(int id, UpdateVocabularyDto dto);
    Task<bool> DeleteVocabularyAsync(int id);
}






