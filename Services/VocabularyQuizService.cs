using Data;
using Services.Interfaces;
using Models;
using Helper;
using Dtos.Quiz;
using Microsoft.EntityFrameworkCore;
namespace Services;


public class VocabularyQuizService : IVocabularyQuizService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<VocabularyQuizService> _logger;

    public VocabularyQuizService(AppDbContext dbContext, ILogger<VocabularyQuizService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<ServiceResult<VocabularyQuizDto>> GenerateQuestion(int lessonId)
    {

        var lessonExists = await _dbContext.Lessons.AnyAsync(x => x.Id == lessonId);

        if (!lessonExists)
        {
            _logger.LogWarning("找不到此測驗對應的課程Id LessonId: {LessonId}", lessonId);

            return new ServiceResult<VocabularyQuizDto>
            {
                Success = false,
                Message = "找不到此測驗對應的課程Id"

            };
        }
        //抓全部單字
        var vocabularies = await _dbContext.Vocabularies
                                         .Where(v => v.LessonId == lessonId)
                                         .ToListAsync();

        var count = vocabularies.Count;

        //要有四個單字當作選項
        if (count < 4)
        {
            _logger.LogWarning("此課程單字數量小於四，無法產生測驗答案");
            return new ServiceResult<VocabularyQuizDto>
            {
                Success = false,
                Message = "此課程單字數量小於四，無法產生測驗答案",
            };
        }



        //隨機選一個當作答案
        var random = new Random();

        // 隨機產生一個 index 隨機數
        var randomIndex = random.Next(count);


        var correctVocabulary = vocabularies[randomIndex];


        //排除答案，再選另外三個
        var wrongAnsers = vocabularies
                            .Where(v => v.Id != correctVocabulary.Id)
                            .OrderBy(v => Guid.NewGuid())
                            .Take(3)
                            .ToList();

        //組合出正確答案＋錯誤答案
        var options = wrongAnsers
            .Select(v => new QuizOptionDto
            {
                VocabularyId = v.Id,
                Text = v.ChineseName
            }).ToList();

        options.Add(new QuizOptionDto
        {
            VocabularyId = correctVocabulary.Id,
            Text = correctVocabulary.ChineseName
        });


        //打亂組合
        options = options
            .OrderBy(option => Guid.NewGuid())
            .ToList();

        //回傳題目
        var dto = new VocabularyQuizDto
        {
            VocabularyId = correctVocabulary.Id,
            Question = correctVocabulary.JapanenseName,
            Options = options
        };

        return new ServiceResult<VocabularyQuizDto>
        {
            Success = true,
            Message = "成功取得測驗題目和答案",
            Data = dto
        };
    }
}