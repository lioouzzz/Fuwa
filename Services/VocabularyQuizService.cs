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


    public async Task<ServiceResult<VocabularyQuizDto>> GenerateVocabularyQuestion(int quizAttemptId)
    {
        //找這次測驗
        var quizAttempt = await _dbContext.QuizAttempt
                                         .FirstOrDefaultAsync(q => q.Id == quizAttemptId);

        if (quizAttempt == null)
        {
            _logger.LogWarning("找不到此測驗對應的課程Id quizAttempt: {quizAttemptId}", quizAttemptId);

            return new ServiceResult<VocabularyQuizDto>
            {
                apiResultStatus = ApiResultStatus.NotFound,
                Success = false,
                Message = "找不到此測驗quizAttempt"
            };
        }


        //先判斷此測驗是否已完成
        if (quizAttempt.CompletedAt != null)
        {
            _logger.LogInformation("此測驗已完成");

            return new ServiceResult<VocabularyQuizDto>
            {
                Success = false,
                Message = "此測驗已完成"
            };
        }

        //找這場測驗已經回答過哪些 Vocabulary
        var answeredVocabulary = await _dbContext.QuizAnswer
                                         .Where(a => a.QuizAttemptId == quizAttemptId)
                                         .Select(a => a.VocabularyId)
                                         .ToListAsync();


        //已經回答完指定題數，就不能再出題
        if (answeredVocabulary.Count == quizAttempt.TotalQuestions)
        {
            return new ServiceResult<VocabularyQuizDto>
            {
                Success = false,
                Message = "此測驗已達作答題數上限"
            };
        }


        //找對應課程全部的單字
        var vocabularies = await _dbContext.Vocabularies
                .Where(v => v.LessonId == quizAttempt.LessonId)
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


        //正確答案候選
        //排除這場測驗已經回答過的 Vocabulary
        var availableVocabularies = vocabularies.Where(v => !answeredVocabulary.Contains(v.Id)).ToList();


        if (availableVocabularies.Count == 0)
        {
            _logger.LogWarning("此課程單字數量為0,無法出題");

            return new ServiceResult<VocabularyQuizDto>
            {
                Success = false,
                Message = "此課程已沒有可出的單字"
            };
        }



        // 從尚未回答過的單字中選正確答案
        // Random.Shared.Next (取得從 0 到很大的正整數)
        var randomIndex = Random.Shared.Next(availableVocabularies.Count);


        var correctVocabulary = vocabularies[randomIndex];


        //排除答案，再選另外三個
        var wrongAnsers = availableVocabularies
                            .Where(v => v.Id != correctVocabulary.Id)
                            .OrderBy(v => Guid.NewGuid())
                            .Take(3)
                            .ToList();


        List<QuizOptionDto> options;


        switch (quizAttempt.QuizType)
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

        switch (quizAttempt.QuizType)
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

        options = options.OrderBy(x => Guid.NewGuid()).ToList();

        //回傳題目
        var dto = new VocabularyQuizDto
        {
            VocabularyId = correctVocabulary.Id,
            Question = questions,
            Type = quizAttempt.QuizType,
            Options = options
        };

        return new ServiceResult<VocabularyQuizDto>
        {
            Success = true,
            Message = "成功取得測驗題目和答案",
            Data = dto
        };
    }

    public async Task<ServiceResult<QuizResultDto>> GetVocabularyQuizResult(int quizAttemptId)
    {
        var quizAttempt = await _dbContext.QuizAttempt
                                .Include(q => q.Lesson)
                                .FirstOrDefaultAsync(q => q.Id == quizAttemptId);

        if (quizAttempt == null)
        {
            _logger.LogWarning("找不到此測驗對應的課程Id quizAttempt: {quizAttemptId}", quizAttemptId);

            return new ServiceResult<QuizResultDto>
            {
                apiResultStatus = ApiResultStatus.NotFound,
                Success = false,
                Message = "找不到此測驗quizAttempt"
            };
        }

        var wrongAnswer = quizAttempt.TotalQuestions - quizAttempt.CorrectCount;

        double accuracy = 0;
        if (quizAttempt.TotalQuestions > 0)
        {
            accuracy = ((double)quizAttempt.CorrectCount / quizAttempt.TotalQuestions) * 100;

        }

        var duration = quizAttempt.CompletedAt.Value
                     - quizAttempt.StartedAt;

        int durationSeconds = (int)duration.TotalSeconds;

        var dto = new QuizResultDto
        {
            QuizAttemptId = quizAttempt.Id,
            LessonNumber = quizAttempt.Lesson.LessonNumber,
            QuizType = quizAttempt.QuizType,
            TotalQuestions = quizAttempt.TotalQuestions,
            CorrectCount = quizAttempt.CorrectCount,
            WrongCount = wrongAnswer,
            Accuracy = accuracy,
            DurationSeconds = durationSeconds
        };

        return new ServiceResult<QuizResultDto>
        {
            Success = true,
            Message = "成功取得測驗結果",
            Data = dto
        };
    }


    //測驗歷史紀錄
    public async Task<ServiceResult<List<QuizVocabularyHistoryDto>>> GetQuizVocabularyHistory()

    {
        var history = await _dbContext.QuizAttempt
                                      .AsNoTracking()
                                      .Where(q => q.CompletedAt != null)
                                      .OrderBy(q => q.CompletedAt)
                                      .Select(q => new QuizVocabularyHistoryDto
                                      {
                                          QuizAttemptId = q.Id,
                                          LessonId = q.LessonId,
                                          LessonNumber = q.Lesson.LessonNumber,
                                          QuizType = q.QuizType,
                                          TotalQuestions = q.TotalQuestions,
                                          CorrectCount = q.CorrectCount,
                                          WrongCount = q.TotalQuestions - q.CorrectCount,
                                          Accuracy = q.TotalQuestions > 0 ? (double)q.CorrectCount / q.TotalQuestions * 100 : 0,
                                          StartedAt = q.StartedAt,
                                          CompletedAt = q.CompletedAt,
                                          DurationSeconds = (int)(q.CompletedAt.Value - q.StartedAt).TotalSeconds
                                      }).ToListAsync();

        if (history == null)
        {
            return new ServiceResult<List<QuizVocabularyHistoryDto>>
            {
                apiResultStatus = ApiResultStatus.NotFound,
                Success = false,
                Message = "查詢歷史測驗紀錄失敗"
            };
        }


        return new ServiceResult<List<QuizVocabularyHistoryDto>>
        {
            Success = true,
            Message = "查詢歷史測驗紀錄成功",
            Data = history
        };

    }

    //單次測驗錯題本
    public async Task<ServiceResult<QuizAttemptWrongAnswerDto>> GetQuizAttemptWrongAnswer(int quizAttemptId)
    {
        var quizAttemptExists = await _dbContext.QuizAttempt.AnyAsync(q => q.Id == quizAttemptId);

        if (!quizAttemptExists)
        {
            _logger.LogWarning("找不到此測驗對應的課程Id quizAttempt: {quizAttemptId}", quizAttemptId);

            return new ServiceResult<QuizAttemptWrongAnswerDto>
            {
                apiResultStatus = ApiResultStatus.NotFound,
                Success = false,
                Message = "找不到此測驗QuizAttemptId"
            };
        }

        var wrongAnswer = await _dbContext.QuizAnswer
            .Where(answer => answer.QuizAttemptId == quizAttemptId
                && answer.IsCorrect == false
                && answer.QuizAttempt.CompletedAt != null)
            .Select(answer => new QuizAttemptWrongAnswerDto
            {
                VocabularyId = answer.VocabularyId,

                Question =
                    answer.QuizAttempt.QuizType == VocabularyQuizType.ChineseToJapanese ? answer.Vocabulary.ChineseName
                    : answer.QuizAttempt.QuizType == VocabularyQuizType.HiraganaToKana ? answer.Vocabulary.KanaName
                    : answer.QuizAttempt.QuizType == VocabularyQuizType.JapaneseToChinese ? answer.Vocabulary.JapanenseName
                    : string.Empty,
                UserAnswer = answer.UserAnswer,

                CorrectAnswer =
                    answer.QuizAttempt.QuizType == VocabularyQuizType.ChineseToJapanese ? answer.Vocabulary.JapanenseName
                    : answer.QuizAttempt.QuizType == VocabularyQuizType.HiraganaToKana ? answer.Vocabulary.KanaName
                    : answer.QuizAttempt.QuizType == VocabularyQuizType.JapaneseToChinese ? answer.Vocabulary.ChineseName
                    : string.Empty
            })
            .FirstOrDefaultAsync();

        if (wrongAnswer == null)
        {
            return new ServiceResult<QuizAttemptWrongAnswerDto>
            {
                Success = false,
                Message = "此測驗沒有錯題"
            };
        }

        return new ServiceResult<QuizAttemptWrongAnswerDto>
        {
            Success = true,
            Message = "單次測驗錯題查詢成功",
            Data = wrongAnswer
        };
    }
    //建立全部的錯題本
    public async Task<ServiceResult<List<QuizWrongAnswerBookDto>>> GetWrongAnswerBookAsync(VocabularyQuizType quizType)
    {

        if (!Enum.IsDefined(typeof(VocabularyQuizType), quizType))
        {
            return new ServiceResult<List<QuizWrongAnswerBookDto>>
            {
                apiResultStatus = ApiResultStatus.NotFound,
                Success = false,
                Message = "不存在的測驗類型"
            };
        }
        var wrongAnswerCount = await _dbContext.QuizAnswer
                        .Where(answer => answer.IsCorrect == false &&
                               answer.QuizAttempt.QuizType == quizType &&
                               answer.QuizAttempt.CompletedAt != null)

                        .GroupBy(answer => new
                        {
                            answer.QuizAttempt.Lesson.LessonNumber,
                            answer.QuizAttempt.QuizType,
                            answer.VocabularyId,
                            answer.Vocabulary.JapanenseName,
                            answer.Vocabulary.KanaName,
                            answer.Vocabulary.ChineseName

                        })

                        .Select(group => new QuizWrongAnswerBookDto
                        {
                            LessonNumber = group.Key.LessonNumber,
                            Type = group.Key.QuizType,
                            VocabularyId = group.Key.VocabularyId,

                            Question =
                                quizType == VocabularyQuizType.JapaneseToChinese ? group.Key.JapanenseName
                                 : quizType == VocabularyQuizType.HiraganaToKana ? group.Key.JapanenseName
                                : quizType == VocabularyQuizType.ChineseToJapanese ? group.Key.ChineseName
                                : string.Empty,

                            CorrectAnswer =
                            quizType == VocabularyQuizType.ChineseToJapanese ? group.Key.JapanenseName
                            : quizType == VocabularyQuizType.HiraganaToKana ? group.Key.KanaName
                            : quizType == VocabularyQuizType.JapaneseToChinese ? group.Key.ChineseName
                            : string.Empty,

                            WrongVocabularyCount = group.Count(),



                        }).ToListAsync();


        return new ServiceResult<List<QuizWrongAnswerBookDto>>
        {
            Success = true,
            Message = "成功取得全部錯題",
            Data = wrongAnswerCount
        };
    }
    public async Task<ServiceResult<bool>> SubmitVocabularyAnswer(SubmitVocabularyAnswerDto dto)
    {

        //找這次測驗
        var quizAttempt = await _dbContext.QuizAttempt.FirstOrDefaultAsync(q => q.Id == dto.QuizAttemptId);

        if (quizAttempt == null)
        {
            _logger.LogWarning("此quizAttempt不存在 ,  quizAttemptId: {quizAttemptId}", dto.QuizAttemptId);
            return new ServiceResult<bool>
            {
                Success = false,
                Message = "找不到此測驗"
            };
        }

        // 先判斷此測驗是否已完成
        if (quizAttempt.CompletedAt != null)
        {
            _logger.LogInformation("此測驗已完成");

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