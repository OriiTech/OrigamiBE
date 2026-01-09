using System.Collections.Generic;

namespace Origami.BusinessTier.Payload.Course
{
    public class LectureDetailResponse
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Url { get; set; }
        public string? Type { get; set; }
        public int? DurationMinutes { get; set; }
        public bool? PreviewAvailable { get; set; }
        public string? Note { get; set; } // NOTE: DB chưa có cột note cho Lecture
        public List<LectureResourceResponse> Resources { get; set; } = new();
        public List<LectureDiscussionResponse> Discussions { get; set; } = new(); // from Question/Answer at lecture level
    }

    public class LectureResourceResponse
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Url { get; set; }
        public string? Type { get; set; }
    }

    public class LectureDiscussionResponse
    {
        public int Id { get; set; }
        public string? Question { get; set; }
        public string? StudentName { get; set; }
        public string? Date { get; set; }
        public List<LectureAnswerResponse> Answers { get; set; } = new();
    }

    public class LectureAnswerResponse
    {
        public string? ResponseName { get; set; }
        public string? Date { get; set; }
        public string? Comment { get; set; }
    }
}

