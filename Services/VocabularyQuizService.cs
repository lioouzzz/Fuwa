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

    //開始測驗API
    public async Task<ServiceResult<StartQuizAnswerResponseDto>> StartQuizAsync(StartQuizRequestDto dto)
    {
        var lessonExists = await _dbContext.Lessons
                                    .AnyAsync(l => l.Id == dto.LessonId);

        if (!lessonExists)
        {
            _logger.LogWarning("測驗的課程不存在,測驗Id: {Id}", dto.LessonId);

            return new ServiceResult<StartQuizAnswerResponseDto>
            {
                apiResultStatus = ApiResultStatus.NotFound,
                Success = false,
                Message = "找不到此測驗對應的課程Id"

            };
        }

        // 2. 檢查 QuizType
        if (!Enum.IsDefined(typeof(VocabularyQuizType), dto.QuizType))
        {
            return new ServiceResult<StartQuizAnswerResponseDto>
            {
                Success = false,
                Message = "測驗類型不存在"
            };
        }

        if (dto.TotalQuestions < 0)
        {
            _logger.LogWarning("測驗的總題數輸入小於0,TotalQuestions: {TotalQuestions}", dto.TotalQuestions);

            return new ServiceResult<StartQuizAnswerResponseDto>
            {
                apiResultStatus = ApiResultStatus.NotFound,
                Success = false,
                Message = "測驗的總題數輸入小於0"

            };

        }



        var quizAttempt = new QuizAttempt
        {
            LessonId = dto.LessonId,
            QuizType = dto.QuizType,
            TotalQuestions = dto.TotalQuestions,
            CorrectCount = 0,
            StartedAt = DateTime.UtcNow,
            CompletedAt = null
        };

        _dbContext.QuizAttempt.Add(quizAttempt);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("已新增quizAttempt資料。ID: {Id}", quizAttempt.Id);


        var resDto = new StartQuizAnswerResponseDto
        {
            QuizAttemptId = quizAttempt.Id
        };

        return new ServiceResult<StartQuizAnswerResponseDto>
        {
            Success = true,
            Message = "成功建立開始測驗API",
            Data = resDto
        };

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
                questions = correctVocabulary.KanaName;
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

        //找這次測驗
        var quizAttempt = await _dbContext.QuizAttempt.FirstOrDefaultAsync(q => q.Id == dto.QuizAttemptId);

        if (quizAttempt == null)
        {
            return new ServiceResult<bool>
            {
                Success = false,
                Message = "找不到此測驗"
            };
        }

        // 先判斷此測驗是否已完成
        if (quizAttempt.CompletedAt != null)
        {
            return new ServiceResult<bool>
            {
                Success = false,
                Message = "此測驗已完成"
            };
        }

        //計算目前已經回答幾題
        var answeredCount = await _dbContext.QuizAnswer
            .CountAsync(a => a.QuizAttemptId == quizAttempt.Id);



        // 防止超過總題數
        if (answeredCount >= quizAttempt.TotalQuestions)
        {
            return new ServiceResult<bool>
            {
                Success = false,
                Message = "此測驗已達作答題數上限"
            };
        }


        var vocabulary = await _dbContext.Vocabularies
                         .FirstOrDefaultAsync(v => v.Id == dto.VocabularyId);

        if (vocabulary == null)
        {
            _logger.LogWarning("此為不存在的單字，請重新輸入");

            return new ServiceResult<bool>
            {
                apiResultStatus = ApiResultStatus.NotFound,
                Success = false,
                Message = "此為不存在的單字，請重新輸入"
            };
        }

        bool isCorrect;

        switch (quizAttempt.QuizType)
        {
            case VocabularyQuizType.ChineseToJapanese:
                isCorrect = dto.Answer == vocabulary.JapanenseName;
                break;

            case VocabularyQuizType.JapaneseToChinese:
                isCorrect = dto.Answer == vocabulary.ChineseName;
                break;

            case VocabularyQuizType.HiraganaToKana:
                isCorrect = dto.Answer == vocabulary.KanaName;
                break;

            default:
                return new ServiceResult<bool>
                {
                    Success = false,
                    Message = "不存在的測驗類型",
                    Data = false
                };
        }

        //  建立這一題的答題紀錄
        var quizAnswer = new QuizAnswer
        {
            QuizAttemptId = quizAttempt.Id,
            VocabularyId = vocabulary.Id,
            UserAnswer = dto.Answer,
            IsCorrect = isCorrect
        };

        _dbContext.QuizAnswer.Add(quizAnswer);

        // 答對的話更新總答對數
        if (isCorrect)
        {
            quizAttempt.CorrectCount++;
        }

        if (answeredCount + 1 >= quizAttempt.TotalQuestions)
        {
            quizAttempt.CompletedAt = DateTime.UtcNow;
        }


        //  一次存進 DB
        await _dbContext.SaveChangesAsync();

        return new ServiceResult<bool>
        {
            Success = true,
            Message = isCorrect ? "測驗結果答對" : "測驗結果答錯",
            Data = isCorrect
        };

    }

}