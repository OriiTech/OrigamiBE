namespace Origami.BusinessTier.Payload.Course
{
    public class ResourceSaveRequest
    {
        public int? Id { get; set; } // Nếu có thì update, không có thì create
        public int LectureId { get; set; }
        public string? Title { get; set; }
        public string? Url { get; set; }
        public string? Type { get; set; } // video/photo/pdf/doc/zip
    }
}

