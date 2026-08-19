using Dtos.Lessons;
using Helper;
namespace Services.Interfaces;

public interface ILessonService
{
    Task<List<LessonDto>> GetAllLessonsAsync();
    Task<LessonDto> GetLessonByIdAsync(int id);

    Task<LessonDetailDto> GetLessonDetail(int id);
    Task<ServiceResult<LessonDto>> CreateLessonAsync(CreateLessonDto dto);
    Task<ServiceResult<bool>> UpdateLessonAsync(int id, UpdateLessonDto dto);
    Task<bool> DeleteLessonAsync(int id);

}