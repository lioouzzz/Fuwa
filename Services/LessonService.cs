using Data;
using Microsoft.EntityFrameworkCore;
using Services.Interfaces;
using Dtos.Lessons;
using Models;
using Helper;

namespace Services;

public class LessonService : ILessonService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<LessonService> _logger;

    public LessonService(AppDbContext dbContext, ILogger<LessonService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<List<LessonDto>> GetAllLessonsAsync()
    {
        return await _dbContext.Lessons
            .AsNoTracking()
            .OrderBy(x => x.LessonNumber)
            .Select(x => new LessonDto
            {
                Id = x.Id,
                Title = x.Title,
                LessonNumber = x.LessonNumber,
                Description = x.Description

            })
            .ToListAsync();
    }

    public async Task<LessonDto?> GetLessonByIdAsync(int id)
    {

        var lesson = await _dbContext.Lessons
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);


        if (lesson == null)
        {
            _logger.LogWarning("找不到指定的課程。ID: {Id}", id);
            return null;
        }

        return new LessonDto
        {
            Id = lesson.Id,
            Title = lesson.Title,
            LessonNumber = lesson.LessonNumber,
            Description = lesson.Description
        };

    }

    public async Task<ServiceResult<LessonDto>> CreateLessonAsync(CreateLessonDto dto)
    {
        var lesson = new Lesson
        {
            Title = dto.Title,
            LessonNumber = dto.LessonNumber,
            Description = dto.Description
        };

        var lessonExist = await _dbContext.Lessons.AnyAsync(x => x.Id == dto.LessonNumber);

        if (lessonExist)
        {
            _logger.LogWarning("此課程已存在，LessonNumber: {LessonNumber}", dto.LessonNumber);

            return new ServiceResult<LessonDto>
            {
                apiResultStatus = ApiResultStatus.Conflict,
                Success = false,
                Message = "此課程已存在"
            };
        }

        _dbContext.Lessons.Add(lesson);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("已新增課程。ID: {Id}, 標題: {Title}", lesson.Id, lesson.Title);

        var lessonDto = new LessonDto
        {
            Id = lesson.Id,
            Title = lesson.Title,
            LessonNumber = lesson.LessonNumber,
            Description = lesson.Description
        };

        return new ServiceResult<LessonDto>
        {
            Success = true,
            Message = "此課程已新增成功",
            Data = lessonDto,
        };

    }

    public async Task<ServiceResult<bool>> UpdateLessonAsync(int id, UpdateLessonDto dto)
    {
        var lesson = await _dbContext.Lessons.FindAsync(id);

        if (lesson == null)
        {
            _logger.LogWarning("找不到指定的課程。ID: {Id}", id);

            return new ServiceResult<bool>
            {
                apiResultStatus = ApiResultStatus.NotFound,
                Success = false,
                Message = "此課程不存在",
            };
        }

        var lessonExist = await _dbContext.Lessons.AnyAsync(l => l.LessonNumber == dto.LessonNumber && l.Id != id);

        if (lessonExist)
        {
            return new ServiceResult<bool>
            {
                apiResultStatus = ApiResultStatus.Conflict,
                Success = false,
                Message = "此課程已存在",
                Data = false
            };
        }

        lesson.Title = dto.Title;
        lesson.LessonNumber = dto.LessonNumber;
        lesson.Description = dto.Description;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("已更新課程。ID: {Id}, 標題: {Title}", lesson.Id, lesson.Title);

        return new ServiceResult<bool>
        {
            Success = true,
            Message = "成功新增課程",
            Data = true
        };
    }


    public async Task<bool> DeleteLessonAsync(int id)
    {
        var lesson = await _dbContext.Lessons.FindAsync(id);

        if (lesson == null)
        {
            _logger.LogWarning("找不到指定的課程。ID: {Id}", id);
            return false;
        }
        _dbContext.Lessons.Remove(lesson);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("已刪除課程。ID: {Id}, 標題: {Title}", lesson.Id, lesson.Title);
        return true;

    }
}