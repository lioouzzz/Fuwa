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

    public async Task<ServiceResult<VocabularyQuizDto>> GenerateQuestion(int lessonId, VocabularyQuizType type)
    {

        var lessonExists = await _dbContext.Lessons.AnyAsync(x => x.Id == lessonId);

        if (!lessonExists)
        {
            _logger.LogWarning("找不到此測驗對應的課程Id LessonId: {LessonId}", lessonId);

            return new ServiceResult<VocabularyQuizDto>
            {
                apiResultStatus = ApiResultStatus.NotFound,
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
                apiResultStatus = ApiResultStatus.NotFound,
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


        List<QuizOptionDto> options;


        switch (type)
        {
            case VocabularyQuizType.ChineseToJapanese:
                options = wrongAnsers
                  .Select(v => new QuizOptionDto
                  {
                      VocabularyId = v.Id,
                      Text = v.JapanenseName
                  }).ToList();
                options.Add(new QuizOptionDto
                {
                    VocabularyId = correctVocabulary.Id,
                    Text = correctVocabulary.JapanenseName
                });
                break;

            case VocabularyQuizType.JapaneseToChinese:
                options = wrongAnsers
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

                break;

            case VocabularyQuizType.HiraganaToKana:
                options = wrongAnsers
                .Select(v => new QuizOptionDto
                {
                    VocabularyId = v.Id,
                    Text = v.KanaName
                }).ToList();

                options.Add(new QuizOptionDto
                {
                    VocabularyId = correctVocabulary.Id,
                    Text = correctVocabulary.KanaName
                });

                break;

            default:
                return new ServiceResult<VocabularyQuizDto>
                {
                    Success = false,
                    Message = "不存在的測驗類型"
                };


        }


        string questions;

        switch (type)
        {
            case VocabularyQuizType.ChineseToJapanese:
                questions = correctVocabulary.ChineseName;
                break;

            case VocabularyQuizType.JapaneseToChinese:
                questions = correctVocabulary.JapanenseName;
                break;

            case VocabularyQuizType.HiraganaToKana:
                questions = correctVocabulary.JapanenseName;
                break;

            default:
                return new ServiceResult<VocabularyQuizDto>
                {
                    Success = false,
                    Message = "不存在的測驗類型"
                };
        }


        //回傳題目
        var dto = new VocabularyQuizDto
        {
            VocabularyId = correctVocabulary.Id,
            Question = questions,
            Type = type,
            Options = options
        };

        return new ServiceResult<VocabularyQuizDto>
        {
            Success = true,
            Message = "成功取得測驗題目和答案",
            Data = dto
        };
    }

    public async Task<ServiceResult<bool>> SubmitVocabularyAnswer(SubmitVocabularyAnswerDto dto)
    {
        var answer = await _dbContext.Vocabularies
                         .FirstOrDefaultAsync(v => v.Id == dto.VocabularyId);

        if (answer == null)
        {
            _logger.LogWarning("此為不存在的單字，請重新輸入");

            return new ServiceResult<bool>
            {
                apiResultStatus = ApiResultStatus.NotFound,
                Success = false,
                Message = "此為不存在的單字，請重新輸入"
            };
        }

        bool answerResult;

        switch (dto.Type)
        {
            case VocabularyQuizType.ChineseToJapanese:
                answerResult = dto.Answer == answer.JapanenseName;
                break;

            case VocabularyQuizType.JapaneseToChinese:
                answerResult = dto.Answer == answer.ChineseName;
                break;

            case VocabularyQuizType.HiraganaToKana:
                answerResult = dto.Answer == answer.KanaName;
                break;

            default:
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "不存在的測驗類型",
                    Data = false
                };
        }


        if (!answerResult)
        {
            _logger.LogInformation("測驗結果錯誤");

            return new ServiceResult<bool>
            {
                apiResultStatus = ApiResultStatus.Validation,
                Success = false,
                Message = "測驗結果錯誤",
                Data = false
            };
        }

        return new ServiceResult<bool>
        {
            Success = true,
            Message = "測驗答案結果正確",
            Data = answerResult
        };

    }

}