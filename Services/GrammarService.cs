using Data;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Dtos.Grammar;
using Models;
using Helper;
using Dtos.GrammarExample;
namespace Services;


public class GrammarService : IGrammarService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<GrammarService> _logger;
    public GrammarService(AppDbContext dbContext, ILogger<GrammarService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<List<GrammarDto>> GetAllGrammarsAsync()
    {
        return await _dbContext.Grammars
            .AsNoTracking()
            .OrderBy(grammar => grammar.Id)
            .Select(grammar => new GrammarDto
            {
                Id = grammar.Id,
                LessonNumber = grammar.Lesson.LessonNumber,
                GrammarName = grammar.GrammarName,
                GrammarChineseName = grammar.GrammarChineseName,
                GrammarExamples = grammar.GrammarExamples
                                    .OrderBy(example => example.Id)
                                    .Select(example => new GrammarExampleDto
                                    {
                                        Id = example.Id,
                                        Japanese = example.Japanese,
                                        GrammarId = example.GrammarId,
                                        Translation = example.Translation
                                    }).ToList()
            }).ToListAsync();
    }

    public async Task<GrammarDto?> GetGrammarByIdAsync(int id)
    {
        var grammar = await _dbContext.Grammars
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(grammar => new GrammarDto
                {
                    Id = grammar.Id,
                    LessonNumber = grammar.Lesson.LessonNumber,
                    GrammarName = grammar.GrammarName,
                    GrammarChineseName = grammar.GrammarChineseName,
                    GrammarExamples = grammar.GrammarExamples
                                            .OrderBy(example => example.Id)
                                            .Select(example => new GrammarExampleDto
                                            {
                                                Id = example.Id,
                                                GrammarId = example.GrammarId,
                                                Japanese = example.Japanese,
                                                Translation = example.Translation
                                            }).ToList()
                }).FirstOrDefaultAsync();

        if (grammar == null)
        {
            _logger.LogWarning("找不到指定的文法。ID: {Id}", id);
            return null;
        }

        return grammar;
    }


    public async Task<ServiceResult<GrammarDto>> CreateGrammarAsync(CreateGrammarDto dto)
    {
        var lesson = await _dbContext.Lessons.FirstOrDefaultAsync(l => l.LessonNumber == dto.LessonNumber);

        if (lesson == null)
        {
            _logger.LogWarning("Grammar插入失敗，此課程Number: {LessonNumber} 不存在", dto.LessonNumber);

            return new ServiceResult<GrammarDto>
            {
                apiResultStatus = ApiResultStatus.NotFound,
                Success = false,
                Message = "指定的課程不存在",

            };
        }

        var grammarExist = await _dbContext.Grammars.AnyAsync(g => g.Lesson.LessonNumber == dto.LessonNumber && g.GrammarName == dto.GrammarName);

        if (grammarExist)
        {
            _logger.LogWarning("Grammar插入失敗，此文法已存在 課程ID: LessonNumber: {LessonNumber},文法名稱: GrammarName: {grammarId}", dto.LessonNumber, dto.GrammarName);

            return new ServiceResult<GrammarDto>
            {
                apiResultStatus = ApiResultStatus.Conflict,
                Success = false,
                Message = "此文法已存在"
            };
        }

        var grammar = new Grammar
        {
            LessonId = lesson.Id,
            GrammarName = dto.GrammarName,
            GrammarChineseName = dto.GrammarChineseName
        };

        _dbContext.Grammars.Add(grammar);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Grammar新增成功， GrammarId: {GrammarId}", grammar.Id);

        var grammarDto = new GrammarDto
        {
            LessonNumber = grammar.Lesson.LessonNumber,
            GrammarName = grammar.GrammarName,
            GrammarChineseName = grammar.GrammarChineseName
        };

        return new ServiceResult<GrammarDto>
        {
            Success = true,
            Message = "新增文法成功",
            Data = grammarDto
        };

    }


    public async Task<ServiceResult<bool>> UpdateGrammarAsync(int id, UpdateGrammarDto dto)

    {
        var lesson = await _dbContext.Lessons.FirstOrDefaultAsync(l => l.LessonNumber == dto.LessonNumber);

        if (lesson == null)
        {
            _logger.LogWarning("找不到指定課程 LessonNumber: {LessonNumber}", dto.LessonNumber);

            return new ServiceResult<bool>
            {
                apiResultStatus = ApiResultStatus.NotFound,
                Success = false,
                Message = "此課程不存在"
            };
        }


        var grammar = await _dbContext.Grammars.FindAsync(id);


        if (grammar == null)
        {
            _logger.LogWarning("找不到指定課程文法 ID: {id}", id);

            return new ServiceResult<bool>
            {
                apiResultStatus = ApiResultStatus.NotFound,
                Success = false,
                Message = "指定的文法不存在",
            };
        }


        grammar.LessonId = lesson.Id;
        grammar.GrammarName = dto.GrammarName;
        grammar.GrammarChineseName = dto.GrammarChineseName;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("已更新課程文法 ID: {Id}, LessonID: {LessonId},文法名稱: {GrammarName}", grammar.Id, grammar.LessonId, grammar.GrammarName);

        return new ServiceResult<bool>
        {
            Success = true,
            Message = "指定課程文法更新成功",
            Data = true
        };
    }


    public async Task<bool> DeleteGrammarAsync(int id)
    {
        var grammar = await _dbContext.Grammars.FindAsync(id);

        if (grammar == null)
        {
            _logger.LogWarning("找不到指定課程文法 ID:{Id}", id);
            return false;
        }

        _dbContext.Grammars.Remove(grammar);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("指定課程文法已刪除  ID:{id},課程代號:{LessonId},文法名稱:{GrammarName}", grammar.Id, grammar.LessonId, grammar.GrammarName);
        return true;
    }

}



