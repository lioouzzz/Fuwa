using Data;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Dtos.Vocabulary;
using Models;
using Helper;
namespace Services;

public class VocabularyService : IVocabularyService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<VocabularyService> _logger;

    public VocabularyService(AppDbContext dbContext, ILogger<VocabularyService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<List<VocabularyDto>> GetAllVocabulariesAsync()
    {
        return await _dbContext.Vocabularies
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new VocabularyDto
            {
                Id = x.Id,
                LessonNumber = x.Lesson.LessonNumber,
                JapaneseName = x.JapanenseName,
                KanaName = x.KanaName,
                ChineseName = x.ChineseName,
                PartOfSpeech = x.PartOfSpeech
            }).ToListAsync();
    }

    public async Task<VocabularyDto?> GetVocabularyByIdAsync(int id)
    {
        var vocabulary = await _dbContext.Vocabularies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (vocabulary == null)
        {
            _logger.LogWarning("找不到指定的詞彙。ID: {Id}", id);
            return null;
        }

        return new VocabularyDto
        {
            Id = vocabulary.Id,
            LessonNumber = vocabulary.Lesson.LessonNumber,
            JapaneseName = vocabulary.JapanenseName,
            KanaName = vocabulary.KanaName,
            ChineseName = vocabulary.ChineseName,
            PartOfSpeech = vocabulary.PartOfSpeech
        };
    }


    public async Task<ServiceResult<VocabularyDto>> CreateVocabularyAsync(CreateVocabularyDto dto)
    {
        var lesson = await _dbContext.Lessons.FirstOrDefaultAsync(l => l.LessonNumber == dto.LessonNumber);

        if (lesson == null)
        {
            _logger.LogWarning("單字插入失敗，此課程Number: {LessonNumber} 不存在", dto.LessonNumber);

            return new ServiceResult<VocabularyDto>
            {
                apiResultStatus = ApiResultStatus.NotFound,
                Success = false,
                Message = "指定的課程不存在",

            };
        }

        var vocabularyExist = await _dbContext.Vocabularies.AnyAsync(x => x.LessonId == lesson.Id && x.JapanenseName == dto.JapaneseName);


        if (vocabularyExist)
        {
            _logger.LogWarning("此單字已存在 課程ID: LessonId: {id},單字名稱: Vocabulary: {vocabulary}", lesson.Id, dto.JapaneseName);

            return new ServiceResult<VocabularyDto>
            {
                apiResultStatus = ApiResultStatus.Conflict,
                Success = false,
                Message = "此單字已存在"
            };
        }


        var vocabulary = new Vocabulary
        {
            LessonId = lesson.Id,
            JapanenseName = dto.JapaneseName,
            KanaName = dto.KanaName,
            ChineseName = dto.ChineseName,
            PartOfSpeech = dto.PartOfSpeech
        };


        _dbContext.Vocabularies.Add(vocabulary);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("已新增詞彙。ID: {Id}, 日文名稱: {JapanenseName}", vocabulary.Id, vocabulary.JapanenseName);
        var vocabularyDto = new VocabularyDto
        {
            Id = vocabulary.Id,
            LessonNumber = vocabulary.Lesson.LessonNumber,
            JapaneseName = vocabulary.JapanenseName,
            KanaName = vocabulary.KanaName,
            ChineseName = vocabulary.ChineseName,
            PartOfSpeech = vocabulary.PartOfSpeech
        };

        return new ServiceResult<VocabularyDto>
        {
            Success = true,
            Message = "單字新增成功",
            Data = vocabularyDto
        };
    }

    public async Task<ServiceResult<bool>> UpdateVocabularyAsync(int id, UpdateVocabularyDto dto)
    {
        var lesson = await _dbContext.Lessons.FirstOrDefaultAsync(l => l.LessonNumber == dto.LessonNumber);

        if (lesson == null)
        {
            _logger.LogWarning("找不到指定課程 ID: {id}", dto.LessonNumber);

            return new ServiceResult<bool>
            {
                apiResultStatus = ApiResultStatus.NotFound,
                Success = false,
                Message = "此課程不存在"
            };
        }

        var vocabulary = await _dbContext.Vocabularies.FindAsync(id);

        if (vocabulary == null)
        {
            _logger.LogWarning("找不到指定的單字詞彙。ID: {Id}", id);
            return new ServiceResult<bool>
            {
                apiResultStatus = ApiResultStatus.NotFound,
                Success = false,
                Message = "此單字詞彙不存在"
            };
        }

        vocabulary.LessonId = lesson.Id;
        vocabulary.JapanenseName = dto.JapaneseName;
        vocabulary.KanaName = dto.KanaName;
        vocabulary.ChineseName = dto.ChineseName;
        vocabulary.PartOfSpeech = dto.PartOfSpeech;

        await _dbContext.SaveChangesAsync();

        return new ServiceResult<bool>
        {
            Success = true,
            Message = "更新單字詞彙成功",
            Data = true
        };
    }

    public async Task<bool> DeleteVocabularyAsync(int id)
    {
        var vocabulary = await _dbContext.Vocabularies.FindAsync(id);

        if (vocabulary == null)
        {
            _logger.LogWarning("找不到指定的詞彙。ID: {Id}", id);
            return false;
        }
        _dbContext.Vocabularies.Remove(vocabulary);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("已刪除詞彙。ID: {Id}, 日文名稱: {JapanenseName}", vocabulary.Id, vocabulary.JapanenseName);
        return true;
    }
}