using System.Collections.Generic;

namespace Origami.BusinessTier.Payload.Course
{
    public class CourseQuestionSummaryResponse
    {
        public int TotalQuestions { get; set; }
        public List<CourseQuestionItemResponse> RecentQuestions { get; set; } = new();
    }

    public class CourseQuestionItemResponse
    {
        public int Id { get; set; }
        public string? Question { get; set; }
        public string? StudentName { get; set; }
        public string? Date { get; set; }
        public List<CourseAnswerItemResponse> Answers { get; set; } = new();
    }

    public class CourseAnswerItemResponse
    {
        public string? ResponseName { get; set; }
        public string? Date { get; set; }
        public string? Comment { get; set; }
    }
}

