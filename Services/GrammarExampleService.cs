using Services.Interfaces;
using Helper;
using Data;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Dtos.GrammarExample;
using Models;

namespace Services;

public class GrammarExampleService : IGrammarExampleService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<GrammarExampleService> _logger;

    public GrammarExampleService(AppDbContext dbContext, ILogger<GrammarExampleService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<List<GrammarExampleDto>> GetAllGrammarExampleAsync()
    {
        return await _dbContext.GrammarExamples
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new GrammarExampleDto
            {
                Id = x.Id,
                GrammarId = x.GrammarId,
                Japanese = x.Japanese,
                Translation = x.Translation
            }).ToListAsync();

    }

    public async Task<GrammarExampleDto> GetGrammarExampleByIdAsync(int id)
    {
        var grammarExample = await _dbContext.GrammarExamples.FindAsync(id);

        if (grammarExample == null)
        {
            _logger.LogWarning("找不到指定的文法例句。ID: {Id}", id);

            return null;

        }

        return new GrammarExampleDto
        {
            Id = grammarExample.Id,
            GrammarId = grammarExample.GrammarId,
            Japanese = grammarExample.Japanese,
            Translation = grammarExample.Translation
        };

    }

    public async Task<ServiceResult<GrammarExampleDto>> CreateGrammarExample(CreateGrammarExampleDto dto)
    {
        var grammar = await _dbContext.Grammars.FirstOrDefaultAsync(x => x.Id == dto.GrammarId);

        if (grammar == null)
        {
            _logger.LogWarning("文法例句插入失敗，找不到此文法 GrammarId: {GrammarId}", dto.GrammarId);

            return new ServiceResult<GrammarExampleDto>
            {
                apiResultStatus = ApiResultStatus.NotFound,
                Success = false,
                Message = "指定的文法不存在"
            };
        }

        var grammarExampleExist = await _dbContext.GrammarExamples.AnyAsync(x => x.GrammarId == dto.GrammarId && x.Japanese == dto.Japanese);

        if (grammarExampleExist)
        {
            _logger.LogWarning("此文法例句已存在");

            return new ServiceResult<GrammarExampleDto>
            {
                apiResultStatus = ApiResultStatus.Conflict,
                Success = false,
                Message = "此文法例句已存在"
            };
        }

        var grammarExample = new GrammarExample
        {
            GrammarId = dto.GrammarId,
            Japanese = dto.Japanese,
            Translation = dto.Translation
        };

        _dbContext.GrammarExamples.Add(grammarExample);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("已新增文法例句。ID: {Id}, Japanese: {Japanese}", grammarExample.Id, grammarExample.Japanese);

        var grammarExampleDto = new GrammarExampleDto
        {
            Id = grammarExample.Id,
            GrammarId = grammarExample.GrammarId,
            Translation = grammarExample.Translation
        };

        return new ServiceResult<GrammarExampleDto>
        {
            Success = true,
            Message = "文法例句新增成功",
            Data = grammarExampleDto
        };
    }

    public async Task<ServiceResult<bool>> UpdateGrammarExample(int id, UpdateGrammarExampleDto dto)
    {
        var grammar = await _dbContext.Grammars.FirstOrDefaultAsync(x => x.Id == dto.GrammarId);

        if (grammar == null)
        {
            _logger.LogWarning("找不到此文法，GrammarId: {GrammarId}", dto.GrammarId);

            return new ServiceResult<bool>
            {
                apiResultStatus = ApiResultStatus.NotFound,
                Success = false,
                Message = "找不到此文法"
            };
        }

        var grammarExample = await _dbContext.GrammarExamples.FindAsync(id);

        if (grammarExample == null)
        {
            _logger.LogWarning("找不到指定的文法例句。ID: {Id}", id);

            return new ServiceResult<bool>
            {
                apiResultStatus = ApiResultStatus.NotFound,
                Success = false,
                Message = "此文法例句不存在"
            };


        }

        grammarExample.GrammarId = dto.GrammarId;
        grammarExample.Japanese = dto.Japanese;
        grammarExample.Translation = dto.Translation;

        await _dbContext.SaveChangesAsync();

        return new ServiceResult<bool>
        {
            Success = true,
            Message = "更新文法例句成功",
            Data = true
        };

    }

    public async Task<bool> DeleteGrammarExample(int id)
    {
        var grammarExample = await _dbContext.GrammarExamples.FindAsync(id);

        if (grammarExample == null)
        {
            _logger.LogWarning("文法例句不存在 ID: {Id}", id);

            return false;
        }

        _dbContext.GrammarExamples.Remove(grammarExample);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("已刪除文法例句。ID: {Id}, 文法例句: {grammarExample}", grammarExample.Id, grammarExample.Japanese);

        return true;
    }
}