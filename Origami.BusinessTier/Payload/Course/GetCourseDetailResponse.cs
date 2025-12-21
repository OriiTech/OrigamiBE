using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Course
{
    public class DetailInstructorInfo
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Image { get; set; }
        public string? Bio { get; set; }
        public decimal? Rating { get; set; }
        public int Reviews { get; set; }
        public int Courses { get; set; }
    }

    public class CourseRatingDetail
    {
        public double Average { get; set; }
        public int Count { get; set; }
        public Dictionary<string, int> Distribution { get; set; } = new Dictionary<string, int>();
    }

    public class CourseContentLesson
    {
        public string Id { get; set; } = null!;
        public string? Title { get; set; }
        public int TotalLectures { get; set; } = 0;
        public int TotalMinutes { get; set; } = 0;
    }

    public class GetCourseDetailResponse
    {
        public string Id { get; set; } = null!;
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public List<DetailInstructorInfo> Instructors { get; set; } = new List<DetailInstructorInfo>();
        public string? Description { get; set; }
        public List<string> Objectives { get; set; } = new List<string>(); // Postponed
        public List<CourseTargetLevelInfo> TargetLevel { get; set; } = new List<CourseTargetLevelInfo>();
        public List<object> Category { get; set; } = new List<object>(); // Postponed, returning empty
        public string? Language { get; set; }
        public DateTime? PublishedDate { get; set; }
        public CourseRatingDetail Rating { get; set; } = new CourseRatingDetail();
        public decimal? Price { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int TotalLessons { get; set; }
        public List<CourseContentLesson> Lessons { get; set; } = new List<CourseContentLesson>();
    }
}
