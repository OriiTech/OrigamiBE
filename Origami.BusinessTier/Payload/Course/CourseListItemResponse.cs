using System;
using System.Collections.Generic;

namespace Origami.BusinessTier.Payload.Course
{
    public class CourseListItemResponse
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public List<SimpleUserDto> Instructors { get; set; } = new();
        public List<IdNameDto> TargetLevel { get; set; } = new();
        public string? Language { get; set; }
        public DateTime? PublishedDate { get; set; }
        public int TotalStudents { get; set; }
        public double? Rating { get; set; }
        public decimal? Price { get; set; }
        public bool? PaidOnly { get; set; } // NOTE: DB chưa có cột paid_only cho Course
        public string? ThumbnailUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? Bestseller { get; set; }
        public bool? Trending { get; set; } // NOTE: DB chưa có cột trending cho Course
    }
}

