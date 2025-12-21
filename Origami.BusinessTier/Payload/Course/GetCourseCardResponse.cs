using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Origami.BusinessTier.Payload.Course
{
    public class CourseInstructorInfo
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Image { get; set; }
    }

    public class CourseTargetLevelInfo
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
    }

    public class GetCourseCardResponse
    {
        public string Id { get; set; } = null!;
        public string? Title { get; set; }
        public List<CourseInstructorInfo> Instructors { get; set; } = new List<CourseInstructorInfo>();
        public List<CourseTargetLevelInfo> TargetLevel { get; set; } = new List<CourseTargetLevelInfo>();
        public string? Language { get; set; }
        public DateTime? PublishedDate { get; set; }
        public int TotalStudents { get; set; }
        public double Rating { get; set; }
        public decimal? Price { get; set; }
        public string? ThumbnailUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? Bestseller { get; set; }
    }
}
