using System;
using System.Collections.Generic;

namespace Origami.BusinessTier.Payload.Course
{
    public class CourseDetailResponse
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Subtitle { get; set; } // NOTE: DB chưa có cột subtitle cho Course
        public List<CourseInstructorDto> Instructors { get; set; } = new();
        public string? Description { get; set; }
        public List<string> Objectives { get; set; } = new(); // NOTE: DB chưa có cột objectives
        public List<IdNameDto> TargetLevel { get; set; } = new();
        public List<IdNameDto> Category { get; set; } = new();
        public string? Language { get; set; }
        public DateTime? PublishedDate { get; set; }
        public CourseLengthDto CourseLength { get; set; } = new();
        public EnrollmentDto Enrollment { get; set; } = new();
        public CourseRatingDto Rating { get; set; } = new();
        public CoursePriceDto Price { get; set; } = new();
        public PreviewDto Preview { get; set; } = new();
        public CourseContentDto Content { get; set; } = new();
        public MetadataDto Metadata { get; set; } = new();
    }

    public class CourseInstructorDto : SimpleUserDto
    {
        public double? Rating { get; set; }
        public int? Reviews { get; set; }
        public int? Courses { get; set; }
    }

    public class CourseLengthDto
    {
        public double? TotalVideoHours { get; set; }
        public int TotalLectures { get; set; }
        public int TotalResources { get; set; }
    }

    public class EnrollmentDto
    {
        public int TotalStudents { get; set; }
        public int? RecentStudents { get; set; } // NOTE: chưa có trường riêng, sẽ để null
    }

    public class CourseRatingDto
    {
        public double? Average { get; set; }
        public int Count { get; set; }
        public CourseRatingDistribution Distribution { get; set; } = new();
    }

    public class CourseRatingDistribution
    {
        public int FiveStars { get; set; }
        public int FourStars { get; set; }
        public int ThreeStars { get; set; }
        public int TwoStars { get; set; }
        public int OneStar { get; set; }
    }

    public class CoursePriceDto
    {
        public decimal? Amount { get; set; }
        public bool? PaidOnly { get; set; } // NOTE: DB chưa có cột paid_only
    }

    public class PreviewDto
    {
        public string? VideoUrl { get; set; } // NOTE: DB chưa có cột preview_video_url cho Course
        public string? ThumbnailUrl { get; set; }
    }

    public class CourseContentDto
    {
        public List<CourseLessonSummaryDto> Lessons { get; set; } = new();
        public int TotalLessons { get; set; }
        public int TotalLectures { get; set; }
        public double? TotalHours { get; set; }
    }

    public class CourseLessonSummaryDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int TotalLectures { get; set; }
        public int VideoCount { get; set; }
        public int TextCount { get; set; }
        public int ResourceCount { get; set; }
        public double? TotalMinutes { get; set; }
    }

    public class MetadataDto
    {
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; } // NOTE: DB chưa có cột updated_at cho Course
        public bool? Bestseller { get; set; }
        public bool? Trending { get; set; } // NOTE: DB chưa có cột trending
    }
}

