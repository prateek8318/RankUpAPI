using AutoMapper;
using RankUpAPI.Areas.Admin.Question.DTOs;
using RankUpAPI.Areas.Admin.Question.Services.Interfaces;
using RankUpAPI.Models;
using RankUpAPI.Repositories.Interfaces;
using System.Globalization;
using System.Text;
using QuestionModel = RankUpAPI.Models.Question;
using SubjectModel = RankUpAPI.Models.Subject;
using ChapterModel = RankUpAPI.Models.Chapter;

namespace RankUpAPI.Areas.Admin.Question.Services.Implementations
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly ITestSeriesQuestionRepository _testSeriesQuestionRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IChapterRepository _chapterRepository;
        private readonly IExamRepository _examRepository;
        private readonly IMapper _mapper;

        public QuestionService(IQuestionRepository questionRepository, ITestSeriesQuestionRepository testSeriesQuestionRepository, ISubjectRepository subjectRepository, IChapterRepository chapterRepository, IExamRepository examRepository, IMapper mapper)
        {
            _questionRepository = questionRepository;
            _testSeriesQuestionRepository = testSeriesQuestionRepository;
            _subjectRepository = subjectRepository;
            _chapterRepository = chapterRepository;
            _examRepository = examRepository;
            _mapper = mapper;
        }

        public async Task<QuestionDto> CreateQuestionAsync(CreateQuestionDto createDto)
        {
            var question = _mapper.Map<QuestionModel>(createDto);
            question.CreatedAt = DateTime.UtcNow;
            question.IsActive = true;

            await _questionRepository.AddAsync(question);
            await _questionRepository.SaveChangesAsync();

            return await GetQuestionByIdAsync(question.Id) ?? throw new Exception("Failed to retrieve created question");
        }

        public async Task<bool> DeleteQuestionAsync(int id)
        {
            var question = await _questionRepository.GetByIdWithDetailsAsync(id);
            
            if (question == null)
                return false;

            var testSeriesQuestions = await _testSeriesQuestionRepository.GetByQuestionIdAsync(id);
            await _testSeriesQuestionRepository.DeleteRangeAsync(testSeriesQuestions);
            await _questionRepository.DeleteAsync(question);
            await _questionRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<QuestionDto>> GetAllQuestionsAsync()
        {
            var questions = await _questionRepository.GetActiveAsync();
            return questions.Select(q => MapToDto(q));
        }

        public async Task<QuestionDto?> GetQuestionByIdAsync(int id)
        {
            var question = await _questionRepository.GetByIdWithDetailsAsync(id);
            if (question == null)
                return null;
            return MapToDto(question);
        }

        public async Task<IEnumerable<QuestionDto>> GetQuestionsByChapterIdAsync(int chapterId)
        {
            var questions = await _questionRepository.GetByChapterIdAsync(chapterId);
            return questions.Select(q => MapToDto(q));
        }

        public async Task<IEnumerable<QuestionDto>> GetQuestionsBySubjectIdAsync(int subjectId)
        {
            var questions = await _questionRepository.GetBySubjectIdAsync(subjectId);
            return questions.Select(q => MapToDto(q));
        }

        public async Task<IEnumerable<QuestionDto>> GetQuestionsByExamIdAsync(int examId)
        {
            var questions = await _questionRepository.GetByExamIdAsync(examId);
            return questions.Select(q => MapToDto(q));
        }

        public async Task<bool> ToggleQuestionStatusAsync(int id, bool isActive)
        {
            var question = await _questionRepository.GetByIdAsync(id);
            if (question == null)
                return false;

            question.IsActive = isActive;
            question.UpdatedAt = DateTime.UtcNow;
            await _questionRepository.UpdateAsync(question);
            await _questionRepository.SaveChangesAsync();
            return true;
        }

        public async Task<QuestionDto?> UpdateQuestionAsync(int id, UpdateQuestionDto updateDto)
        {
            var question = await _questionRepository.GetByIdAsync(id);
            if (question == null)
                return null;

            _mapper.Map(updateDto, question);
            question.UpdatedAt = DateTime.UtcNow;

            await _questionRepository.UpdateAsync(question);
            await _questionRepository.SaveChangesAsync();
            return await GetQuestionByIdAsync(id);
        }

        public async Task<BulkUploadQuestionDto> BulkUploadQuestionsAsync(IFormFile file)
        {
            var result = new BulkUploadQuestionDto();
            var errors = new List<string>();

            if (file == null || file.Length == 0)
            {
                result.Errors.Add("No file uploaded");
                return result;
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".csv")
            {
                result.Errors.Add("Only CSV files are supported");
                return result;
            }

            try
            {
                using var stream = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
                var lines = new List<string>();
                string? line;
                while ((line = await stream.ReadLineAsync()) != null)
                {
                    lines.Add(line);
                }

                if (lines.Count < 2)
                {
                    result.Errors.Add("CSV file must have at least a header row and one data row");
                    return result;
                }

                var header = ParseCsvLine(lines[0]);
                var headerMap = new Dictionary<string, int>();
                for (int i = 0; i < header.Count; i++)
                {
                    headerMap[header[i].Trim().ToLowerInvariant()] = i;
                }

                var defaultExam = (await _examRepository.GetActiveAsync()).FirstOrDefault();
                if (defaultExam == null)
                {
                    result.Errors.Add("No exam found in database. Please create an exam first.");
                    return result;
                }

                var subjectChapterMap = new Dictionary<string, int>();

                for (int rowIndex = 1; rowIndex < lines.Count; rowIndex++)
                {
                    try
                    {
                        var row = ParseCsvLine(lines[rowIndex]);
                        if (row.Count == 0) continue;

                        string GetValueFromRow(string key, string defaultValue = "")
                        {
                            if (headerMap.TryGetValue(key.ToLowerInvariant(), out int index) && index < row.Count)
                            {
                                var value = row[index].Trim();
                                if (value.Equals("Null", StringComparison.OrdinalIgnoreCase) || 
                                    value.Equals("NULL", StringComparison.OrdinalIgnoreCase))
                                    return "";
                                return value;
                            }
                            var variations = new[] { key, key.Replace(" ", ""), key.Replace(" ", "_") };
                            foreach (var variation in variations)
                            {
                                if (headerMap.TryGetValue(variation.ToLowerInvariant(), out int idx) && idx < row.Count)
                                {
                                    var value = row[idx].Trim();
                                    if (value.Equals("Null", StringComparison.OrdinalIgnoreCase) || 
                                        value.Equals("NULL", StringComparison.OrdinalIgnoreCase))
                                        return "";
                                    return value;
                                }
                            }
                            return defaultValue;
                        }

                        var subjectName = GetValueFromRow("subject name");
                        if (string.IsNullOrWhiteSpace(subjectName))
                        {
                            errors.Add($"Row {rowIndex + 1}: Subject Name is required");
                            result.ErrorCount++;
                            continue;
                        }

                        int chapterId;
                        if (!subjectChapterMap.TryGetValue(subjectName, out chapterId))
                        {
                            var subjects = await _subjectRepository.GetByExamIdAsync(defaultExam.Id);
                            var subject = subjects.FirstOrDefault(s => s.Name.ToLower() == subjectName.ToLower());
                            
                            if (subject == null)
                            {
                                subject = new SubjectModel
                                {
                                    Name = subjectName,
                                    ExamId = defaultExam.Id,
                                    CreatedAt = DateTime.UtcNow,
                                    IsActive = true
                                };
                                await _subjectRepository.AddAsync(subject);
                                await _subjectRepository.SaveChangesAsync();
                            }

                            var chapters = await _chapterRepository.GetBySubjectIdAsync(subject.Id);
                            var chapter = chapters.FirstOrDefault(c => c.Name == "Default");
                            
                            if (chapter == null)
                            {
                                chapter = new ChapterModel
                                {
                                    Name = "Default",
                                    SubjectId = subject.Id,
                                    CreatedAt = DateTime.UtcNow,
                                    IsActive = true
                                };
                                await _chapterRepository.AddAsync(chapter);
                                await _chapterRepository.SaveChangesAsync();
                            }

                            chapterId = chapter.Id;
                            subjectChapterMap[subjectName] = chapterId;
                        }

                        var questionDto = ParseQuestionFromCsvRowWithChapter(row, headerMap, chapterId);

                        if (questionDto != null)
                        {
                            var validationErrors = ValidateQuestion(questionDto);
                            if (validationErrors.Any())
                            {
                                errors.Add($"Row {rowIndex + 1}: {string.Join(", ", validationErrors)}");
                                result.ErrorCount++;
                                continue;
                            }

                            var question = _mapper.Map<QuestionModel>(questionDto);
                            question.CreatedAt = DateTime.UtcNow;
                            question.IsActive = true;

                            await _questionRepository.AddAsync(question);
                            result.SuccessCount++;
                        }
                        else
                        {
                            errors.Add($"Row {rowIndex + 1}: Failed to parse question data");
                            result.ErrorCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Row {rowIndex + 1}: {ex.Message}");
                        result.ErrorCount++;
                    }
                }

                await _questionRepository.SaveChangesAsync();
                result.Errors = errors;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Error processing file: {ex.Message}");
            }

            return result;
        }

        private QuestionDto MapToDto(QuestionModel question)
        {
            return new QuestionDto
            {
                Id = question.Id,
                QuestionTextEnglish = question.QuestionTextEnglish,
                QuestionTextHindi = question.QuestionTextHindi,
                Type = question.Type,
                QuestionImageUrlEnglish = question.QuestionImageUrlEnglish,
                QuestionImageUrlHindi = question.QuestionImageUrlHindi,
                QuestionVideoUrlEnglish = question.QuestionVideoUrlEnglish,
                QuestionVideoUrlHindi = question.QuestionVideoUrlHindi,
                OptionAEnglish = question.OptionAEnglish,
                OptionBEnglish = question.OptionBEnglish,
                OptionCEnglish = question.OptionCEnglish,
                OptionDEnglish = question.OptionDEnglish,
                OptionAHindi = question.OptionAHindi,
                OptionBHindi = question.OptionBHindi,
                OptionCHindi = question.OptionCHindi,
                OptionDHindi = question.OptionDHindi,
                OptionImageAUrl = question.OptionImageAUrl,
                OptionImageBUrl = question.OptionImageBUrl,
                OptionImageCUrl = question.OptionImageCUrl,
                OptionImageDUrl = question.OptionImageDUrl,
                CorrectAnswer = question.CorrectAnswer,
                ExplanationEnglish = question.ExplanationEnglish,
                ExplanationHindi = question.ExplanationHindi,
                SolutionImageUrlEnglish = question.SolutionImageUrlEnglish,
                SolutionImageUrlHindi = question.SolutionImageUrlHindi,
                SolutionVideoUrlEnglish = question.SolutionVideoUrlEnglish,
                SolutionVideoUrlHindi = question.SolutionVideoUrlHindi,
                Difficulty = question.Difficulty,
                ChapterId = question.ChapterId,
                ChapterName = question.Chapter.Name,
                SubjectId = question.Chapter.SubjectId,
                SubjectName = question.Chapter.Subject.Name,
                ExamId = question.Chapter.Subject.ExamId,
                ExamName = question.Chapter.Subject.Exam.Name,
                Marks = question.Marks,
                NegativeMarks = question.NegativeMarks,
                EstimatedTimeInSeconds = question.EstimatedTimeInSeconds,
                IsMcq = question.IsMcq,
                IsActive = question.IsActive,
                CreatedAt = question.CreatedAt,
                UpdatedAt = question.UpdatedAt
            };
        }

        private List<string> ParseCsvLine(string line)
        {
            var result = new List<string>();
            var currentField = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        currentField.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(currentField.ToString());
                    currentField.Clear();
                }
                else
                {
                    currentField.Append(c);
                }
            }

            result.Add(currentField.ToString());
            return result;
        }

        private List<string> ValidateQuestion(CreateQuestionDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.QuestionTextEnglish) && string.IsNullOrWhiteSpace(dto.QuestionTextHindi))
                errors.Add("Question text (English or Hindi) is required");

            if (string.IsNullOrWhiteSpace(dto.OptionAEnglish))
                errors.Add("Option A (English) is required");

            if (string.IsNullOrWhiteSpace(dto.OptionBEnglish))
                errors.Add("Option B (English) is required");

            if (string.IsNullOrWhiteSpace(dto.OptionCEnglish))
                errors.Add("Option C (English) is required");

            if (string.IsNullOrWhiteSpace(dto.OptionDEnglish))
                errors.Add("Option D (English) is required");

            if (string.IsNullOrWhiteSpace(dto.CorrectAnswer) || !new[] { "A", "B", "C", "D" }.Contains(dto.CorrectAnswer.ToUpperInvariant()))
                errors.Add("Correct answer must be A, B, C, or D");

            if (dto.ChapterId <= 0)
                errors.Add("Valid Chapter ID is required");

            return errors;
        }

        private CreateQuestionDto? ParseQuestionFromCsvRowWithChapter(List<string> row, Dictionary<string, int> headerMap, int chapterId)
        {
            try
            {
                var dto = new CreateQuestionDto();
                dto.ChapterId = chapterId;

                string GetValue(string key, string defaultValue = "")
                {
                    if (headerMap.TryGetValue(key.ToLowerInvariant(), out int index) && index < row.Count)
                    {
                        var value = row[index].Trim();
                        if (value.Equals("Null", StringComparison.OrdinalIgnoreCase) || 
                            value.Equals("NULL", StringComparison.OrdinalIgnoreCase))
                            return "";
                        return value;
                    }
                    var variations = new[] { key, key.Replace(" ", ""), key.Replace(" ", "_") };
                    foreach (var variation in variations)
                    {
                        if (headerMap.TryGetValue(variation.ToLowerInvariant(), out int idx) && idx < row.Count)
                        {
                            var value = row[idx].Trim();
                            if (value.Equals("Null", StringComparison.OrdinalIgnoreCase) || 
                                value.Equals("NULL", StringComparison.OrdinalIgnoreCase))
                                return "";
                            return value;
                        }
                    }
                    return defaultValue;
                }

                dto.QuestionTextEnglish = GetValue("question name english") ?? 
                                         GetValue("questiontextenglish") ?? 
                                         GetValue("questiontext") ?? 
                                         GetValue("question") ?? "";
                dto.QuestionTextHindi = GetValue("questiontexthindi") ?? 
                                       GetValue("question name hindi") ?? "";

                var questionImage = GetValue("question image");
                var typeStr = GetValue("type", "text").ToLowerInvariant();
                if (!string.IsNullOrWhiteSpace(questionImage))
                    dto.Type = QuestionType.Image;
                else if (typeStr == "video")
                    dto.Type = QuestionType.Video;
                else
                    dto.Type = QuestionType.Text;

                dto.OptionAEnglish = GetValue("option 1") ?? 
                                     GetValue("optionaenglish") ?? 
                                     GetValue("optiona") ?? "";
                dto.OptionBEnglish = GetValue("option 2") ?? 
                                     GetValue("optionbenglish") ?? 
                                     GetValue("optionb") ?? "";
                dto.OptionCEnglish = GetValue("option 3") ?? 
                                     GetValue("optioncenglish") ?? 
                                     GetValue("optionc") ?? "";
                dto.OptionDEnglish = GetValue("option 4") ?? 
                                     GetValue("optiondenglish") ?? 
                                     GetValue("optiond") ?? "";

                dto.OptionAHindi = GetValue("optionahindi") ?? "";
                dto.OptionBHindi = GetValue("optionbhindi") ?? "";
                dto.OptionCHindi = GetValue("optionchindi") ?? "";
                dto.OptionDHindi = GetValue("optiondhindi") ?? "";

                var correctOption = GetValue("correct option") ?? GetValue("correctanswer") ?? "";
                if (!string.IsNullOrWhiteSpace(correctOption))
                {
                    if (int.TryParse(correctOption.Trim(), out int correctIndex))
                    {
                        dto.CorrectAnswer = correctIndex switch
                        {
                            0 => "A",
                            1 => "B",
                            2 => "C",
                            3 => "D",
                            _ => "A"
                        };
                    }
                    else
                    {
                        var upper = correctOption.Trim().ToUpperInvariant();
                        if (upper.Length > 0 && new[] { "A", "B", "C", "D" }.Contains(upper[0].ToString()))
                            dto.CorrectAnswer = upper[0].ToString();
                        else
                            dto.CorrectAnswer = "A";
                    }
                }
                else
                {
                    dto.CorrectAnswer = "A";
                }

                dto.ExplanationEnglish = GetValue("solution detail english") ?? 
                                         GetValue("explanationenglish") ?? 
                                         GetValue("explanation") ?? 
                                         GetValue("solution detail") ?? "";
                dto.ExplanationHindi = GetValue("explanationhindi") ?? 
                                      GetValue("solution detail hindi") ?? "";

                var difficultyStr = GetValue("difficulty", "easy").ToLowerInvariant();
                dto.Difficulty = difficultyStr switch
                {
                    "medium" => DifficultyLevel.Medium,
                    "hard" => DifficultyLevel.Hard,
                    _ => DifficultyLevel.Easy
                };

                dto.QuestionImageUrlEnglish = GetValue("question image") ?? 
                                            GetValue("questionimageurlenglish") ?? "";
                dto.QuestionImageUrlHindi = GetValue("questionimageurlhindi") ?? "";
                dto.QuestionVideoUrlEnglish = GetValue("questionvideourlenglish") ?? "";
                dto.QuestionVideoUrlHindi = GetValue("questionvideourlhindi") ?? "";

                dto.OptionImageAUrl = GetValue("option 1 image") ?? 
                                    GetValue("optionimageaurl") ?? "";
                dto.OptionImageBUrl = GetValue("option 2 image") ?? 
                                    GetValue("optionimageburl") ?? "";
                dto.OptionImageCUrl = GetValue("option 3 image") ?? 
                                    GetValue("optionimagecurl") ?? "";
                dto.OptionImageDUrl = GetValue("option 4 image") ?? 
                                    GetValue("optionimagedurl") ?? "";

                dto.SolutionImageUrlEnglish = GetValue("solution image") ?? 
                                             GetValue("solutionimageurlenglish") ?? "";
                dto.SolutionImageUrlHindi = GetValue("solutionimageurlhindi") ?? "";
                dto.SolutionVideoUrlEnglish = GetValue("solutionvideourlenglish") ?? "";
                dto.SolutionVideoUrlHindi = GetValue("solutionvideourlhindi") ?? "";

                if (int.TryParse(GetValue("marks"), out int marks))
                    dto.Marks = marks;
                else
                    dto.Marks = 1;

                if (decimal.TryParse(GetValue("negativemarks"), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal negativeMarks))
                    dto.NegativeMarks = negativeMarks;

                if (int.TryParse(GetValue("estimatedtimeinseconds"), out int time))
                    dto.EstimatedTimeInSeconds = time;
                else
                    dto.EstimatedTimeInSeconds = 120;

                dto.IsMcq = GetValue("ismcq", "true").ToLowerInvariant() != "false";

                return dto;
            }
            catch
            {
                return null;
            }
        }
    }
}
