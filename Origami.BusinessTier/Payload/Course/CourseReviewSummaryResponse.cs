using System.Collections.Generic;

namespace Origami.BusinessTier.Payload.Course
{
    public class CourseReviewSummaryResponse
    {
        public int Total { get; set; }
        public List<CourseReviewItemResponse> Recent { get; set; } = new();
    }

    public class CourseReviewItemResponse
    {
        public int Id { get; set; }
        public string? StudentName { get; set; }
        public int? Rating { get; set; }
        public string? Date { get; set; }
        public string? Comment { get; set; }
        public int? LikeCount { get; set; } // NOTE: DB chưa có cột like_count cho Course_review
        public CourseReviewResponseItem? Response { get; set; }
    }

    public class CourseReviewResponseItem
    {
        public string? SenseiName { get; set; }
        public string? Date { get; set; }
        public string? Comment { get; set; }
    }
}

